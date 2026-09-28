using System;
using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class EnemyStateMachine
    {
        private const float MinimumRunningSpeed = 13f;
        public EntityAnimationState AnimationState { get; private set; } = EntityAnimationState.Idle;
        public bool IsDead { get; private set; }
        public bool IsFlipped { get; set; }

        public EnemyStateMachine()
        {
            Reset();
        }

        public void Update(Vector2 velocity)
        {
            // checking if enemy is dead
            if (IsDead)
            {
                AnimationState = EntityAnimationState.Dead;
                return;
            }

            if (velocity.X < 0)
            {
                IsFlipped = true;
            } 
            else if (velocity.X > 0)
            {
                IsFlipped = false;
            }

            if (Math.Abs(velocity.X) > MinimumRunningSpeed)
            {
                AnimationState = EntityAnimationState.Run;
            }
            else
            {
                AnimationState = EntityAnimationState.Idle;
            }
        }

        public void SetFacingDirection(float horizontalVelocity)
        {
            if (horizontalVelocity < 0)
            {
                IsFlipped = true;
            }
            else if (horizontalVelocity > 0)
            {
                IsFlipped = false;
            }
        }

        public void SetFacingDirection(int direction)
        {
            IsFlipped = direction < 0;
        }

        public void Reset()
        {
            AnimationState = EntityAnimationState.Idle;
            IsDead = false;
            IsFlipped = false;
        }

        public void TakeDamage()
        {
            // TODO: implement enemy damage logic
        }
    }
}
