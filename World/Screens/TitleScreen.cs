using Microsoft.Xna.Framework.Input;

namespace TeamUno.Mario.World
{
    internal class TitleScreen
    {
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        public void ResetInput(KeyboardState keyboardState, GamePadState gamePadState)
        {
            _previousKeyboardState = keyboardState;
            _previousGamePadState = gamePadState;
        }

        public bool Update(KeyboardState keyboardState, GamePadState gamePadState)
        {
            bool keyboardStartPressed =
                (keyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
                || (keyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space));
            bool gamePadStartPressed =
                (gamePadState.IsButtonDown(Buttons.A) && _previousGamePadState.IsButtonUp(Buttons.A))
                || (gamePadState.IsButtonDown(Buttons.Start) && _previousGamePadState.IsButtonUp(Buttons.Start));

            _previousKeyboardState = keyboardState;
            _previousGamePadState = gamePadState;
            return keyboardStartPressed || gamePadStartPressed;
        }
    }
}
