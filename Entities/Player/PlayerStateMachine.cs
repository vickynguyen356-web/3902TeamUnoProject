using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Player
{
    internal class PlayerStateMachine
    {
        public const float FormTransitionDurationSeconds = 0.7f;
        private const float StarDuration = 10f;
        private const float FireballPoseDurationSeconds = 0.12f;

        private readonly PlayerForm _startingForm;
        private float _starTimeRemaining;
        private float _throwTimeRemaining;
        private float _transitionTimeRemaining;
        public EntityAnimationState AnimationState { get; private set; } = EntityAnimationState.Idle;
        public PlayerForm Form { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsDead { get; private set; }
        public PlayerForm TransitionStartForm { get; private set; }
        public PlayerForm TransitionTargetForm { get; private set; }

        public bool IsSmall => Form == PlayerForm.Small;
        public bool IsStarActive => _starTimeRemaining > 0f;
        public bool IsThrowing => _throwTimeRemaining > 0;
        public bool IsChangingForm => _transitionTimeRemaining > 0;

        public PlayerStateMachine(PlayerForm startingForm = PlayerForm.Super)
        {
            _startingForm = startingForm;
            Form = startingForm;
            ResetFormTransition();
        }

        public void ActivateStar()
        {
            if (!IsDead)
            {
                _starTimeRemaining = StarDuration;
            }
        }

        public void UpdateStar(float elapsedSeconds)
        {
            _starTimeRemaining = Math.Max(0f, _starTimeRemaining - elapsedSeconds);
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

            TransitionStartForm = Form;
            TransitionTargetForm = targetForm;
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

        public void UpdateAnimationState(
            bool isGrounded,
            Vector2 velocity,
            bool isClimbing = false,
            bool isSwimming = false,
            bool isDrifting = false)
        {
            AnimationState = PlayerAnimationSelector.Select(this, isGrounded, velocity, isClimbing, isSwimming, isDrifting);
        }

        public void Reset()
        {
            Reset(_startingForm);
        }

        public void Reset(PlayerForm form)
        {
            _starTimeRemaining = 0f;
            AnimationState = EntityAnimationState.Idle;
            Form = form;
            IsCrouching = false;
            IsDead = false;
            _throwTimeRemaining = 0;
            ResetFormTransition();
        }

        private void ResetFormTransition()
        {
            _transitionTimeRemaining = 0;
            TransitionStartForm = Form;
            TransitionTargetForm = Form;
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

        public void Die()
        {
            IsDead = true;
            IsCrouching = false;
            _throwTimeRemaining = 0;
            _transitionTimeRemaining = 0;
            AnimationState = EntityAnimationState.Dead;
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
