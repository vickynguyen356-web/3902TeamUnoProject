using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal class Camera
    {
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private int _levelWidth;
        private int _levelHeight;
        private float? _maximumX;
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

        public Camera(int viewportWidth, int viewportHeight, int levelWidth, int levelHeight)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(viewportWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(viewportHeight);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelHeight);

            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _levelWidth = levelWidth;
            _levelHeight = levelHeight;
            _position = Vector2.Zero;
        }

        public void SetLevelSize(int levelWidth, int levelHeight)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelHeight);
            _levelWidth = levelWidth;
            _levelHeight = levelHeight;
            _maximumX = null;
            MoveTo(Position.X, Position.Y);
        }

        public void SetMaximumX(float maximumX)
        {
            _maximumX = Math.Max(0f, maximumX);
            MoveTo(Position.X, Position.Y);
        }

        public void Follow(Rectangle targetBounds)
        {
            if (!IsFollowing)
            {
                return;
            }

            float targetX = targetBounds.Center.X - _viewportWidth / 2f;
            MoveTo(targetX, Position.Y);
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
            MoveTo(x, Position.Y);
        }

        public void MoveTo(float x, float y)
        {
            float maximumX = Math.Max(0, _levelWidth - _viewportWidth);
            if (_maximumX.HasValue)
            {
                maximumX = Math.Min(maximumX, _maximumX.Value);
            }
            float maximumY = Math.Max(0, _levelHeight - _viewportHeight);
            float cameraX = MathHelper.Clamp(x, 0, maximumX);
            float cameraY = MathHelper.Clamp(y, 0, maximumY);

            _position = new Vector2((float)Math.Floor(cameraX), (float)Math.Floor(cameraY));
        }
    }
}
