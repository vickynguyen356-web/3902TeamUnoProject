using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Commands
{
    public class PreviousBlockCommand : ICommand
    {
        private readonly IDemoControls _demoControls;

        public PreviousBlockCommand(IDemoControls demoControls)
        {
            _demoControls = demoControls;
        }

        public void Execute()
        {
            _demoControls.PreviousBlock();
        }
    }
}
