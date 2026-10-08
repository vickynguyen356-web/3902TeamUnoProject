using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal abstract class Projectile : IProjectile
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

        protected Projectile(ISprite sprite, Vector2 position, Vector2 velocity, bool isEnemyProjectile)
        {
            ArgumentNullException.ThrowIfNull(sprite);

            Sprite = sprite;
            Position = position;
            Velocity = velocity;
            IsEnemyProjectile = isEnemyProjectile;
            IsDead = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            FacingDirection facingDirection = FacingDirection.Left;
            if (Velocity.X > 0f)
            {
                facingDirection = FacingDirection.Right;
            }

            Sprite.Draw(spriteBatch, Bounds, facingDirection, Rotation, RotateAroundCenter);
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
    }
}
