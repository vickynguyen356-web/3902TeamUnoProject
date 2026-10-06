using Microsoft.Xna.Framework.Input;

namespace TeamUno.Mario.Input
{
    internal class KeyboardInput
    {
        private KeyboardState _previousKeyState;
        private KeyboardState _currentKeyState;

        public void Update(KeyboardState keyboardState)
        {
            _previousKeyState = _currentKeyState;
            _currentKeyState = keyboardState;
        }

        public bool IsDown(Keys key)
        {
            return _currentKeyState.IsKeyDown(key);
        }

        public bool WasPressed(Keys key)
        {
            return _currentKeyState.IsKeyDown(key) && !_previousKeyState.IsKeyDown(key);
        }
    }
}
