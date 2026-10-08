using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    internal class GamepadController : IController
    {
        public bool IsJumpHeld
        {
            get
            {
                return false;
            }
        }

        public void Update()
        {
        }
    }
}
