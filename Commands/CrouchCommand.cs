using System;
using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class CrouchCommand : ICommand
    {
        private readonly IPlayer _player;
        private readonly bool _shouldCrouch;

        public CrouchCommand(IPlayer player, bool shouldCrouch)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _player = player;
            _shouldCrouch = shouldCrouch;
        }

        public void Execute()
        {
            _player.SetCrouching(_shouldCrouch);
        }
    }
}
