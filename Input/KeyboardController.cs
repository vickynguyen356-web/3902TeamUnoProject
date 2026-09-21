using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sprint0.Interfaces;

namespace Sprint0.Input
{
    public class KeyboardController : IController
    {
        private KeyboardState _currentKeyRef;

        /// <summary>
        /// gets current KeyboardState
        /// </summary>
        public void Update()
        {
            _currentKeyRef = Keyboard.GetState();
        }

        /// <summary>
        /// exits the game 
        /// </summary>
        /// <returns> true if esc key was pressed; false otherwise</returns>
        public bool EscQuit()
        {
            return _currentKeyRef.IsKeyDown(Keys.Escape);
        }

        /// <summary>
        /// gets direction that character moves based on keyboard input
        /// </summary>
        /// <returns> unit vector, if not (0,0), for direction of movement for character </returns>
        public Vector2 GetMovementDirection()
        {
            Vector2 direction = Vector2.Zero;

            if (_currentKeyRef.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (_currentKeyRef.IsKeyDown(Keys.S)) direction.Y += 1;
            if (_currentKeyRef.IsKeyDown(Keys.A)) direction.X -= 1;
            if (_currentKeyRef.IsKeyDown(Keys.D)) direction.X += 1;

            // turns vector into unit vector 
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            return direction;
        }

        public float GetRotation(Vector2 charPosition)
        {
            // keyboard input doesn't control rotation
            return 0f;
        }

        public bool IsJumpRequested()
        {
            // keyboard input doesn't control jump
            return false;
        }
    }
}
