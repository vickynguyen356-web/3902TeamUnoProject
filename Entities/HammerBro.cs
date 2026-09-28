using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

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
        private int _direction = 1;
        private bool _isThrowing;
        private float _throwTimer;
        private const float ThrowDuration = 0.8f;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _hammerBroAnimations;
        private readonly float _leftBound;
        private readonly float _rightBound;

        public HammerBro(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _hammerBroAnimations = HammerBroSpriteFactory.CreateHammerBroAnimations();
            _leftBound = position.X - PatrolDistance;
            _rightBound = position.X + PatrolDistance;
            Velocity.X = -RunSpeed;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                if (_isThrowing)
                {
                    Velocity.X = 0f;
                    _throwTimer = _throwTimer + elapsedSeconds;

                    if (_throwTimer >= ThrowDuration)
                    {
                        _throwTimer = 0f;
                        _isThrowing = false;
                        _direction = _direction * -1;
                        Velocity.X = RunSpeed * _direction;
                    }
                }
                else
                {
                    Velocity.X = RunSpeed * _direction;
                    Position = Position + Velocity * elapsedSeconds;

                    if (Position.X <= _leftBound)
                    {
                        Position = new Vector2(_leftBound, Position.Y);
                        _isThrowing = true;
                        _throwTimer = 0f;
                    }
                    else if (Position.X >= _rightBound)
                    {
                        Position = new Vector2(_rightBound, Position.Y);
                        _isThrowing = true;
                        _throwTimer = 0f;
                    }
                }
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
    }
}
