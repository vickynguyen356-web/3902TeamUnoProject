using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class HammerBro : Enemy
    {
        public const int HammerBroWidth = 32;
        public const int HammerBroHeight = 48;
        private const float RunSpeed = 45f;
        private const float PatrolDistance = 100f;
        private const float HammerSpeed = 150f;
        private const float HammerVerticalSpeed = -180f;
        private const float ThrowDuration = 0.56f;
        private const float ThrowReleaseTime = 0.28f;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _hammerBroAnimations;
        private readonly float _leftBound;
        private readonly float _rightBound;
        private FacingDirection _patrolDirection = FacingDirection.Left;
        private bool _isThrowing;
        private float _throwTimer;
        private bool _hammerReleased;

        public override int Width
        {
            get
            {
                return HammerBroWidth;
            }
        }

        public override int Height
        {
            get
            {
                return HammerBroHeight;
            }
        }

        public HammerBro(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _hammerBroAnimations = HammerBroSpriteFactory.CreateHammerBroAnimations();
            _leftBound = position.X - PatrolDistance;
            _rightBound = position.X + PatrolDistance;
            Velocity.X = GetPatrolVelocity();

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsDead)
            {
                Velocity.X = 0f;
                UpdateAnimation(gameTime);
                return;
            }

            if (_isThrowing)
            {
                // Stop moving while throwing.
                Velocity.X = 0f;
                _throwTimer = _throwTimer + elapsedSeconds;

                // Release the hammer halfway through the throw animation.
                if (_throwTimer >= ThrowReleaseTime && !_hammerReleased)
                {
                    ThrowHammer();
                    _hammerReleased = true;
                }

                // Finish the throw and start walking in the opposite direction.
                if (_throwTimer >= ThrowDuration)
                {
                    _throwTimer = 0f;
                    _isThrowing = false;
                    _hammerReleased = false;

                    if (_patrolDirection == FacingDirection.Left)
                    {
                        _patrolDirection = FacingDirection.Right;
                    }
                    else
                    {
                        _patrolDirection = FacingDirection.Left;
                    }

                    Velocity.X = GetPatrolVelocity();
                }
            }
            else
            {
                // Walk in the current direction.
                Velocity.X = GetPatrolVelocity();
                Position = Position + Velocity * elapsedSeconds;

                // Reached the left boundary.
                if (Position.X <= _leftBound)
                {
                    Position = new Vector2(_leftBound, Position.Y);
                    StartThrowing();
                }
                else if (Position.X >= _rightBound)
                {
                    // Reached the right boundary.
                    Position = new Vector2(_rightBound, Position.Y);
                    StartThrowing();
                }
            }

            UpdateAnimation(gameTime);
        }

        private void StartThrowing()
        {
            _isThrowing = true;
            _throwTimer = 0f;
            _hammerReleased = false;
        }

        private float GetPatrolVelocity()
        {
            if (_patrolDirection == FacingDirection.Left)
            {
                return -RunSpeed;
            }

            return RunSpeed;
        }

        private void ThrowHammer()
        {
            float direction = 1f;
            if (StateMachine.FacingDirection == FacingDirection.Left)
            {
                direction = -1f;
            }

            float hammerX;
            if (direction < 0f)
            {
                hammerX = Position.X - Hammer.HammerWidth;
            }
            else
            {
                hammerX = Position.X + Width;
            }

            Vector2 hammerPosition = new Vector2(hammerX, Position.Y);
            Vector2 hammerVelocity = new Vector2(direction * HammerSpeed, HammerVerticalSpeed);
            QueueProjectile(
                ProjectileType.Hammer,
                hammerPosition,
                hammerVelocity);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);
            EntityAnimationState animationState;

            if (_isThrowing)
            {
                animationState = EntityAnimationState.ThrowHammer;
            }
            else
            {
                animationState = StateMachine.AnimationState;
            }

            SpriteAnimation animation;
            if (_hammerBroAnimations.TryGetValue(animationState, out animation))
            {
                Sprite.Update(gameTime, animation);
            }
            else
            {
                Sprite.Update(gameTime, _hammerBroAnimations[EntityAnimationState.Idle]);
            }
        }
    }
}
