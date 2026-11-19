using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.World;

namespace TeamUno.Mario.Entities.Enemies
{
    internal sealed class FireBarSegment : Enemy
    {
        private const float DegreesPerSecond = 100f;
        private const int SegmentRadius = 16;
        private const int SegmentSize = 48;
        private const int HazardBoundsSize = 12;
        private static readonly SpriteAnimation FireAnimation = CreateFireAnimation();

        private readonly Vector2 _rotationPoint;
        private readonly int _segmentIndex;
        private readonly float _rotationSign;
        private readonly bool _isHazard;
        private float _angle;

        public override int Width
        {
            get { return SegmentSize; }
        }

        public override int Height
        {
            get { return SegmentSize; }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                if (!_isHazard)
                {
                    return Rectangle.Empty;
                }

                return new Rectangle(
                    (int)Position.X + (SegmentSize - HazardBoundsSize) / 2,
                    (int)Position.Y + (SegmentSize - HazardBoundsSize) / 2,
                    HazardBoundsSize,
                    HazardBoundsSize);
            }
        }

        public FireBarSegment(
            ISprite sprite,
            Vector2 rotationPoint,
            int segmentIndex,
            float startingAngle,
            string rotationDirection,
            bool isHazard)
            : base(sprite, GetSegmentPosition(rotationPoint, segmentIndex, startingAngle))
        {
            _rotationPoint = rotationPoint;
            _segmentIndex = segmentIndex;
            _angle = startingAngle;
            _rotationSign = GetRotationSign(rotationDirection);
            _isHazard = isHazard;
            Sprite.Update(new GameTime(), FireAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            _angle = (_angle + _rotationSign * DegreesPerSecond * elapsedSeconds) % 360f;
            if (_angle < 0f)
            {
                _angle += 360f;
            }

            Position = GetSegmentPosition(_rotationPoint, _segmentIndex, _angle);
            Sprite.Update(gameTime, FireAnimation);
        }

        public override void UpdateGravity(GameTime gameTime)
        {
        }

        public override void TakeDamage()
        {
        }

        public override void Kill()
        {
        }

        private static Vector2 GetSegmentPosition(Vector2 center, int segmentIndex, float angle)
        {
            float radius = segmentIndex * SegmentRadius;
            float radians = MathHelper.ToRadians(angle);
            return center + new Vector2(
                (float)Math.Cos(radians) * radius,
                -(float)Math.Sin(radians) * radius);
        }

        private static float GetRotationSign(string direction)
        {
            string normalizedDirection = direction.Replace("_", string.Empty, StringComparison.Ordinal);
            if (string.Equals(normalizedDirection, "CLOCKWISE", StringComparison.OrdinalIgnoreCase))
            {
                return -1f;
            }

            if (string.Equals(normalizedDirection, "COUNTERCLOCKWISE", StringComparison.OrdinalIgnoreCase))
            {
                return 1f;
            }

            if (string.Equals(normalizedDirection, "NONE", StringComparison.OrdinalIgnoreCase))
            {
                return 0f;
            }

            throw new ArgumentException("Unsupported fire bar rotation direction.", nameof(direction));
        }

        private static SpriteAnimation CreateFireAnimation()
        {
            SpriteFrame[] frames = new SpriteFrame[4];
            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                Rectangle source = LevelTile.GetSourceRectangle(611 + frameIndex);
                frames[frameIndex] = new SpriteFrame(source, source);
            }

            return new SpriteAnimation(1f / 12f, frames);
        }
    }
}
