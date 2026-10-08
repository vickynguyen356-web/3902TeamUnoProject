using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal class Hammer : Projectile
    {
        public const int HammerWidth = 16;
        public const int HammerHeight = 32;
        private const float Gravity = 500f;
        private const float RotationSpeed = 8f;
        private readonly SpriteAnimation _hammerAnimation;
        private float _rotation;

        public override float Rotation
        {
            get
            {
                return _rotation;
            }
        }

        public override int Width
        {
            get
            {
                return HammerWidth;
            }
        }

        public override int Height
        {
            get
            {
                return HammerHeight;
            }
        }

        public override ProjectileType Type
        {
            get
            {
                return ProjectileType.Hammer;
            }
        }

        public override bool RotateAroundCenter
        {
            get
            {
                return true;
            }
        }

        public Hammer(ISprite sprite, Vector2 position, Vector2 velocity, bool isEnemyProjectile)
            : base(sprite, position, velocity, isEnemyProjectile)
        {
            _hammerAnimation = ProjectileSpriteFactory.CreateHammerAnimation();

            Sprite.Update(new GameTime(), _hammerAnimation);
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

            _rotation = _rotation + RotationSpeed * elapsedSeconds;

            Sprite.Update(gameTime, _hammerAnimation);
        }
    }
}
