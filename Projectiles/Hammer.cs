using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Projectiles
{
    public class Hammer : Projectile
    {
        public const int HammerWidth = 64;
        public const int HammerHeight = 64;
        private const float Gravity = 500f;
        private const float RotationSpeed = 8f;
        private float _rotation;
        public override float Rotation
        {
            get
            {
                return _rotation;
            }
        }

        private readonly ISprite _sprite;
        private readonly SpriteAnimation _hammerAnimation;
       

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

        public Hammer(ISprite sprite, Vector2 position, Vector2 velocity)
            : base(sprite, position, velocity)
        {
            Position = position;
            Velocity = velocity;
            IReadOnlyDictionary<ProjectileType, SpriteAnimation>
                animations = ProjectileSpriteFactory.CreateProjectileAnimations();
            _hammerAnimation = animations[ProjectileType.Hammer];

            IsDead = false;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Velocity.Y = Velocity.Y + Gravity * elapsedSeconds;
            Position += Velocity * elapsedSeconds;

            _rotation = _rotation + RotationSpeed * elapsedSeconds; 

            Sprite.Update(gameTime, _hammerAnimation);
        }

        public override void Reset()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            IsDead = false;
            _rotation = 0f;

            Sprite.Reset();

            Sprite.Update(new GameTime(), _hammerAnimation);
        }
    }
}
