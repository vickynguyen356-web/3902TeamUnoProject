using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class PauseCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public PauseCommand(IGameActions gameActions)
        {
            ArgumentNullException.ThrowIfNull(gameActions);

            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.TogglePause();
        }
    }
}
