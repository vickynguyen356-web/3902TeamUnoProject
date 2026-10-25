using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class EnterPipeCommand : ICommand
    {
        private readonly IPlayer _player;

        public EnterPipeCommand(IPlayer player)
        {
            ArgumentNullException.ThrowIfNull(player);
            _player = player;
        }

        public void Execute()
        {
            _player.RequestPipeEntry();
        }
    }
}
