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
        private readonly IGameActions _gameActions;
        private readonly KeyBindings _bindings;
        private readonly ICommand _moveLeftCommand;
        private readonly ICommand _moveRightCommand;
        private readonly ICommand _crouchCommand;
        private readonly ICommand _standCommand;
        private readonly ICommand _jumpCommand;
        private readonly ICommand _climbUpCommand;
        private readonly ICommand _climbDownCommand;
        private readonly ICommand _stopClimbingCommand;
        private readonly ICommand _quitCommand;
        private readonly ICommand _resetCommand;
        private readonly ICommand _pauseCommand;
        private readonly ICommand _muteCommand;
        private readonly ICommand _enterPipeCommand;
        private readonly ICommand _throwFireballCommand;
        private readonly IReadOnlyList<LoadAreaCommand> _areaCommands;

        public bool IsJumpHeld
        {
            get
            {
                return _isJumpHeld;
            }
        }

        public KeyboardController(IPlayer player, IGameActions gameActions, KeyBindings bindings)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(gameActions);
            ArgumentNullException.ThrowIfNull(bindings);

            _gameActions = gameActions;
            _bindings = bindings;

            _moveLeftCommand = new MoveCommand(player, -1);
            _moveRightCommand = new MoveCommand(player, 1);
            _crouchCommand = new CrouchCommand(player, true);
            _standCommand = new CrouchCommand(player, false);
            _quitCommand = new QuitCommand(gameActions);
            _resetCommand = new ResetCommand(gameActions);
            _pauseCommand = new PauseCommand(gameActions);
            _muteCommand = new MuteCommand(gameActions);
            _jumpCommand = new JumpCommand(player);
            _climbUpCommand = new ClimbCommand(player, -1);
            _climbDownCommand = new ClimbCommand(player, 1);
            _stopClimbingCommand = new ClimbCommand(player, 0);
            _enterPipeCommand = new EnterPipeCommand(player);
            _throwFireballCommand = new ThrowFireballCommand(player);

            _areaCommands = new List<LoadAreaCommand>
            {
                new LoadAreaCommand(gameActions, "overworld"),
                new LoadAreaCommand(gameActions, "underground"),
                new LoadAreaCommand(gameActions, "test"),
                new LoadAreaCommand(gameActions, "test", "items"),
                new LoadAreaCommand(gameActions, "test", "combat"),
                new LoadAreaCommand(gameActions, "test", "bowser"),
                new LoadAreaCommand(gameActions, "test", "pipes"),
                new LoadAreaCommand(gameActions, "bowser-party")
            };

            _heldCommands = new Dictionary<Keys, ICommand>();
            _pressedCommands = new Dictionary<Keys, ICommand>();

            // must come after all the commands above exist
            BuildMaps();
            _bindings.Changed += BuildMaps;
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

        // call after the menu closes so keys held in the menu don't count as new presses
        public void ResetInput()
        {
            _previousKeyState = Keyboard.GetState();
        }

        private void BuildMaps()
        {
            _heldCommands.Clear();
            _heldCommands[_bindings.Get(GameAction.MoveLeft)] = _moveLeftCommand;
            _heldCommands[Keys.Left] = _moveLeftCommand;
            _heldCommands[_bindings.Get(GameAction.MoveRight)] = _moveRightCommand;
            _heldCommands[Keys.Right] = _moveRightCommand;
            _heldCommands[_bindings.Get(GameAction.Crouch)] = _crouchCommand;
            _heldCommands[Keys.Down] = _crouchCommand;

            _pressedCommands.Clear();
            _pressedCommands[Keys.Q] = _quitCommand;   // Escape now opens the menu
            _pressedCommands[_bindings.Get(GameAction.Restart)] = _resetCommand;
            _pressedCommands[_bindings.Get(GameAction.Pause)] = _pauseCommand;
            _pressedCommands[_bindings.Get(GameAction.Mute)] = _muteCommand;
            _pressedCommands[Keys.Down] = _enterPipeCommand;
            _pressedCommands[_bindings.Get(GameAction.Crouch)] = _enterPipeCommand;
            _pressedCommands[_bindings.Get(GameAction.Jump)] = _jumpCommand;
            _pressedCommands[Keys.Up] = _jumpCommand;
            _pressedCommands[Keys.Space] = _jumpCommand;
            _pressedCommands[_bindings.Get(GameAction.Fireball)] = _throwFireballCommand;
            _pressedCommands[Keys.N] = _throwFireballCommand;

            Keys[] areaKeys = { Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8 };
            for (int index = 0; index < areaKeys.Length; index = index + 1)
            {
                _pressedCommands[areaKeys[index]] = _areaCommands[index];
            }
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

                if (key == _bindings.Get(GameAction.Jump) || key == Keys.Up)
                {
                    _pendingCommands.Add(_climbUpCommand);
                }
                else if (key == _bindings.Get(GameAction.Crouch) || key == Keys.Down)
                {
                    _pendingCommands.Add(_climbDownCommand);
                }
            }

            bool opposingClimb = _pendingCommands.Contains(_climbUpCommand)
                && _pendingCommands.Contains(_climbDownCommand);
            if (!_pendingCommands.Contains(_climbUpCommand)
                && !_pendingCommands.Contains(_climbDownCommand)
                || opposingClimb)
            {
                _pendingCommands.Add(_stopClimbingCommand);
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

            foreach (ICommand command in _pendingCommands)
            {
                if (command is LoadAreaCommand)
                {
                    command.Execute();
                    return;
                }
            }

            if (_pendingCommands.Contains(_pauseCommand))
            {
                _pauseCommand.Execute();
                return;
            }

            if (!_gameActions.CanControlPlayer)
            {
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

                if (_pendingCommands.Contains(_climbUpCommand)
                    && _pendingCommands.Contains(_climbDownCommand)
                    && (command == _climbUpCommand || command == _climbDownCommand))
                {
                    continue;
                }

                command.Execute();
            }
        }
    }
}