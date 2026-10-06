using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal class Camera
    {
        private readonly int _viewportWidth;
        private readonly int _levelWidth;
        private Vector2 _position;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
            }
        }

        public Camera(int viewportWidth, int levelWidth)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(viewportWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelWidth);

            _viewportWidth = viewportWidth;
            _levelWidth = levelWidth;
            _position = Vector2.Zero;
        }

        public void Follow(Rectangle targetBounds)
        {
            float targetX = targetBounds.Center.X - _viewportWidth / 2f;
            float maximumX = Math.Max(0, _levelWidth - _viewportWidth);
            float cameraX = MathHelper.Clamp(targetX, 0, maximumX);

            _position = new Vector2((float)Math.Floor(cameraX), 0);
        }
    }
}
