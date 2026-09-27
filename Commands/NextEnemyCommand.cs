using System;
using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class NextEnemyCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public NextEnemyCommand(IGameActions gameActions)
        {
            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.NextEnemy();
        }
    }
}
