using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Player
{
    internal class MarioPlayer : IPlayer, IProjectileEmitter, ICollidable
    {
        public const int StandingHeight = 64;
        private const float StarColorDuration = 0.12f;
        private const float DamageInvulnerabilityDurationSeconds = 1.5f;
        private static readonly Color[] StarColors =
        {
            Color.White, Color.Red, Color.Green, Color.Blue
        };
        private float _starColorTime;
        private float _damageInvulnerabilityTimeRemaining;

        private const int SmallHeight = 32;
        private const int BodyWidth = 32;
        private const int CrouchingHeight = 44;
        private const float HorizontalAcceleration = 1400f;
        private const float MaximumHorizontalSpeed = 260f;
        private const float HorizontalFriction = 1800f;
        private const float Gravity = 1500f;
        private const float JumpVelocity = -700f;
        private const float FlagpoleClimbSpeed = 180f;
        private const float VineClimbSpeed = 180f;
        private const float FireballSpeed = 200f;
        private const float FireballVerticalVelocity = -100f;

        private readonly ISprite _sprite;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _superAnimations;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _smallAnimations;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _fireAnimations;
        private readonly PlayerStateMachine _stateMachine;
        private readonly List<IProjectile> _projectiles = new List<IProjectile>();
        private readonly IReadOnlyList<IProjectile> _readOnlyProjectiles;
        private SpriteAnimation _formTransitionAnimation;
        private readonly Vector2 _startingPosition;
        private Vector2 _position;
        private Vector2 _velocity;
        private bool _isGrounded;
        private FacingDirection _facingDirection = FacingDirection.Left;
        private float _movementDirection;
        private bool _jumpRequested;
        private bool _fireballRequested;
        private bool _pipeEntryRequested;
        private bool _isSlidingOnFlagpole;
        private bool _isClimbingFlagpole;
        private bool _isClimbingVine;
        private float _climbDirection;
        private float _flagpoleTopPositionY;
        private float _flagpoleBottomPositionY;

        public IReadOnlyList<IProjectile> Projectiles
        {
            get
            {
                return _readOnlyProjectiles;
            }
        }

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

        public bool IsChangingForm
        {
            get
            {
                return _stateMachine.IsChangingForm;
            }
        }

        public bool IsSlidingOnFlagpole
        {
            get
            {
                return _isSlidingOnFlagpole;
            }
        }

        internal bool IsClimbingVine
        {
            get
            {
                return _isClimbingVine;
            }
        }

        public EntityAnimationState AnimationState
        {
            get
            {
                return _stateMachine.AnimationState;
            }
        }

        public FacingDirection FacingDirection
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
                if (IsSmall)
                {
                    bodyHeight = SmallHeight;
                }
                else if (IsCrouching)
                {
                    bodyHeight = CrouchingHeight;
                }

                return new Rectangle((int)Position.X, (int)Position.Y, BodyWidth, bodyHeight);
            }
        }

        public MarioPlayer(ISprite sprite, Vector2 position, PlayerForm startingForm = PlayerForm.Super)
        {
            ArgumentNullException.ThrowIfNull(sprite);

            _sprite = sprite;
            _superAnimations = MarioSpriteFactory.CreateSuperAnimations();
            _smallAnimations = MarioSpriteFactory.CreateSmallAnimations();
            _fireAnimations = MarioSpriteFactory.CreateFireAnimations();
            _stateMachine = new PlayerStateMachine(startingForm);
            _readOnlyProjectiles = _projectiles.AsReadOnly();
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
            if (_isClimbingVine)
            {
                StopVineClimb();
            }

            _jumpRequested = true;
        }

        public void Climb(float direction)
        {
            _climbDirection = MathHelper.Clamp(direction, -1, 1);
        }

        public void ThrowFireball()
        {
            _fireballRequested = true;
        }

        public void RequestPipeEntry()
        {
            _pipeEntryRequested = true;
        }

        internal bool IsPipeEntryRequested
        {
            get
            {
                return _pipeEntryRequested;
            }
        }

        internal void ClearPipeEntryRequest()
        {
            _pipeEntryRequested = false;
        }

        public void ClearProjectiles()
        {
            _projectiles.Clear();
        }

        internal void AddProjectile(IProjectile projectile)
        {
            ArgumentNullException.ThrowIfNull(projectile);
            _projectiles.Add(projectile);
        }

        public bool IsStarActive
        {
            get { return _stateMachine.IsStarActive; }
        }

        public void ActivateStar()
        {
            _stateMachine.ActivateStar();
            _starColorTime = 0f;
        }

        public void BeginFormTransition(PlayerForm targetForm)
        {
            int previousHeight = Bounds.Height;
            if (_stateMachine.BeginFormTransition(targetForm))
            {
                StopVineClimb();
                PreserveFeetPosition(previousHeight);
                StartFormTransitionAnimation();
            }
        }

        internal void UpdateState(float elapsedSeconds)
        {
            int previousHeight = Bounds.Height;
            bool wasChangingForm = IsChangingForm;
            _damageInvulnerabilityTimeRemaining = Math.Max(
                0f, _damageInvulnerabilityTimeRemaining - elapsedSeconds);
            if (wasChangingForm)
            {
                ClearInputRequests();
            }

            _stateMachine.UpdateStar(elapsedSeconds);
            if (IsStarActive && !IsDead)
            {
                _starColorTime = _starColorTime + elapsedSeconds;
            }
            else
            {
                _starColorTime = 0f;
            }
            _stateMachine.UpdateThrowTimer(elapsedSeconds);
            _stateMachine.UpdateFormTransition(elapsedSeconds);
            if (wasChangingForm && !IsChangingForm)
            {
                PreserveFeetPosition(previousHeight);
                _formTransitionAnimation = null;
            }
        }

        internal void UpdateVelocity(float elapsedSeconds)
        {
            if (IsChangingForm)
            {
                ClearInputRequests();
                return;
            }

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
                EmitFireball();
            }

            if (IsSlidingOnFlagpole)
            {
                float verticalDirection = _isClimbingFlagpole ? -1f : 1f;
                _velocity = new Vector2(0, verticalDirection * FlagpoleClimbSpeed);
                ClearInputRequests();
                return;
            }

            if (_isClimbingVine && _movementDirection == 0)
            {
                _velocity = new Vector2(0, _climbDirection * VineClimbSpeed);
                ClearInputRequests();
                return;
            }
            else if (_isClimbingVine)
            {
                StopVineClimb();
            }

            _velocity.Y = _velocity.Y + Gravity * elapsedSeconds;

            ClearInputRequests();
        }

        private void EmitFireball()
        {
            float direction = -1f; // left
            if (FacingDirection == FacingDirection.Right)
            {
                direction = 1f; // right
            }

            Rectangle marioBounds = Bounds;
            Vector2 position = new Vector2(
                marioBounds.Right,
                marioBounds.Center.Y - Fireball.FireballHeight / 2f);
            if (direction < 0)
            {
                position.X = marioBounds.Left - Fireball.FireballWidth;
            }

            Vector2 velocity = new Vector2(direction * FireballSpeed, FireballVerticalVelocity);
            IProjectile fireball = ProjectileFactory.Create(
                ProjectileType.Fireball, position, velocity, isEnemyProjectile: false);
            _projectiles.Add(fireball);
        }

        private void UpdateHorizontalVelocity(float elapsedSeconds)
        {
            if (_movementDirection != 0)
            {
                if (_movementDirection > 0)
                {
                    FacingDirection = FacingDirection.Right;
                }
                else
                {
                    FacingDirection = FacingDirection.Left;
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
                _velocity.X = Math.Max(0, _velocity.X - speedReduction);
            }
            else if (_velocity.X < 0)
            {
                _velocity.X = Math.Min(0, _velocity.X + speedReduction);
            }
        }

        internal void UpdatePosition(float elapsedSeconds)
        {
            if (IsChangingForm)
            {
                return;
            }

            if (IsSlidingOnFlagpole)
            {
                UpdateFlagpolePosition(elapsedSeconds);
                return;
            }

            Position = Position + _velocity * elapsedSeconds;
            IsGrounded = false;
        }

        internal void ApplyMotion(Vector2 position, Vector2 velocity, bool isGrounded)
        {
            Position = position;
            _velocity = velocity;
            IsGrounded = isGrounded;
        }

        internal void EnterArea(Vector2 spawnFeet)
        {
            StopVineClimb();
            SetCrouching(false);
            Position = new Vector2(spawnFeet.X, spawnFeet.Y - Bounds.Height);
            _velocity = Vector2.Zero;
            IsGrounded = false;
            _climbDirection = 0;
            ClearPipeEntryRequest();
            StopFlagpoleSlide();
            ClearInputRequests();
            ClearProjectiles();
            UpdateAnimation(new GameTime());
        }

        internal void StartFlagpoleSlide(float flagpoleCenterX, float flagpoleTopY, float flagpoleBottomY)
        {
            StopVineClimb();
            _isSlidingOnFlagpole = true;
            _isClimbingFlagpole = true;
            _flagpoleTopPositionY = flagpoleTopY;
            _flagpoleBottomPositionY = flagpoleBottomY - Bounds.Height;
            Position = new Vector2(flagpoleCenterX - Bounds.Width / 2f, Position.Y);
            _velocity = new Vector2(0, -FlagpoleClimbSpeed);
            IsGrounded = false;
            ClearInputRequests();
        }

        internal void TryClimbVine(Rectangle vineBounds)
        {
            if (IsDead || IsChangingForm || (_climbDirection == 0 && !_isClimbingVine))
            {
                return;
            }

            _isClimbingVine = true;
            SetCrouching(false);

            Rectangle playerBounds = Bounds;
            float minimumY = vineBounds.Top;
            float maximumY = vineBounds.Bottom - playerBounds.Height;
            float positionY = MathHelper.Clamp(Position.Y, minimumY, Math.Max(minimumY, maximumY));
            Position = new Vector2(vineBounds.Center.X - playerBounds.Width / 2f, positionY);
            bool reachedBottom = positionY >= maximumY && _climbDirection >= 0;
            bool reachedLimit = (positionY <= minimumY && _climbDirection < 0)
                || (positionY >= maximumY && _climbDirection > 0);
            float verticalVelocity = reachedLimit ? 0 : _climbDirection * VineClimbSpeed;
            _velocity = new Vector2(0, verticalVelocity);
            IsGrounded = reachedBottom;
        }

        private void StopVineClimb()
        {
            _isClimbingVine = false;
            _velocity = new Vector2(_velocity.X, 0);
        }

        private void UpdateFlagpolePosition(float elapsedSeconds)
        {
            float nextPositionY = Position.Y + _velocity.Y * elapsedSeconds;
            if (_isClimbingFlagpole && nextPositionY <= _flagpoleTopPositionY)
            {
                nextPositionY = _flagpoleTopPositionY;
                _isClimbingFlagpole = false;
            }
            else if (!_isClimbingFlagpole && nextPositionY >= _flagpoleBottomPositionY)
            {
                nextPositionY = _flagpoleBottomPositionY;
                _isSlidingOnFlagpole = false;
                IsGrounded = true;
            }

            Position = new Vector2(Position.X, nextPositionY);
            if (!IsSlidingOnFlagpole)
            {
                _velocity = Vector2.Zero;
            }
        }

        internal void StopFlagpoleSlide()
        {
            _isSlidingOnFlagpole = false;
            _isClimbingFlagpole = false;
        }

        internal void DieFromFall()
        {
            if (IsDead)
            {
                return;
            }

            _stateMachine.Die();
            StopVineClimb();
            _velocity = Vector2.Zero;
            ClearInputRequests();
        }

        internal void UpdateAnimation(GameTime gameTime)
        {
            _stateMachine.UpdateAnimationState(IsGrounded, _velocity, IsSlidingOnFlagpole || _isClimbingVine);
            _sprite.Update(gameTime, GetAnimation());
        }

        private SpriteAnimation GetAnimation()
        {
            if (IsChangingForm)
            {
                return _formTransitionAnimation;
            }

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
            _starColorTime = 0f;
            _damageInvulnerabilityTimeRemaining = 0f;
            Position = _startingPosition;
            _velocity = Vector2.Zero;
            ClearInputRequests();
            ClearProjectiles();
            ClearPipeEntryRequest();
            _formTransitionAnimation = null;
            IsGrounded = false;
            _isSlidingOnFlagpole = false;
            _isClimbingFlagpole = false;
            _isClimbingVine = false;
            _climbDirection = 0;
            FacingDirection = FacingDirection.Left;
            _stateMachine.Reset();
            _sprite.Reset();
            UpdateAnimation(new GameTime());
        }

        internal void Respawn()
        {
            Reset();
            _stateMachine.Reset(PlayerForm.Small);
            UpdateAnimation(new GameTime());
        }

        public void TakeDamage()
        {
            if (IsDead
                || IsChangingForm
                || IsStarActive
                || _damageInvulnerabilityTimeRemaining > 0f)
            {
                return;
            }

            StartInvulnerabilityFrames();
            int previousHeight = Bounds.Height;
            _stateMachine.TakeDamage();
            PreserveFeetPosition(previousHeight);
            _fireballRequested = false;

            if (IsChangingForm)
            {
                StartFormTransitionAnimation();
                return;
            }

            if (IsDead)
            {
                StopVineClimb();
                _velocity.X = 0;
                _movementDirection = 0;
                _jumpRequested = false;
            }
        }

        private void StartInvulnerabilityFrames()
        {
            _damageInvulnerabilityTimeRemaining = DamageInvulnerabilityDurationSeconds;
        }

        public void SetCrouching(bool crouching)
        {
            if (crouching && _isClimbingVine)
            {
                return;
            }

            int previousHeight = Bounds.Height;
            _stateMachine.SetCrouching(crouching);
            PreserveFeetPosition(previousHeight);
        }

        private void StartFormTransitionAnimation()
        {
            _formTransitionAnimation = MarioSpriteFactory.CreateFormTransitionAnimation(
                _stateMachine.TransitionStartForm, _stateMachine.TransitionTargetForm);
            ClearInputRequests();
            _sprite.Update(new GameTime(), _formTransitionAnimation);
        }

        internal void ClearControls()
        {
            ClearInputRequests();
            ClearPipeEntryRequest();
            _climbDirection = 0;
        }

        private void ClearInputRequests()
        {
            _movementDirection = 0;
            _jumpRequested = false;
            _fireballRequested = false;
        }

        private void PreserveFeetPosition(int previousHeight)
        {
            int heightChange = Bounds.Height - previousHeight;
            Position = new Vector2(Position.X, Position.Y - heightChange);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color tint = Color.White;
            if (IsStarActive && !IsDead)
            {
                // change colors while the star timer is running
                int colorIndex = (int)(_starColorTime / StarColorDuration) % StarColors.Length;
                tint = StarColors[colorIndex];
            }
            _sprite.Draw(spriteBatch, Bounds, FacingDirection, tint);
        }
    }
}
