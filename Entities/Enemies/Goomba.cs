using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class Goomba : Enemy
    {
        public const int GoombaWidth = 32;
        public const int GoombaHeight = 32;
        private const float RunSpeed = 45f;
        private const float PatrolDistance = 100f;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _goombaAnimations;
        private readonly float _leftBound;
        private readonly float _rightBound;

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

        public Goomba(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _goombaAnimations = GoombaSpriteFactory.CreateGoombaAnimations();
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
    }
}
