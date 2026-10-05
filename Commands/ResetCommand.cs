using System;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class ResetCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public ResetCommand(IGameActions gameActions)
        {
            if (gameActions == null)
            {
                throw new ArgumentNullException(nameof(gameActions));
            }

            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.Reset();
        }
    }
}
