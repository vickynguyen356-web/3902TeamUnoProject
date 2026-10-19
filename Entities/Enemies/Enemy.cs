using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal abstract class Enemy : IEnemy, IProjectileEmitter, ICollidable
    {
        protected readonly EnemyStateMachine StateMachine;
        protected readonly ISprite Sprite;
        protected Vector2 Velocity;
        private IReadOnlyList<Block> _worldBlocks;

        public void SetWorldBlocks(IReadOnlyList<Block> blocks)
        {
            _worldBlocks = blocks;
        }

        protected bool TryWalk(float elapsedSeconds)
        {
            float nextX = Position.X + Velocity.X * elapsedSeconds;
            Rectangle nextBounds = new Rectangle((int)nextX, (int)Position.Y, Width, Height);
            if (!CanWalkTo(nextBounds))
            {
                return false;
            }

            Position = new Vector2(nextX, Position.Y);
            return true;
        }

        private bool CanWalkTo(Rectangle nextBounds)
        {
            if (_worldBlocks == null || Velocity.X == 0f)
            {
                return true;
            }

            int aheadX = nextBounds.Right;
            if (Velocity.X < 0f)
            {
                aheadX = nextBounds.Left - 1;
            }

            // check for ground under the enemy and just ahead of its feet
            Rectangle groundAhead = new Rectangle(aheadX, Bounds.Bottom, 1, 4);
            Rectangle groundBelow = new Rectangle(Bounds.Left, Bounds.Bottom, Width, 4);
            bool hasGroundBelow = false;
            bool hasGroundAhead = false;

            foreach (Block block in _worldBlocks)
            {
                if (block.Type == BlockType.Empty || block.Type == BlockType.Vine
                    || block.Type == BlockType.FlagPole)
                {
                    continue;
                }

                // platforms support walking but do not act as side walls
                if (block.Type != BlockType.Platform && nextBounds.Intersects(block.Bounds))
                {
                    return false;
                }

                if (block.Bounds.Top >= Bounds.Bottom)
                {
                    if (groundBelow.Intersects(block.Bounds))
                    {
                        hasGroundBelow = true;
                    }
                    if (groundAhead.Intersects(block.Bounds))
                    {
                        hasGroundAhead = true;
                    }
                }
            }

            // turn before walking off the surface we are standing on
            if (hasGroundBelow && !hasGroundAhead)
            {
                return false;
            }

            return true;
        }

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
            ArgumentNullException.ThrowIfNull(sprite);

            Sprite = sprite;
            Position = position;
            _projectiles = new List<IProjectile>();
            _readOnlyProjectiles = _projectiles.AsReadOnly();

            StateMachine = new EnemyStateMachine();
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

        public IReadOnlyList<IProjectile> Projectiles
        {
            get
            {
                return _readOnlyProjectiles;
            }
        }

        protected void QueueProjectile(ProjectileType type, Vector2 position, Vector2 velocity)
        {
            IProjectile projectile = ProjectileFactory.Create(type, position, velocity, true);
            _projectiles.Add(projectile);
        }

        public void ClearProjectiles()
        {
            _projectiles.Clear();
        }

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
            Sprite.Draw(
                spriteBatch,
                Bounds,
                StateMachine.FacingDirection);
        }

        public virtual void TakeDamage()
        {
            StateMachine.TakeDamage();
        }

        public virtual void Kill()
        {
            StateMachine.Kill();
        }
    }
}
