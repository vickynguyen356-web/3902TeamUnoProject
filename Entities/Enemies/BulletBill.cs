using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal sealed class BulletBill : Enemy
    {
        public const int BulletBillWidth = 48;
        public const int BulletBillHeight = 48;
        private const float HorizontalSpeed = 180f;
        public const int DefaultTileId = 90;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _animations;

        public override int Width
        {
            get { return BulletBillWidth; }
        }

        public override int Height
        {
            get { return BulletBillHeight; }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + 4,
                    (int)Position.Y + 4,
                    BulletBillWidth - 8,
                    BulletBillHeight - 8);
            }
        }

        public BulletBill(
            ISprite sprite,
            Vector2 position,
            float direction,
            int sourceTileId = DefaultTileId)
            : base(sprite, position)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(direction, 0f);

            Velocity = new Vector2(Math.Sign(direction) * HorizontalSpeed, 0f);
            SpriteFrame frame = EnemySpriteSheet.CreateEnemyIdFrame(sourceTileId, 16, 16);
            _animations = new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, new SpriteAnimation(1f, frame) }
            };
            UpdateMovementAnimation(new GameTime(), _animations);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            if (!IsDead)
            {
                Position += Velocity * elapsedSeconds;
            }

            UpdateMovementAnimation(gameTime, _animations);
        }

        public override void UpdateGravity(GameTime gameTime)
        {
            if (IsDead)
            {
                base.UpdateGravity(gameTime);
            }
        }
    }
}
