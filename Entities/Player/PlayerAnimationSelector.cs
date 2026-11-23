using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Player
{
    /// <summary>
    /// Chooses which animation the player should be playing based on the 
    /// PlayerStateMachine state and the player's current physics.
    /// </summary>
    internal static class PlayerAnimationSelector
    {
        private const float MinimumRunningSpeed = 15f;

        public static EntityAnimationState Select(
            PlayerStateMachine state,
            bool isGrounded,
            Vector2 velocity,
            bool isClimbing = false,
            bool isSwimming = false,
            bool isDrifting = false)
        {
            if (state.IsDead)
            {
                return EntityAnimationState.Dead;
            }

            if (state.IsChangingForm)
            {
                return EntityAnimationState.Transform;
            }

            if (isClimbing)
            {
                return EntityAnimationState.Climb;
            }

            if (state.IsCrouching)
            {
                return EntityAnimationState.Crouch;
            }

            if (state.IsThrowing)
            {
                return EntityAnimationState.ThrowFireball;
            }

            if (isDrifting)
            {
                return EntityAnimationState.Drift;
            }

            if (isSwimming)
            {
                return SelectSwimmingState(isGrounded, velocity);
            }

            return SelectMovementState(isGrounded, velocity);
        }

        private static EntityAnimationState SelectSwimmingState(bool isGrounded, Vector2 velocity)
        {
            if (!isGrounded && velocity.Y < -MinimumRunningSpeed)
            {
                return EntityAnimationState.SwimJump;
            }

            if (isGrounded && Math.Abs(velocity.X) > MinimumRunningSpeed)
            {
                return EntityAnimationState.SwimWalk;
            }

            return EntityAnimationState.Swim;
        }

        private static EntityAnimationState SelectMovementState(bool isGrounded, Vector2 velocity)
        {
            if (!isGrounded)
            {
                if (!isGrounded)
                {
                    return EntityAnimationState.Jump;
                }

                return EntityAnimationState.Fall;
            }

            if (Math.Abs(velocity.X) > MinimumRunningSpeed)
            {
                return EntityAnimationState.Run;
            }

            return EntityAnimationState.Idle;
        }
    }
}