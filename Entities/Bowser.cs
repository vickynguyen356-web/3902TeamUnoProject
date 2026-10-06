using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Entities
{
    internal class Bowser : Enemy
    {
        public const int BowserWidth = 64;
        public const int BowserHeight = 64;
        public override int Width
        {
            get
            {
                return BowserWidth;
            }
        }

        public override int Height
        {
            get
            {
                return BowserHeight;
            }
        }

        private const float WalkSpeed = 45f;
        private const float WalkDistance = 150f;
        private const float FireballSpeed = 150f;
        private const float SpitFireDuration = 0.6f;
        private float _distanceTraveled;
        private bool _hasStopped;
        private bool _isSpittingFire;
        private float _spitFireTimer;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _bowserAnimations;
        private readonly IProjectileFactory _projectileFactory;

        public Bowser(ISprite sprite, Vector2 position, IProjectileFactory projectileFactory)
            : base(sprite, position)
        {
            ArgumentNullException.ThrowIfNull(projectileFactory);

            _projectileFactory = projectileFactory;
            _bowserAnimations = BowserSpriteFactory.CreateBowserAnimations();
            _distanceTraveled = 0f;
            Velocity.X = -WalkSpeed;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead && !_hasStopped)
            {
                float movement = Velocity.X * elapsedSeconds;

                Position = Position + new Vector2(movement, 0f);
                _distanceTraveled = _distanceTraveled + Math.Abs(movement);

                if (_distanceTraveled >= WalkDistance)
                {
                    _distanceTraveled = WalkDistance;
                    _hasStopped = true;
                    Velocity = Vector2.Zero;
                }
            }

            if (_isSpittingFire)
            {
                _spitFireTimer = _spitFireTimer + elapsedSeconds;

                if (_spitFireTimer >= SpitFireDuration)
                {
                    _isSpittingFire = false;
                    _spitFireTimer = 0f;
                }
            }

            UpdateAnimation(gameTime);
        }

        public void SpitFire()
        {
            if (IsDead)
            {
                return;
            }

            _isSpittingFire = true;
            _spitFireTimer = 0f;

            float direction;
            if (StateMachine.IsFlipped)
            {
                direction = -1f;
            }
            else
            {
                direction = 1f;
            }

            float fireballX;
            if (direction < 0f)
            {
                fireballX = Position.X - Fireball.FireballWidth;
            }
            else
            {
                fireballX = Position.X + Width;
            }

            float fireballY = Position.Y + Height / 2f - Fireball.FireballHeight / 2f;
            Vector2 fireballPosition = new Vector2(fireballX, fireballY);
            Vector2 fireballVelocity = new Vector2(direction * FireballSpeed, 0f);
            IProjectile fireball = _projectileFactory.Create(
                ProjectileType.Fireball,
                fireballPosition,
                fireballVelocity);

            AddProjectile(fireball);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            if (_isSpittingFire && !IsDead)
            {
                StateMachine.Update(Velocity);
                Sprite.Update(gameTime, _bowserAnimations[EntityAnimationState.SpitFire]);
            }
            else
            {
                UpdateMovementAnimation(gameTime, _bowserAnimations);
            }
        }
    }
}
