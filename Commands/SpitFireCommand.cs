using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class SpitFireCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public SpitFireCommand(IGameActions gameActions)
        {
            ArgumentNullException.ThrowIfNull(gameActions);

            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.SpitFire();
        }
    }
}
