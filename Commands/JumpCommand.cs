using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class JumpCommand : ICommand
    {
        private readonly IPlayer _player;

        public JumpCommand(IPlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _player = player;
        }

        public void Execute()
        {
            _player.Jump();
        }
    }
}
