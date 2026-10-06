using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    public class KeyboardController : IController
    {
        private readonly KeyboardInput _input;
        private readonly Dictionary<Keys, ICommand> _heldCommands;
        private readonly Dictionary<Keys, ICommand> _pressedCommands;
        private readonly List<ICommand> _activeCommands = new List<ICommand>();
        private readonly ICommand _moveLeftCommand;
        private readonly ICommand _moveRightCommand;
        private readonly ICommand _crouchCommand;
        private readonly ICommand _standCommand;

        public KeyboardController(IPlayer player, IGameActions gameActions, KeyboardInput input)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (gameActions == null)
            {
                throw new ArgumentNullException(nameof(gameActions));
            }

            _input = input;
            _moveLeftCommand = new MoveCommand(player, -1);
            _moveRightCommand = new MoveCommand(player, 1);
            _crouchCommand = new CrouchCommand(player, true);
            _standCommand = new CrouchCommand(player, false);
            ICommand jumpCommand = new JumpCommand(player);
            ICommand throwFireballCommand = new ThrowFireballCommand(player);
            ICommand spitFireCommand = new SpitFireCommand(gameActions);

            _heldCommands = new Dictionary<Keys, ICommand>
            {
                { Keys.A, _moveLeftCommand },
                { Keys.Left, _moveLeftCommand },
                { Keys.D, _moveRightCommand },
                { Keys.Right, _moveRightCommand },
                { Keys.S, _crouchCommand },
                { Keys.Down, _crouchCommand }
            };

            _pressedCommands = new Dictionary<Keys, ICommand>
            {
                { Keys.W, jumpCommand },
                { Keys.Up, jumpCommand },
                { Keys.Space, jumpCommand },
                { Keys.Z, throwFireballCommand },
                { Keys.N, throwFireballCommand },
                { Keys.B, spitFireCommand }
            };
        }

        public void Update()
        {
            CollectActiveCommands(_heldCommands, false);
            CancelOpposingMovement();

            if (!_activeCommands.Contains(_crouchCommand))
            {
                _standCommand.Execute();
            }

            ExecuteActiveCommands();

            CollectActiveCommands(_pressedCommands, true);
            ExecuteActiveCommands();
        }

        private void CollectActiveCommands(Dictionary<Keys, ICommand> bindings, bool newlyPressedOnly)
        {
            _activeCommands.Clear();

            foreach (KeyValuePair<Keys, ICommand> binding in bindings)
            {
                bool isActive = _input.IsDown(binding.Key);
                if (newlyPressedOnly)
                {
                    isActive = _input.WasPressed(binding.Key);
                }

                if (isActive && !_activeCommands.Contains(binding.Value))
                {
                    _activeCommands.Add(binding.Value);
                }
            }
        }

        private void CancelOpposingMovement()
        {
            if (_activeCommands.Contains(_moveLeftCommand) && _activeCommands.Contains(_moveRightCommand))
            {
                _activeCommands.Remove(_moveLeftCommand);
                _activeCommands.Remove(_moveRightCommand);
            }
        }

        private void ExecuteActiveCommands()
        {
            foreach (ICommand command in _activeCommands)
            {
                command.Execute();
            }
        }
    }
}
