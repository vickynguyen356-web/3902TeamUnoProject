using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    public class NextBlockCommand : ICommand
    {
        private readonly IDemoControls _demoControls;

        public NextBlockCommand(IDemoControls demoControls)
        {
            _demoControls = demoControls;
        }

        public void Execute()
        {
            _demoControls.NextBlock();
        }
    }
}
