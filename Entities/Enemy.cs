using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public abstract class Enemy : IEnemy, IProjectileEmitter
    {
        protected readonly EnemyStateMachine StateMachine;
        protected readonly ISprite Sprite;
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

        protected Enemy(ISprite sprite, Vector2 position)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            Sprite = sprite;
            Position = position;

            StateMachine = new EnemyStateMachine();
            _projectiles = new List<IProjectile>();
            _readOnlyProjectiles = _projectiles.AsReadOnly();
        }

        public IReadOnlyList<IProjectile> Projectiles
        {
            get
            {
                return _readOnlyProjectiles;
            }
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

        protected void UpdateMovementAnimation(
            GameTime gameTime,
            IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> animations)
        {
            StateMachine.Update(Velocity);

            SpriteAnimation animation;
            if (!animations.TryGetValue(StateMachine.AnimationState, out animation))
            {
                animation = animations[EntityAnimationState.Idle];
            }

            Sprite.Update(gameTime, animation);
        }

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

        protected void AddProjectile(IProjectile projectile)
        {
            if (projectile == null)
            {
                throw new ArgumentNullException(nameof(projectile));
            }

            projectile.IsEnemyProjectile = true;
            _projectiles.Add(projectile);
        }

        public void ClearProjectiles()
        {
            _projectiles.Clear();
        }
    }
}
