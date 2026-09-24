using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class MarioPlayer : IPlayer
    {
        public const int StandingHeight = 72;
        private const int BodyWidth = 42;
        private const int CrouchingHeight = 44;
        private const float HorizontalAcceleration = 1400f;
        private const float MaximumHorizontalSpeed = 260f;
        private const float HorizontalFriction = 1800f;
        private const float Gravity = 1500f;
        private const float JumpVelocity = -700f;

        private readonly ISprite _sprite;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _superAnimations;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _smallAnimations;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _fireAnimations;
        private readonly PlayerStateMachine _stateMachine;
        private readonly Vector2 _startingPosition;
        private Vector2 _velocity;
        private float _movementDirection;
        private bool _jumpRequested;
        private bool _fireballRequested;
        public event Action FireballRequested;

        public Vector2 Position { get; private set; }
        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
        }
        public bool IsGrounded { get; private set; } = true;
        public bool IsSmall
        {
            get
            {
                return _stateMachine.IsSmall;
            }
        }
        public PlayerForm Form
        {
            get
            {
                return _stateMachine.Form;
            }
        }
        public bool IsCrouching
        {
            get
            {
                return _stateMachine.IsCrouching;
            }
        }
        public bool IsDead
        {
            get
            {
                return _stateMachine.IsDead;
            }
        }
        public EntityAnimationState AnimationState
        {
            get
            {
                return _stateMachine.AnimationState;
            }
        }
        public SpriteEffects FacingDirection { get; private set; } = SpriteEffects.None;
        public Rectangle Bounds
        {
            get
            {
                int bodyHeight = StandingHeight;
                if (IsCrouching)
                {
                    bodyHeight = CrouchingHeight;
                }

                return new Rectangle((int)Position.X, (int)Position.Y, BodyWidth, bodyHeight);
            }
        }

        public MarioPlayer(ISprite sprite, Vector2 position, PlayerForm startingForm = PlayerForm.Super)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            _sprite = sprite;
            _superAnimations = MarioSpriteFactory.CreateSuperAnimations();
            _smallAnimations = MarioSpriteFactory.CreateSmallAnimations();
            _fireAnimations = MarioSpriteFactory.CreateFireAnimations();
            _stateMachine = new PlayerStateMachine(startingForm);
            _startingPosition = position;
            Position = position;
            UpdateAnimation(new GameTime());
        }

        
        public void Move(float movementDirection)
        {
            _movementDirection = MathHelper.Clamp(movementDirection, -1, 1);
        }

        public void Jump()
        {
            _jumpRequested = true;
        }

        public void ThrowFireball()
        {
            _fireballRequested = true;
        }

        internal void UpdateVelocity(float elapsedSeconds)
        {
            _stateMachine.UpdateThrowTimer(elapsedSeconds);
            if (!IsDead)
            {
                UpdateHorizontalVelocity(elapsedSeconds);

                if (_jumpRequested && IsGrounded && !IsCrouching)
                {
                    _velocity.Y = JumpVelocity;
                    IsGrounded = false;
                }
            }

            // Set the facing direction before starting a throw
            if (_fireballRequested && _stateMachine.TryThrowFireball())
            {
                if (FireballRequested != null)
                {
                    FireballRequested();
                }
            }

            _velocity.Y += Gravity * elapsedSeconds;

            // Clear the input 
            _movementDirection = 0;
            _jumpRequested = false;
            _fireballRequested = false;
        }

        private void UpdateHorizontalVelocity(float elapsedSeconds)
        {
            if (_movementDirection != 0)
            {
                // FlipHorizontally selects the right-facing picture from the sheet.
                if (_movementDirection > 0)
                {
                    FacingDirection = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    FacingDirection = SpriteEffects.None;
                }
            }

            if (_movementDirection != 0 && !IsCrouching)
            {
                float acceleratedVelocityX = _velocity.X
                    + _movementDirection * HorizontalAcceleration * elapsedSeconds;
                _velocity.X = MathHelper.Clamp(
                    acceleratedVelocityX, -MaximumHorizontalSpeed, MaximumHorizontalSpeed);
                return;
            }

            // Slow down when movement is released or Mario crouches
            float speedReduction = HorizontalFriction * elapsedSeconds;
            if (_velocity.X > 0)
            {
                _velocity.X -= speedReduction;
                if (_velocity.X < 0)
                {
                    _velocity.X = 0;
                }
            }
            else if (_velocity.X < 0)
            {
                _velocity.X += speedReduction;
                if (_velocity.X > 0)
                {
                    _velocity.X = 0;
                }
            }
        }

        internal void ApplyMotion(Vector2 position, Vector2 velocity, bool isGrounded)
        {
            Position = position;
            _velocity = velocity;
            IsGrounded = isGrounded;
        }

        internal void UpdateAnimation(GameTime gameTime)
        {
            _stateMachine.Update(IsGrounded, _velocity);
            _sprite.Update(gameTime, GetAnimation());
        }

        private SpriteAnimation GetAnimation()
        {
            IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> formAnimations;
            switch (Form)
            {
                case PlayerForm.Small:
                    formAnimations = _smallAnimations;
                    break;
                case PlayerForm.Fire:
                    formAnimations = _fireAnimations;
                    break;
                default:
                    formAnimations = _superAnimations;
                    break;
            }

            if (formAnimations.TryGetValue(AnimationState, out SpriteAnimation animation))
            {
                return animation;
            }

            return formAnimations[EntityAnimationState.Idle];
        }

        public void Reset()
        {
            Position = _startingPosition;
            _velocity = Vector2.Zero;
            _movementDirection = 0;
            _jumpRequested = false;
            _fireballRequested = false;
            IsGrounded = true;
            FacingDirection = SpriteEffects.None;
            _stateMachine.Reset();
            _sprite.Reset();
            UpdateAnimation(new GameTime());
        }

        public void TakeDamage()
        {
            _stateMachine.TakeDamage();
        }

        public void SetCrouching(bool crouching)
        {
            bool wasCrouching = IsCrouching;
            float feetPositionY = Position.Y + Bounds.Height;
            _stateMachine.SetCrouching(crouching);
            if (wasCrouching != IsCrouching)
            {
                // Move the top of the body so the feet stay in place
                Position = new Vector2(Position.X, feetPositionY - Bounds.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Bounds, FacingDirection);
        }
    }
}
