using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    internal class NextEnemyCommand : ICommand
    {
        private readonly IDemoControls _demoControls;

        public NextEnemyCommand(IDemoControls demoControls)
        {
            _demoControls = demoControls;
        }

        public void Execute()
        {
            _demoControls.NextEnemy();
        }
    }
}
