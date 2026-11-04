using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class MuteCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public MuteCommand(IGameActions gameActions)
        {
            ArgumentNullException.ThrowIfNull(gameActions);

            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.ToggleMute();
        }
    }
}
