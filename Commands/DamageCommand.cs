using Sprint0.Interfaces;

namespace Sprint0.Commands
{
    public class DamageCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public DamageCommand(IGameActions gameActions)
        {
            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.TriggerDamage();
        }
    }
}
