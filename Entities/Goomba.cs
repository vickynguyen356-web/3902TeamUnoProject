using Microsoft.Xna.Framework;
using Sprint0.Interfaces;
using System.Collections.Generic;

namespace Sprint0.Entities
{
    public class Goomba : Enemy
    {
        /* goomba fields */
        public const int GoombaWidth = 64;
        public const int GoombaHeight = 64;

        // references available after creating Goomba
        public override int Width => GoombaWidth;
        public override int Height => GoombaHeight;
        private const float RunSpeed = 45f;
        private const float PatrolDistance = 100f;
        /* animation related fields */
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _goombaAnimations;
        private readonly Vector2 _startingPos;
        private readonly float _leftBound;
        private readonly float _rightBound;

        public Goomba(ISprite sprite, Vector2 position) 
            : base(sprite, position)
        {
            _goombaAnimations = GoombaSpriteFactory.CreateGoombaAnimations();
            // moving left 100 pixels, then turning right and moving 100 pixels back
            _startingPos = position;
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
                Position += Velocity * elapsedSeconds;

                if (Position.X <= _leftBound)
                {
                    Velocity.X = RunSpeed;
                } else if (Position.X >= _rightBound)
                {
                    Velocity.X = -RunSpeed;
                }
            }

            UpdateAnimation(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);

            Sprite.Update(gameTime, GetCurrentAnimation());
        }

        private SpriteAnimation GetCurrentAnimation()
        {
            if (_goombaAnimations.TryGetValue(StateMachine.AnimationState, out SpriteAnimation animation))
            {
                return animation;
            }

            return _goombaAnimations[EntityAnimationState.Idle];
        }

        public void Reset()
        {
            Position = _startingPos;
            Velocity.X = -RunSpeed;

            StateMachine.Reset();
            Sprite.Reset();

            UpdateAnimation(new GameTime());
        }
    }
}
