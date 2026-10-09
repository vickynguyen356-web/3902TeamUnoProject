using System;
using System.Collections.Generic;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    internal class GamepadController : IController
    {
        private readonly IReadOnlyList<InputAction> _actions = Array.Empty<InputAction>();

        public IReadOnlyList<InputAction> Actions
        {
            get
            {
                return _actions;
            }
        }

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
