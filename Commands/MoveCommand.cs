using System;
using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly IPlayer _player;
        private readonly float _movementDirection;

        public MoveCommand(IPlayer player, float movementDirection)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _player = player;
            _movementDirection = movementDirection;
        }

        public void Execute()
        {
            _player.Move(_movementDirection);
        }
    }
}
