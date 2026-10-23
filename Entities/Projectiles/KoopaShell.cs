using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class KoopaShell : Projectile
    {
        private readonly SpriteAnimation _shellAnimation;
        public const int ShellWidth = 20;
        public const int ShellHeight = 16;

        public override int Width
        {
            get
            {
                return ShellWidth;
            }
        }

        public override int Height
        {
            get
            {
                return ShellHeight;
            }
        }

        public override ProjectileType Type
        {
            get
            {
                return ProjectileType.KoopaShell;
            }
        }

        public KoopaShell(ISprite sprite, Vector2 position, Vector2 velocity, bool isEnemyProjectile)
            : base(sprite, position, velocity, isEnemyProjectile)
        {
            _shellAnimation = ProjectileSpriteFactory.CreateShellAnimation();
            Sprite.Update(new GameTime(), _shellAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                return;
            }

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position = Position + Velocity * elapsedSeconds;
            Sprite.Update(gameTime, _shellAnimation);
        }

        internal void Bounce()
        {
            Velocity.X = -Velocity.X;
        }
    }
}
