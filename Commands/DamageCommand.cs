using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class DamageCommand : ICommand
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
