using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    internal class PlayerStateMachine
    {
        private const float MinimumRunningSpeed = 15f;
        private const float FireballPoseDurationSeconds = 0.12f;

        private readonly PlayerForm _startingForm;
        private float _throwTimeRemaining;
        private EntityAnimationState _animationState = EntityAnimationState.Idle;
        private PlayerForm _form;
        private bool _isCrouching;
        private bool _isDead;

        public EntityAnimationState AnimationState
        {
            get
            {
                return _animationState;
            }
            private set
            {
                _animationState = value;
            }
        }

        public PlayerForm Form
        {
            get
            {
                return _form;
            }
            private set
            {
                _form = value;
            }
        }

        public bool IsSmall
        {
            get
            {
                return Form == PlayerForm.Small;
            }
        }

        public bool IsCrouching
        {
            get
            {
                return _isCrouching;
            }
            private set
            {
                _isCrouching = value;
            }
        }

        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            private set
            {
                _isDead = value;
            }
        }

        public bool IsThrowing
        {
            get
            {
                return _throwTimeRemaining > 0;
            }
        }

        public PlayerStateMachine(PlayerForm startingForm = PlayerForm.Super)
        {
            _startingForm = startingForm;
            _form = startingForm;
        }

        public void UpdateThrowTimer(float elapsedSeconds)
        {
            _throwTimeRemaining = Math.Max(0, _throwTimeRemaining - elapsedSeconds);
        }

        public bool TryThrowFireball()
        {
            if (Form != PlayerForm.Fire || IsDead || IsCrouching || IsThrowing)
            {
                return false;
            }

            _throwTimeRemaining = FireballPoseDurationSeconds;
            return true;
        }

        public void Update(bool isGrounded, Vector2 velocity)
        {
            if (IsDead)
            {
                AnimationState = EntityAnimationState.Dead;
            }
            else if (IsCrouching)
            {
                AnimationState = EntityAnimationState.Crouch;
            }
            else if (IsThrowing)
            {
                AnimationState = EntityAnimationState.ThrowFireball;
            }
            else if (!isGrounded)
            {
                if (velocity.Y < 0)
                {
                    AnimationState = EntityAnimationState.Jump;
                }
                else
                {
                    AnimationState = EntityAnimationState.Fall;
                }
            }
            else if (Math.Abs(velocity.X) > MinimumRunningSpeed)
            {
                AnimationState = EntityAnimationState.Run;
            }
            else
            {
                AnimationState = EntityAnimationState.Idle;
            }
        }

        public void Reset()
        {
            AnimationState = EntityAnimationState.Idle;
            Form = _startingForm;
            IsCrouching = false;
            IsDead = false;
            _throwTimeRemaining = 0;
        }

        public void TakeDamage()
        {
            if (IsDead)
            {
                return;
            }

            _throwTimeRemaining = 0;

            if (Form == PlayerForm.Small)
            {
                IsDead = true;
                AnimationState = EntityAnimationState.Dead;
                return;
            }

            if (Form == PlayerForm.Fire)
            {
                Form = PlayerForm.Super;
            }
            else
            {
                Form = PlayerForm.Small;
            }

            IsCrouching = false;
        }

        public void SetCrouching(bool crouching)
        {
            IsCrouching = crouching && !IsSmall && !IsDead;
            if (IsCrouching)
            {
                _throwTimeRemaining = 0;
            }
        }
    }
}
