using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class CrouchCommand : ICommand
    {
        private readonly IPlayer _player;
        private readonly bool _shouldCrouch;

        public CrouchCommand(IPlayer player, bool shouldCrouch)
        {
            ArgumentNullException.ThrowIfNull(player);

            _player = player;
            _shouldCrouch = shouldCrouch;
        }

        public void Execute()
        {
            _player.SetCrouching(_shouldCrouch);
        }
    }
}
