using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal class Camera
    {
        private readonly int _viewportWidth;
        private readonly int _levelWidth;
        private Vector2 _position;
        private bool _isFollowing = true;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public bool IsFollowing
        {
            get
            {
                return _isFollowing;
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
            if (!IsFollowing)
            {
                return;
            }

            float targetX = targetBounds.Center.X - _viewportWidth / 2f;
            MoveTo(targetX);
        }

        public void PauseFollowing()
        {
            _isFollowing = false;
        }

        public void ResumeFollowing()
        {
            _isFollowing = true;
        }

        public void MoveTo(float x)
        {
            float maximumX = Math.Max(0, _levelWidth - _viewportWidth);
            float cameraX = MathHelper.Clamp(x, 0, maximumX);

            _position = new Vector2((float)Math.Floor(cameraX), 0);
        }
    }
}
