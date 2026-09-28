using System;
using Microsoft.Xna.Framework.Input;
using Sprint0.Commands;
using Sprint0.Interfaces;

namespace Sprint0.Input
{
    public class KeyboardController : IController
    {
        private readonly ICommand _moveLeftCommand;
        private readonly ICommand _moveRightCommand;
        private readonly ICommand _jumpCommand;
        private readonly ICommand _crouchCommand;
        private readonly ICommand _standCommand;
        private readonly ICommand _throwFireballCommand;
        private readonly ICommand _quitCommand;
        private readonly ICommand _resetCommand;
        private readonly ICommand _previousEnemyCommand;
        private readonly ICommand _nextEnemyCommand;
        private readonly ICommand _damageCommand;
        private KeyboardState _previousKeyState;
        private KeyboardState _currentKeyState;

        public KeyboardController(IPlayer player, IGameActions gameActions)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (gameActions == null)
            {
                throw new ArgumentNullException(nameof(gameActions));
            }

            _moveLeftCommand = new MoveCommand(player, -1);
            _moveRightCommand = new MoveCommand(player, 1);
            _jumpCommand = new JumpCommand(player);
            _crouchCommand = new CrouchCommand(player, true);
            _standCommand = new CrouchCommand(player, false);
            _throwFireballCommand = new ThrowFireballCommand(player);
            _quitCommand = new QuitCommand(gameActions);
            _resetCommand = new ResetCommand(gameActions);
            _previousEnemyCommand = new PreviousEnemyCommand(gameActions);
            _nextEnemyCommand = new NextEnemyCommand(gameActions);
            _damageCommand = new DamageCommand(gameActions);
        }

        public void Update()
        {
            _previousKeyState = _currentKeyState;
            _currentKeyState = Keyboard.GetState();

            bool moveLeft = _currentKeyState.IsKeyDown(Keys.A) || _currentKeyState.IsKeyDown(Keys.Left);
            bool moveRight = _currentKeyState.IsKeyDown(Keys.D) || _currentKeyState.IsKeyDown(Keys.Right);
            bool crouch = _currentKeyState.IsKeyDown(Keys.S) || _currentKeyState.IsKeyDown(Keys.Down);

            if (moveLeft && !moveRight)
            {
                _moveLeftCommand.Execute();
            }

            if (moveRight && !moveLeft)
            {
                _moveRightCommand.Execute();
            }

            if (crouch)
            {
                _crouchCommand.Execute();
            }
            else
            {
                _standCommand.Execute();
            }

            if (WasPressed(Keys.W) || WasPressed(Keys.Up) || WasPressed(Keys.Space))
            {
                _jumpCommand.Execute();
            }

            if (WasPressed(Keys.Z) || WasPressed(Keys.N))
            {
                _throwFireballCommand.Execute();
            }

            if (WasPressed(Keys.Q) || WasPressed(Keys.Escape))
            {
                _quitCommand.Execute();
            }

            if (WasPressed(Keys.R))
            {
                _resetCommand.Execute();
            }

            if (WasPressed(Keys.O))
            {
                _previousEnemyCommand.Execute();
            }

            if (WasPressed(Keys.P))
            {
                _nextEnemyCommand.Execute();
            }

            if (WasPressed(Keys.E))
            {
                _damageCommand.Execute();
            }
        }

        private bool WasPressed(Keys key)
        {
            return _currentKeyState.IsKeyDown(key) && !_previousKeyState.IsKeyDown(key);
        }
    }
}
