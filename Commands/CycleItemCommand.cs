using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    public class CycleItemCommand : ICommand
    {
        private readonly IDemoControls _demoControls;
        private readonly int _direction;

        public CycleItemCommand(IDemoControls demoControls, int direction)
        {
            _demoControls = demoControls;
            _direction = direction;
        }

        public void Execute()
        {
            _demoControls.CycleItem(_direction);
        }
    }
}
