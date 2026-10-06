using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    public class SpitFireCommand : ICommand
    {
        private readonly IGameActions _gameActions;

        public SpitFireCommand(IGameActions gameActions)
        {
            _gameActions = gameActions;
        }

        public void Execute()
        {
            _gameActions.SpitFire();
        }
    }
}
