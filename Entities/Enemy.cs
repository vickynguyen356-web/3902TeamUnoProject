using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Entities
{
    internal abstract class Enemy : IEnemy, IProjectileEmitter
    {
        protected readonly EnemyStateMachine StateMachine;
        protected readonly ISprite Sprite;
        protected readonly IProjectileFactory ProjectileFactory;
        protected Vector2 Velocity;
        private Vector2 _position;
        private readonly List<IProjectile> _projectiles;
        private readonly IReadOnlyList<IProjectile> _readOnlyProjectiles;

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

        public IReadOnlyList<IProjectile> Projectiles
        {
            get
            {
                return _readOnlyProjectiles;
            }
        }

        protected Enemy(ISprite sprite, Vector2 position, IProjectileFactory projectileFactory)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            if (projectileFactory == null)
            {
                throw new ArgumentNullException(nameof(projectileFactory));
            }

            Sprite = sprite;
            Position = position;
            ProjectileFactory = projectileFactory;

            StateMachine = new EnemyStateMachine();

            _projectiles = new List<IProjectile>();
            _readOnlyProjectiles = _projectiles.AsReadOnly();
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

        public bool IsDead
        {
            get
            {
                return StateMachine.IsDead;
            }
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects facingDirection;

            if (StateMachine.IsFlipped)
            {
                facingDirection = SpriteEffects.FlipHorizontally;
            }
            else
            {
                facingDirection = SpriteEffects.None;
            }

            Sprite.Draw(
                spriteBatch,
                Bounds,
                facingDirection);
        }

        public virtual void TakeDamage()
        {
            StateMachine.TakeDamage();
        }

        public virtual void Kill()
        {
            StateMachine.Kill();
        }

        protected void AddProjectile(IProjectile projectile)
        {
            if (projectile == null)
            {
                throw new ArgumentNullException(nameof(projectile));
            }

            _projectiles.Add(projectile);
        }

        public void ClearProjectiles()
        {
            _projectiles.Clear();
        }
    }
}
