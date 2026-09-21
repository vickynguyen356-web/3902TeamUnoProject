using System;
using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class PlayerStateMachine
    {
        private const float MinimumRunningSpeed = 15f;
        private const float FireballPoseDurationSeconds = 0.12f;

        private readonly PlayerForm _startingForm;
        private float _throwTimeRemaining;

        public EntityAnimationState AnimationState { get; private set; } = EntityAnimationState.Idle;
        public PlayerForm Form { get; private set; }
        public bool IsSmall
        {
            get
            {
                return Form == PlayerForm.Small;
            }
        }
        public bool IsCrouching { get; private set; }
        public bool IsDead { get; private set; }
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
            Form = startingForm;
        }

        public void UpdateThrowTimer(float elapsedSeconds)
        {
            _throwTimeRemaining = Math.Max(0, _throwTimeRemaining - elapsedSeconds);
        }

        public bool TryThrowFireball()
        {
            // Only Fire Mario can start a throw.
            if (Form != PlayerForm.Fire || IsDead || IsCrouching || IsThrowing)
            {
                return false;
            }

            _throwTimeRemaining = FireballPoseDurationSeconds;
            return true;
        }

        public void Update(bool isGrounded, Vector2 velocity)
        {
            // Check special poses before walking or jumping.
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
                // A negative Y velocity means Mario is going up.
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
            // Damage and power-up changes go here.
        }

        public void SetCrouching(bool crouching)
        {
            IsCrouching = crouching && !IsSmall && !IsDead;
            if (IsCrouching)
            {
                // Crouching ends the throw pose.
                _throwTimeRemaining = 0;
            }
        }
    }
}
