using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class KoopaShell : Projectile
    {
        private const float MoveSpeed = 520f;
        private const float Gravity = 1500f;
        private const float MaximumFallSpeed = 700f;
        private const float KickGraceDurationSeconds = 0.4f;
        private readonly SpriteAnimation _shellAnimation;
        private float _kickGraceTimeRemaining;
        public const int ShellWidth = 40;
        public const int ShellHeight = 32;

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
            elapsedSeconds = System.Math.Min(elapsedSeconds, 1f / 30f);
            Velocity.Y = System.Math.Min(Velocity.Y + Gravity * elapsedSeconds, MaximumFallSpeed);
            Position = Position + Velocity * elapsedSeconds;
            if (_kickGraceTimeRemaining > 0f)
            {
                _kickGraceTimeRemaining = System.Math.Max(
                    0f, _kickGraceTimeRemaining - elapsedSeconds);
            }

            Sprite.Update(gameTime, _shellAnimation);
        }

        internal void Bounce()
        {
            Velocity.X = -Velocity.X;
        }

        internal bool IsMoving
        {
            get
            {
                return Velocity.X != 0f;
            }
        }

        internal bool CanDamageMario
        {
            get
            {
                return IsMoving && _kickGraceTimeRemaining == 0f;
            }
        }

        internal void Kick(float direction)
        {
            if (!IsDead && direction != 0f)
            {
                Velocity.X = System.Math.Sign(direction) * MoveSpeed;
                _kickGraceTimeRemaining = KickGraceDurationSeconds;
            }
        }

        internal void Stop()
        {
            Velocity.X = 0f;
            _kickGraceTimeRemaining = 0f;
        }
    }
}
