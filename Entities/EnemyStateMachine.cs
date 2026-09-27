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
        public bool IsFlipped { get; private set; }

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
            }
            else if (Math.Abs(velocity.X) > MinimumRunningSpeed)
            {
                AnimationState = EntityAnimationState.Run;
            }
            else
            {
                AnimationState = EntityAnimationState.Idle;
            }

            // flip sprite based on horizontal movement
            if (velocity.X <0)
            {
                IsFlipped = true;
            }
            else if (velocity.X > 0)
            {
                IsFlipped = false;
            }
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
