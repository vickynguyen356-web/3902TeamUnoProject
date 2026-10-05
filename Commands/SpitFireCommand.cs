using TeamUno.Mario.Interfaces;
using System;

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
