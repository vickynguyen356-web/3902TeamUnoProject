using System.Collections.Generic;
using TeamUno.Mario.Input;

namespace TeamUno.Mario.Interfaces
{
    internal interface IController
    {
        IReadOnlyList<InputAction> Actions
        {
            get;
        }

        bool IsJumpHeld
        {
            get;
        }

        void Update();
    }
}
