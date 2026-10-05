using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class MoveCommand : ICommand
    {
        private readonly IPlayer _player;
        private readonly float _movementDirection;

        public MoveCommand(IPlayer player, float movementDirection)
        {
            ArgumentNullException.ThrowIfNull(player);

            _player = player;
            _movementDirection = movementDirection;
        }

        public void Execute()
        {
            _player.Move(_movementDirection);
        }
    }
}
