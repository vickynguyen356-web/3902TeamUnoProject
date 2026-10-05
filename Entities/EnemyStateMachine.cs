using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    internal class EnemyStateMachine
    {
        private const float MinimumRunningSpeed = 13f;
        private EntityAnimationState _animationState = EntityAnimationState.Idle;
        private bool _isDead;
        private bool _isFlipped;

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

        public bool IsFlipped
        {
            get
            {
                return _isFlipped;
            }
            set
            {
                _isFlipped = value;
            }
        }

        public EnemyStateMachine()
        {
            Reset();
        }

        public void Update(Vector2 velocity)
        {
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

        public void Reset()
        {
            AnimationState = EntityAnimationState.Idle;
            IsDead = false;
            IsFlipped = false;
        }

        public static void TakeDamage()
        {
            // TODO: implement enemy damage logic
        }

        public void Kill()
        {
            _isDead = true;
        }
    }
}
