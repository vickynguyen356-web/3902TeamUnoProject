using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Projectiles
{
    public abstract class Projectile : IProjectile
    {
        protected readonly ISprite Sprite;
        protected Vector2 Velocity;
        private Vector2 _position;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            protected set
            {
                _position = value;
            }
        }

        public abstract int Width
        {
            get;
        }

        public abstract int Height
        {
            get;
        }

        public bool IsDead
        {
            get;
            protected set;
        }

        public bool IsEnemyProjectile
        {
            get;
            set;
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Width,
                    Height);
            }
        }

        public Vector2 CurrentVelocity
        {
            get
            {
                return Velocity;
            }
        }

        public abstract ProjectileType Type
        {
            get;
        }

        public abstract void Update(GameTime gameTime);

        protected Projectile(ISprite sprite, Vector2 position, Vector2 velocity)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            Sprite = sprite;
            Position = position;
            Velocity = velocity;
            IsDead = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Bounds, SpriteEffects.None, Rotation, RotateAroundCenter);
        }

        public void Kill()
        {
            IsDead = true;
        }

        public virtual float Rotation
        {
            get
            {
                return 0f;
            }
        }

        public virtual bool RotateAroundCenter
        {
            get
            {
                return false;
            }
        }

        public abstract void Reset();
    }
}
