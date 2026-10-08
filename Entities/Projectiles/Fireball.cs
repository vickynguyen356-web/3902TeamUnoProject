using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class Fireball : Projectile
    {
        public const int FireballWidth = 16;
        public const int FireballHeight = 16;
        private const float Gravity = 500f;
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

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Velocity.Y = Velocity.Y + Gravity * elapsedSeconds;

            Position = Position + Velocity * elapsedSeconds;
            Sprite.Update(gameTime, _fireballAnimation);
        }

        public void OnCollision()
        {
            Kill();
        }
    }
}
