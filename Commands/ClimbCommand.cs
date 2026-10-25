using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class ClimbCommand : ICommand
    {
        private readonly IPlayer _player;
        private readonly float _direction;

        public ClimbCommand(IPlayer player, float direction)
        {
            ArgumentNullException.ThrowIfNull(player);

            _player = player;
            _direction = direction;
        }

        public void Execute()
        {
            _player.Climb(_direction);
        }
    }
}
