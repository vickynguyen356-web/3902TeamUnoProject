using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Player
{
    internal class PlayerStateMachine
    {
        public const float FormTransitionDurationSeconds = 0.7f;
        private const float MinimumRunningSpeed = 15f;
        private const float FireballPoseDurationSeconds = 0.12f;

        private readonly PlayerForm _startingForm;
        private float _throwTimeRemaining;
        private float _transitionTimeRemaining;
        private PlayerForm _transitionStartForm;
        private PlayerForm _transitionTargetForm;
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

        public bool IsChangingForm
        {
            get
            {
                return _transitionTimeRemaining > 0;
            }
        }

        public PlayerForm TransitionStartForm
        {
            get
            {
                return _transitionStartForm;
            }
        }

        public PlayerForm TransitionTargetForm
        {
            get
            {
                return _transitionTargetForm;
            }
        }

        public PlayerStateMachine(PlayerForm startingForm = PlayerForm.Super)
        {
            _startingForm = startingForm;
            _form = startingForm;
            ResetFormTransition();
        }

        public void UpdateThrowTimer(float elapsedSeconds)
        {
            _throwTimeRemaining = Math.Max(0, _throwTimeRemaining - elapsedSeconds);
        }

        public bool BeginFormTransition(PlayerForm targetForm)
        {
            if (!Enum.IsDefined(targetForm))
            {
                throw new ArgumentOutOfRangeException(nameof(targetForm));
            }

            if (targetForm == Form || IsDead || IsChangingForm)
            {
                return false;
            }

            _transitionStartForm = Form;
            _transitionTargetForm = targetForm;
            _transitionTimeRemaining = FormTransitionDurationSeconds;
            IsCrouching = false;
            _throwTimeRemaining = 0;
            AnimationState = EntityAnimationState.Transform;
            return true;
        }

        public void UpdateFormTransition(float elapsedSeconds)
        {
            if (!IsChangingForm || elapsedSeconds <= 0)
            {
                return;
            }

            _transitionTimeRemaining = Math.Max(0, _transitionTimeRemaining - elapsedSeconds);
            if (!IsChangingForm)
            {
                Form = TransitionTargetForm;
                AnimationState = EntityAnimationState.Idle;
            }
        }

        public bool TryThrowFireball()
        {
            if (Form != PlayerForm.Fire || IsDead || IsCrouching || IsThrowing || IsChangingForm)
            {
                return false;
            }

            _throwTimeRemaining = FireballPoseDurationSeconds;
            return true;
        }

        public void UpdateAnimationState(bool isGrounded, Vector2 velocity)
        {
            if (IsDead)
            {
                AnimationState = EntityAnimationState.Dead;
            }
            else if (IsChangingForm)
            {
                AnimationState = EntityAnimationState.Transform;
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
            ResetFormTransition();
        }

        private void ResetFormTransition()
        {
            _transitionTimeRemaining = 0;
            _transitionStartForm = Form;
            _transitionTargetForm = Form;
        }

        public void TakeDamage()
        {
            if (IsDead || IsChangingForm)
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

            BeginFormTransition(PlayerForm.Small);
        }

        public void SetCrouching(bool crouching)
        {
            if (IsChangingForm)
            {
                return;
            }

            IsCrouching = crouching && !IsSmall && !IsDead;
            if (IsCrouching)
            {
                _throwTimeRemaining = 0;
            }
        }
    }
}
