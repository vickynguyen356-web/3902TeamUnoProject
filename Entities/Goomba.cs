using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public class Goomba : Enemy
    {
        public const int GoombaWidth = 64;
        public const int GoombaHeight = 64;
        public override int Width
        {
            get
            {
                return GoombaWidth;
            }
        }

        public override int Height
        {
            get
            {
                return GoombaHeight;
            }
        }

        private const float RunSpeed = 45f;
        private const float PatrolDistance = 100f;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _goombaAnimations;
        private readonly Vector2 _startingPos;
        private readonly float _leftBound;
        private readonly float _rightBound;

        public Goomba(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _goombaAnimations = GoombaSpriteFactory.CreateGoombaAnimations();
            _startingPos = position;
            _leftBound = position.X - PatrolDistance;
            _rightBound = position.X + PatrolDistance;
            Velocity.X = -RunSpeed;

            UpdateMovementAnimation(new GameTime(), _goombaAnimations);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                Position = Position + Velocity * elapsedSeconds;

                if (Position.X <= _leftBound)
                {
                    Velocity.X = RunSpeed;
                }
                else if (Position.X >= _rightBound)
                {
                    Velocity.X = -RunSpeed;
                }
            }

            UpdateMovementAnimation(gameTime, _goombaAnimations);
        }

        public void Reset()
        {
            Position = _startingPos;
            Velocity.X = -RunSpeed;

            StateMachine.Reset();
            Sprite.Reset();

            UpdateMovementAnimation(new GameTime(), _goombaAnimations);
        }
    }
}
