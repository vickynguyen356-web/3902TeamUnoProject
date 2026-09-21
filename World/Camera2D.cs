using System;
using Microsoft.Xna.Framework;

namespace Sprint0.World
{
    public class Camera2D
    {
        public Vector2 Position { get; private set; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }

        public Camera2D(int viewportWidth, int viewportHeight)
        {
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
        }

        public void Follow(Vector2 targetPosition, int worldWidth, int worldHeight)
        {
            // Center the target, then stop at the world edges instead of revealing space outside the level
            Vector2 desiredPosition = targetPosition - new Vector2(ViewportWidth / 2f, ViewportHeight / 2f);
            Position = new Vector2(
                MathHelper.Clamp(desiredPosition.X, 0, Math.Max(0, worldWidth - ViewportWidth)),
                MathHelper.Clamp(desiredPosition.Y, 0, Math.Max(0, worldHeight - ViewportHeight)));
        }

        // Moving the camera right shifts world drawing left by the same amount
        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position, 0));
            }
        }
    }
}
