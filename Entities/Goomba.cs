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

        public Goomba(ISprite sprite, Vector2 position, IProjectileFactory projectileFactory)
            : base(sprite, position, projectileFactory)
        {
            _goombaAnimations = GoombaSpriteFactory.CreateGoombaAnimations();
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

            UpdateAnimation(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);

            Sprite.Update(gameTime, GetCurrentAnimation());
        }

        private SpriteAnimation GetCurrentAnimation()
        {
            SpriteAnimation animation;

            if (_goombaAnimations.TryGetValue(StateMachine.AnimationState, out animation))
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
