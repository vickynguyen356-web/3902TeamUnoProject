using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class Bowser : Enemy
    {
        public const int BowserWidth = 64;
        public const int BowserHeight = 64;
        private const float WalkSpeed = 45f;
        private const float PatrolDistance = 150f;
        private const float FireSpeed = 150f;
        private const float FireInterval = 2.5f;
        private const float SpitFireDuration = 0.6f;
        private const float FireReleaseTime = 0.28f;
        private const float MouthHeight = 24f;

        private readonly float _leftBound;
        private readonly float _rightBound;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _bowserAnimations;
        private bool _isSpittingFire;
        private bool _fireReleased;
        private float _spitFireTimer;
        private float _fireTimer;

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

        public Bowser(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _bowserAnimations = BowserSpriteFactory.CreateBowserAnimations();
            _leftBound = position.X - PatrolDistance;
            _rightBound = position.X + PatrolDistance;
            Velocity.X = -WalkSpeed;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsDead)
            {
                Velocity.X = 0f;
                _isSpittingFire = false;
                UpdateAnimation(gameTime);
                return;
            }

            _fireTimer = _fireTimer + elapsedSeconds;

            if (_isSpittingFire)
            {
                UpdateSpitFire(elapsedSeconds);
            }
            else
            {
                UpdatePatrol(elapsedSeconds);
                if (_fireTimer >= FireInterval)
                {
                    SpitFire();
                }
            }

            UpdateAnimation(gameTime);
        }

        public void SpitFire()
        {
            if (IsDead || _isSpittingFire)
            {
                return;
            }

            _isSpittingFire = true;
            _fireReleased = false;
            _spitFireTimer = 0f;
            _fireTimer = 0f;
            StateMachine.Update(Velocity);
            Velocity.X = 0f;
        }

        private void UpdatePatrol(float elapsedSeconds)
        {
            Position = Position + new Vector2(Velocity.X * elapsedSeconds, 0f);

            if (Position.X <= _leftBound)
            {
                Position = new Vector2(_leftBound, Position.Y);
                Velocity.X = WalkSpeed;
            }
            else if (Position.X >= _rightBound)
            {
                Position = new Vector2(_rightBound, Position.Y);
                Velocity.X = -WalkSpeed;
            }
        }

        private void UpdateSpitFire(float elapsedSeconds)
        {
            _spitFireTimer = _spitFireTimer + elapsedSeconds;

            if (_spitFireTimer >= FireReleaseTime && !_fireReleased)
            {
                ReleaseFire();
                _fireReleased = true;
            }

            if (_spitFireTimer >= SpitFireDuration)
            {
                _isSpittingFire = false;
                if (StateMachine.FacingDirection == FacingDirection.Left)
                {
                    Velocity.X = -WalkSpeed;
                }
                else
                {
                    Velocity.X = WalkSpeed;
                }
            }
        }

        private void ReleaseFire()
        {
            float direction;
            if (StateMachine.FacingDirection == FacingDirection.Left)
            {
                direction = -1f;
            }
            else
            {
                direction = 1f;
            }

            float fireX;
            if (direction < 0f)
            {
                fireX = Position.X - BowserFire.FireWidth;
            }
            else
            {
                fireX = Position.X + Width;
            }

            float fireY = Position.Y + MouthHeight - BowserFire.FireHeight / 2f;
            Vector2 firePosition = new Vector2(fireX, fireY);
            Vector2 fireVelocity = new Vector2(direction * FireSpeed, 0f);
            QueueProjectile(
                ProjectileType.BowserFire,
                firePosition,
                fireVelocity);
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
