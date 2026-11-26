using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal sealed class PulleyPlatformPair
    {
        private const int TileSize = 48;
        private const float Acceleration = 540f;
        private const float DampingPerFrame = 0.92f;
        private readonly int _pulleyY;
        private float _leftVelocity;
        private float _fallVelocity;
        private IBlock _fallingPlatform;
        private bool _hasDetached;

        public IBlock Left { get; }
        public IBlock Right { get; }

        public Rectangle LeftRope
        {
            get
            {
                return GetRope(Left);
            }
        }

        public Rectangle RightRope
        {
            get
            {
                return GetRope(Right);
            }
        }

        public PulleyPlatformPair(IBlock left, IBlock right, int pulleyY)
        {
            Left = left;
            Right = right;
            _pulleyY = pulleyY;
        }

        public void Update(GameTime gameTime)
        {
            float elapsedSeconds = Math.Min(
                1f / 30f,
                Math.Max(0f, (float)gameTime.ElapsedGameTime.TotalSeconds));
            if (elapsedSeconds == 0f)
            {
                return;
            }

            if (_hasDetached)
            {
                _fallingPlatform.ConsumePlatformLevelContact();
                _fallVelocity += Acceleration * elapsedSeconds;
                Vector2 fallingPosition = _fallingPlatform.Position;
                fallingPosition.Y += _fallVelocity * elapsedSeconds;
                _fallingPlatform.MoveTo(fallingPosition);
                return;
            }

            bool leftContact = Left.ConsumePlatformLevelContact();
            bool rightContact = Right.ConsumePlatformLevelContact();
            if (leftContact != rightContact)
            {
                _leftVelocity += (leftContact ? Acceleration : -Acceleration)
                    * elapsedSeconds;
            }
            else
            {
                _leftVelocity *= (float)Math.Pow(
                    DampingPerFrame,
                    elapsedSeconds * 60f);
            }

            float leftY = Left.Position.Y + _leftVelocity * elapsedSeconds;
            float rightY = Right.Position.Y - _leftVelocity * elapsedSeconds;
            if (leftY <= _pulleyY)
            {
                leftY = _pulleyY;
                _fallingPlatform = Right;
                _fallVelocity = Math.Max(0f, -_leftVelocity);
                _hasDetached = true;
            }
            else if (rightY <= _pulleyY)
            {
                rightY = _pulleyY;
                _fallingPlatform = Left;
                _fallVelocity = Math.Max(0f, _leftVelocity);
                _hasDetached = true;
            }

            Left.MoveTo(new Vector2(Left.Position.X, leftY));
            Right.MoveTo(new Vector2(Right.Position.X, rightY));
        }

        private Rectangle GetRope(IBlock platform)
        {
            int top = _pulleyY;
            int bottom = (int)platform.Bounds.Top;
            return new Rectangle(
                (int)platform.Position.X + TileSize,
                top,
                TileSize,
                Math.Max(0, bottom - top));
        }
    }
}
