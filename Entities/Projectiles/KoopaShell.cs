using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class KoopaShell : Projectile
    {
        private const float MoveSpeed = 520f;
        private const float NonEnemyProjectileDurationSeconds = 0.4f;
        private readonly SpriteAnimation _shellAnimation;
        private float _nonEnemyProjectileTimeRemaining;
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
            if (_nonEnemyProjectileTimeRemaining > 0f)
            {
                _nonEnemyProjectileTimeRemaining = System.Math.Max(
                    0f, _nonEnemyProjectileTimeRemaining - elapsedSeconds);
                if (_nonEnemyProjectileTimeRemaining == 0f)
                {
                    IsEnemyProjectile = true;
                }
            }

            Sprite.Update(gameTime, _shellAnimation);
        }

        internal void Bounce()
        {
            Velocity.X = -Velocity.X;
        }

        internal bool IsMoving
        {
            get { return Velocity.X != 0f; }
        }

        internal void Kick(float direction)
        {
            if (!IsDead && direction != 0f)
            {
                Velocity.X = System.Math.Sign(direction) * MoveSpeed;
                IsEnemyProjectile = false;
                _nonEnemyProjectileTimeRemaining = NonEnemyProjectileDurationSeconds;
            }
        }

        internal void Stop()
        {
            Velocity.X = 0f;
        }
    }
}
