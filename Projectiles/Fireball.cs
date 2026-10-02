using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Projectiles
{
    public class Fireball : Projectile
    {
        public const int FireballWidth = 64;
        public const int FireballHeight = 64;
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

        public Fireball(ISprite sprite, Vector2 position, Vector2 velocity)
            : base(sprite, position, velocity)
        {
            Position = position;
            Velocity = velocity;
            IsDead = false;

            IReadOnlyDictionary<ProjectileType, SpriteAnimation>
                animations = ProjectileSpriteFactory.CreateProjectileAnimations();

            _fireballAnimation = animations[ProjectileType.Fireball];
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * elapsedSeconds;

            Sprite.Update(gameTime, _fireballAnimation);
        }

        public override void Reset()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;

            Sprite.Reset();

            Sprite.Update(new GameTime(), _fireballAnimation);
        }
    }
}
