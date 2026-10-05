using System;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    internal class KeyboardController : IController
    {
        private readonly KeyboardInput _input;
        private readonly ICommand _moveLeftCommand;
        private readonly ICommand _moveRightCommand;
        private readonly ICommand _jumpCommand;
        private readonly ICommand _crouchCommand;
        private readonly ICommand _standCommand;
        private readonly ICommand _throwFireballCommand;
        private readonly ICommand _damageCommand;
        private readonly ICommand _spitFireCommand;

        public KeyboardController(IPlayer player, IGameActions gameActions, KeyboardInput input)
        {
            ArgumentNullException.ThrowIfNull(player);

            ArgumentNullException.ThrowIfNull(gameActions);

            ArgumentNullException.ThrowIfNull(input);

            _input = input;
            _moveLeftCommand = new MoveCommand(player, -1);
            _moveRightCommand = new MoveCommand(player, 1);
            _jumpCommand = new JumpCommand(player);
            _crouchCommand = new CrouchCommand(player, true);
            _standCommand = new CrouchCommand(player, false);
            _throwFireballCommand = new ThrowFireballCommand(player);
            _damageCommand = new DamageCommand(gameActions);
            _spitFireCommand = new SpitFireCommand(gameActions);
        }

        public void Update()
        {
            HandleMovementKeys();
            HandlePlayerActionKeys();
            HandleDamageKey();
        }

        private void HandleMovementKeys()
        {
            bool moveLeft = _input.IsDown(Keys.A) || _input.IsDown(Keys.Left);
            bool moveRight = _input.IsDown(Keys.D) || _input.IsDown(Keys.Right);
            bool crouch = _input.IsDown(Keys.S) || _input.IsDown(Keys.Down);

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
        }

        private void HandlePlayerActionKeys()
        {
            if (_input.WasPressed(Keys.W) || _input.WasPressed(Keys.Up) || _input.WasPressed(Keys.Space))
            {
                _jumpCommand.Execute();
            }

            if (_input.WasPressed(Keys.Z) || _input.WasPressed(Keys.N))
            {
                _throwFireballCommand.Execute();
            }

            if (_input.WasPressed(Keys.B))
            {
                _spitFireCommand.Execute();
            }
        }

        private void HandleDamageKey()
        {
            if (_input.WasPressed(Keys.E))
            {
                _damageCommand.Execute();
            }

            if (_input.WasPressed(Keys.B))
            {
                _spitFireCommand.Execute();
            }
        }
    }
}
