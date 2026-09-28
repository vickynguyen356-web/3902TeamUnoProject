using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
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
        private Vector2 _position;
        private Vector2 _velocity;
        private bool _isGrounded = true;
        private SpriteEffects _facingDirection = SpriteEffects.None;
        private float _movementDirection;
        private bool _jumpRequested;
        private bool _fireballRequested;
        public event Action FireballRequested;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            private set
            {
                _position = value;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
        }

        public bool IsGrounded
        {
            get
            {
                return _isGrounded;
            }
            private set
            {
                _isGrounded = value;
            }
        }

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

        public SpriteEffects FacingDirection
        {
            get
            {
                return _facingDirection;
            }
            private set
            {
                _facingDirection = value;
            }
        }

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
            _position = position;
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

            if (_fireballRequested && _stateMachine.TryThrowFireball())
            {
                if (FireballRequested != null)
                {
                    FireballRequested();
                }
            }

            _velocity.Y = _velocity.Y + Gravity * elapsedSeconds;

            _movementDirection = 0;
            _jumpRequested = false;
            _fireballRequested = false;
        }

        private void UpdateHorizontalVelocity(float elapsedSeconds)
        {
            if (_movementDirection != 0)
            {
                // FlipHorizontally selects the right-facing picture from the sheet
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

            float speedReduction = HorizontalFriction * elapsedSeconds;
            if (_velocity.X > 0)
            {
                _velocity.X = _velocity.X - speedReduction;
                if (_velocity.X < 0)
                {
                    _velocity.X = 0;
                }
            }
            else if (_velocity.X < 0)
            {
                _velocity.X = _velocity.X + speedReduction;
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

            SpriteAnimation animation;
            if (formAnimations.TryGetValue(AnimationState, out animation))
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
            int previousHeight = Bounds.Height;
            _stateMachine.TakeDamage();
            PreserveFeetPosition(previousHeight);
            _fireballRequested = false;

            if (IsDead)
            {
                _velocity.X = 0;
                _movementDirection = 0;
                _jumpRequested = false;
            }
        }

        public void SetCrouching(bool crouching)
        {
            int previousHeight = Bounds.Height;
            _stateMachine.SetCrouching(crouching);
            PreserveFeetPosition(previousHeight);
        }

        private void PreserveFeetPosition(int previousHeight)
        {
            int heightChange = Bounds.Height - previousHeight;
            if (heightChange != 0)
            {
                // Move the top of the body so the feet stay in place
                Position = new Vector2(Position.X, Position.Y - heightChange);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Bounds, FacingDirection);
        }
    }
}
