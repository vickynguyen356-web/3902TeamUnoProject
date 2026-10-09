using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    internal class KeyboardController : IController
    {
        private KeyboardState _previousKeyState;
        private bool _isJumpHeld;
        private readonly Dictionary<Keys, InputAction> _heldActions;
        private readonly Dictionary<Keys, InputAction> _pressedActions;
        private readonly List<InputAction> _actions = new List<InputAction>();
        private readonly IReadOnlyList<InputAction> _actionView;

        public IReadOnlyList<InputAction> Actions
        {
            get
            {
                return _actionView;
            }
        }

        public bool IsJumpHeld
        {
            get
            {
                return _isJumpHeld;
            }
        }

        public KeyboardController()
        {
            _actionView = _actions.AsReadOnly();

            _heldActions = new Dictionary<Keys, InputAction>
            {
                { Keys.A, InputAction.MoveLeft },
                { Keys.Left, InputAction.MoveLeft },
                { Keys.D, InputAction.MoveRight },
                { Keys.Right, InputAction.MoveRight },
                { Keys.S, InputAction.Crouch },
                { Keys.Down, InputAction.Crouch }
            };

            _pressedActions = new Dictionary<Keys, InputAction>
            {
                { Keys.Q, InputAction.Quit },
                { Keys.Escape, InputAction.Quit },
                { Keys.R, InputAction.Reset },
                { Keys.W, InputAction.Jump },
                { Keys.Up, InputAction.Jump },
                { Keys.Space, InputAction.Jump },
                { Keys.Z, InputAction.ThrowFireball },
                { Keys.N, InputAction.ThrowFireball }
            };
        }

        public void Update()
        {
            Update(Keyboard.GetState());
        }

        public void Update(KeyboardState keyboardState)
        {
            _actions.Clear();
            _isJumpHeld = false;

            foreach (Keys key in keyboardState.GetPressedKeys())
            {
                InputAction action;

                if (_heldActions.TryGetValue(key, out action))
                {
                    _actions.Add(action);
                }

                if (_pressedActions.TryGetValue(key, out action))
                {
                    if (action == InputAction.Jump)
                    {
                        _isJumpHeld = true;
                    }

                    if (_previousKeyState.IsKeyUp(key))
                    {
                        _actions.Add(action);
                    }
                }
            }

            _previousKeyState = keyboardState;
        }
    }
}
