using System;
using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class JumpCommand : ICommand
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
