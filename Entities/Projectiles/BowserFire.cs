using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class BowserFire : Projectile
    {
        public const int FireWidth = 48;
        public const int FireHeight = 16;
        private readonly SpriteAnimation _fireAnimation;

        public override int Width
        {
            get
            {
                return FireWidth;
            }
        }

        public override int Height
        {
            get
            {
                return FireHeight;
            }
        }

        public override ProjectileType Type
        {
            get
            {
                return ProjectileType.BowserFire;
            }
        }

        public BowserFire(ISprite sprite, Vector2 position, Vector2 velocity, bool isEnemyProjectile)
            : base(sprite, position, velocity, isEnemyProjectile)
        {
            _fireAnimation = ProjectileSpriteFactory.CreateBowserFireAnimation();
            Sprite.Update(new GameTime(), _fireAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                return;
            }

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position = Position + Velocity * elapsedSeconds;
            Sprite.Update(gameTime, _fireAnimation);
        }
    }
}
