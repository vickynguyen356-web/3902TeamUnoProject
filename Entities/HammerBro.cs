using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Entities
{
    public class HammerBro : Enemy
    {
        public const int HammerBroWidth = 64;
        public const int HammerBroHeight = 64;
        public override int Width
        {
            get
            {
                return HammerBroWidth;
            }
        }

        public override int Height
        {
            get
            {
                return HammerBroHeight;
            }
        }

        private const float RunSpeed = 45f;
        private const float PatrolDistance = 100f;
        // 1 = right, -1 = left
        private int _direction = -1;
        public float HammerThrowDirection
        {
            get;
            private set;
        }
        
        public bool ShouldThrowHammer
        {
            get;
            private set;
        }
        private bool _isThrowing;
        private float _throwTimer;
        private const float ThrowDuration = 0.56f;
        private const float ThrowReleaseTime = 0.28f;
        private bool _hammerReleased;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _hammerBroAnimations;
        private readonly float _leftBound;
        private readonly float _rightBound;

        public HammerBro(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _hammerBroAnimations = HammerBroSpriteFactory.CreateHammerBroAnimations();
            _leftBound = position.X - PatrolDistance;
            _rightBound = position.X + PatrolDistance;
            Velocity.X = RunSpeed * _direction;
            HammerThrowDirection = _direction;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                if (_isThrowing)
                {
                    // Stop moving while throwing.
                    Velocity.X = 0f;

                    _throwTimer = _throwTimer + elapsedSeconds;

                    // Release the hammer halfway through the throw animation.
                    if (_throwTimer >= ThrowReleaseTime && !_hammerReleased)
                    {
                        HammerThrowDirection = _direction;
                        ShouldThrowHammer = true;
                        _hammerReleased = true;
                    }

                    // Finish the throw and start walking in the opposite direction.
                    if (_throwTimer >= ThrowDuration)
                    {
                        _throwTimer = 0f;
                        _isThrowing = false;
                        _hammerReleased = false;
                        _direction = _direction * -1;
                    }
                }
                else
                {
                    // Walk in the current direction.
                    Velocity.X = RunSpeed * _direction;
                    Position = Position + Velocity * elapsedSeconds;

                    // Reached the left boundary.
                    if (Position.X <= _leftBound)
                    {
                        Position = new Vector2(_leftBound, Position.Y);
                        _isThrowing = true;
                        _throwTimer = 0f;
                        _hammerReleased = false;
                    }
                    // Reached the right boundary.
                    else if (Position.X >= _rightBound)
                    {
                        Position = new Vector2(_rightBound, Position.Y);
                        _isThrowing = true;
                        _throwTimer = 0f;
                        _hammerReleased = false;
                    }
                }
            }
            else
            {
                Velocity.X = 0f;
            }

            UpdateAnimation(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);
            EntityAnimationState animationState;

            if (Velocity.X < 0)
            {
                StateMachine.IsFlipped = false;
            }
            else if (Velocity.X > 0)
            {
                StateMachine.IsFlipped = true;
            }

            if (_isThrowing)
            {
                animationState = EntityAnimationState.ThrowHammer;
            }
            else
            {
                animationState = StateMachine.AnimationState;
            }
            SpriteAnimation animation;

            if (_hammerBroAnimations.TryGetValue(animationState,
                out animation))
            {
                Sprite.Update(gameTime, animation);
            }
            else
            {
                Sprite.Update(gameTime, _hammerBroAnimations[EntityAnimationState.Idle]);
            }
        }

        public bool ConsumeHammerThrowRequest()
        {
            if (!ShouldThrowHammer)
            {
                return false;
            }

            ShouldThrowHammer = false;
            return true;
       
            }
        }
}

