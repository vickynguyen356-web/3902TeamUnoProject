using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class EnemyStateMachine
    {
        private const float MinimumRunningSpeed = 13f;
        private EntityAnimationState _animationState = EntityAnimationState.Idle;
        private bool _isDead;
        private FacingDirection _facingDirection;

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
                FacingDirection = FacingDirection.Left;
            }
            else if (velocity.X > 0)
            {
                FacingDirection = FacingDirection.Right;
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
            FacingDirection = FacingDirection.Left;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Enemy damage will update this enemy's state")]
        public void TakeDamage()
        {
            // TODO: implement enemy damage logic
        }

        public void Kill()
        {
            _isDead = true;
        }
    }
}
