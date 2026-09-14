using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGameLibrary.Interfaces;

namespace MonoGameLibrary.Input
{
    public class MouseController : IController
    {
        private MouseState _currentMouseRef;
        private MouseState _previousMouseRef;
        private KeyboardState _currentKeyRef;

        public void Update()
        {
            _previousMouseRef = _currentMouseRef;
            _currentMouseRef = Mouse.GetState();

            // for EscQuit() esc key condition
            _currentKeyRef = Keyboard.GetState();
        }

        public bool EscQuit()
        {
            return _currentKeyRef.IsKeyDown(Keys.Escape);
        }

        public Vector2 GetMovementDirection()
        {
            // mouse doesn't control movement
            return Vector2.Zero;

        }

        public bool IsJumpRequested()
        {
            return _currentMouseRef.RightButton == ButtonState.Pressed &&
                _previousMouseRef.RightButton == ButtonState.Released;
        }
    }
}