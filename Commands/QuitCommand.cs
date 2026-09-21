using System;
using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class QuitCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public QuitCommand(IGameActions gameActions)
        {
            if (gameActions == null)
            {
                throw new ArgumentNullException(nameof(gameActions));
            }

            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.Quit();
        }
    }
}
