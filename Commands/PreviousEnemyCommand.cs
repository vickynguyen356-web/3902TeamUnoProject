using System;
using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class PreviousEnemyCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public PreviousEnemyCommand (IGameActions gameActions)
        {
            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.PreviousEnemy();
        }
    }
}
