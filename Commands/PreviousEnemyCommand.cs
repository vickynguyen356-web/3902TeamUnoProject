using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class PreviousEnemyCommand : ICommand
    {
        private readonly IDemoControls _demoControls;

        public PreviousEnemyCommand(IDemoControls demoControls)
        {
            _demoControls = demoControls;
        }

        public void Execute()
        {
            _demoControls.PreviousEnemy();
        }
    }
}
