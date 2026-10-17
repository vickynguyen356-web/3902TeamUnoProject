using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    internal class KeyboardController : IController
    {
        private KeyboardState _previousKeyState;
        private bool _isJumpHeld;
        private readonly Dictionary<Keys, ICommand> _heldCommands;
        private readonly Dictionary<Keys, ICommand> _pressedCommands;
        private readonly List<ICommand> _pendingCommands = new List<ICommand>();
        private readonly ICommand _moveLeftCommand;
        private readonly ICommand _moveRightCommand;
        private readonly ICommand _crouchCommand;
        private readonly ICommand _standCommand;
        private readonly ICommand _jumpCommand;
        private readonly ICommand _quitCommand;
        private readonly ICommand _resetCommand;

        public bool IsJumpHeld
        {
            get
            {
                return _isJumpHeld;
            }
        }

        public KeyboardController(IPlayer player, IGameActions gameActions)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(gameActions);

            _moveLeftCommand = new MoveCommand(player, -1);
            _moveRightCommand = new MoveCommand(player, 1);
            _crouchCommand = new CrouchCommand(player, true);
            _standCommand = new CrouchCommand(player, false);
            _quitCommand = new QuitCommand(gameActions);
            _resetCommand = new ResetCommand(gameActions);
            _jumpCommand = new JumpCommand(player);
            ICommand throwFireballCommand = new ThrowFireballCommand(player);

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
                { Keys.Q, _quitCommand },
                { Keys.Escape, _quitCommand },
                { Keys.R, _resetCommand },
                { Keys.W, _jumpCommand },
                { Keys.Up, _jumpCommand },
                { Keys.Space, _jumpCommand },
                { Keys.Z, throwFireballCommand },
                { Keys.N, throwFireballCommand }
            };
        }

        public void Update()
        {
            Update(Keyboard.GetState());
        }

        public void Update(KeyboardState keyboardState)
        {
            ReadBindings(keyboardState);
            _previousKeyState = keyboardState;
            ExecuteCommands();
        }

        private void ReadBindings(KeyboardState keyboardState)
        {
            _pendingCommands.Clear();
            _isJumpHeld = false;

            foreach (Keys key in keyboardState.GetPressedKeys())
            {
                ICommand command;

                if (_heldCommands.TryGetValue(key, out command))
                {
                    _pendingCommands.Add(command);
                }

                if (_pressedCommands.TryGetValue(key, out command))
                {
                    if (command == _jumpCommand)
                    {
                        _isJumpHeld = true;
                    }

                    if (_previousKeyState.IsKeyUp(key))
                    {
                        _pendingCommands.Add(command);
                    }
                }
            }
        }

        private void ExecuteCommands()
        {
            if (_pendingCommands.Contains(_quitCommand))
            {
                _quitCommand.Execute();
                return;
            }

            if (_pendingCommands.Contains(_resetCommand))
            {
                _resetCommand.Execute();
                return;
            }

            if (!_pendingCommands.Contains(_crouchCommand))
            {
                _standCommand.Execute();
            }

            bool opposingMovement = _pendingCommands.Contains(_moveLeftCommand)
                && _pendingCommands.Contains(_moveRightCommand);

            foreach (ICommand command in _pendingCommands)
            {
                if (opposingMovement && (command == _moveLeftCommand || command == _moveRightCommand))
                {
                    continue;
                }

                command.Execute();
            }
        }
    }
}
