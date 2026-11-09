using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class Fireball : Projectile
    {
        public const int FireballWidth = 16;
        public const int FireballHeight = 16;
        public const float HorizontalSpeed = 400f;
        public const float BlockBounceSpeed = 240f;
        private const float Gravity = 1200f;
        private const float LifetimeSeconds = 3f;
        private const float MaximumElapsedSeconds = 1f / 30f;
        private float _timeRemaining = LifetimeSeconds;
        private readonly SpriteAnimation _fireballAnimation;

        public override int Width
        {
            get
            {
                return FireballWidth;
            }
        }

        public override int Height
        {
            get
            {
                return FireballHeight;
            }
        }

        public override ProjectileType Type
        {
            get
            {
                return ProjectileType.Fireball;
            }
        }

        public Fireball(ISprite sprite, Vector2 position, Vector2 velocity, bool isEnemyProjectile)
            : base(sprite, position, velocity, isEnemyProjectile)
        {
            _fireballAnimation = ProjectileSpriteFactory.CreateFireballAnimation();

            Sprite.Update(new GameTime(), _fireballAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                return;
            }

            float elapsedSeconds = Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, MaximumElapsedSeconds);
            _timeRemaining = _timeRemaining - elapsedSeconds;
            if (_timeRemaining <= 0f)
            {
                Kill();
                return;
            }

            Position = Position + Velocity * elapsedSeconds
                + new Vector2(0f, Gravity * elapsedSeconds * elapsedSeconds / 2f);
            Velocity.Y = Velocity.Y + Gravity * elapsedSeconds;
            Sprite.Update(gameTime, _fireballAnimation);
        }

        public void OnCollision()
        {
            Kill();
        }

        internal void Bounce()
        {
            Velocity.Y = -BlockBounceSpeed;
        }
    }
}
