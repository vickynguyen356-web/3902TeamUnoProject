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
            IReadOnlyDictionary<ProjectileType, SpriteAnimation>
                animations = ProjectileSpriteFactory.CreateProjectileAnimations();

            _fireballAnimation = animations[ProjectileType.Fireball];

            Sprite.Update(new GameTime(),
                _fireballAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                Position += Velocity * elapsedSeconds;
            }

            Sprite.Update(gameTime,
                ProjectileSpriteFactory.CreateProjectileAnimations()[
                    ProjectileType.Fireball
                    ]);
        }

        public override void Reset()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;

            Sprite.Reset();

            Sprite.Update(new GameTime(), _fireballAnimation);
        }

        public void OnCollision()
        {
            Kill();
        }
    }
}
