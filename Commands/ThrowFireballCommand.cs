using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class ThrowFireballCommand : ICommand
    {
        private readonly IPlayer _player;

        public ThrowFireballCommand(IPlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _player = player;
        }

        public void Execute()
        {
            _player.ThrowFireball();
        }
    }
}
