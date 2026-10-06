using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    internal class Koopa : Enemy
    {
        public const int KoopaWidth = 64;
        public const int KoopaHeight = 64;
        public override int Width
        {
            get
            {
                return KoopaWidth;
            }
        }

        public override int Height
        {
            get
            {
                return KoopaHeight;
            }
        }

        private const float WalkSpeed = 45f;
        private const float WalkDistance = 150f;
        private float _distanceTraveled;
        private bool _hasStopped;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _koopaAnimations;
        private readonly Vector2 _startingPos;

        public Koopa(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _koopaAnimations = KoopaSpriteFactory.CreateKoopaAnimations();
            _startingPos = position;
            _distanceTraveled = 0f;
            Velocity.X = -WalkSpeed;

            UpdateMovementAnimation(new GameTime(), _koopaAnimations);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead && !_hasStopped)
            {
                float movement = Velocity.X * elapsedSeconds;

                Position = Position + new Vector2(movement, 0f);
                _distanceTraveled = _distanceTraveled + Math.Abs(movement);

                if (_distanceTraveled >= WalkDistance)
                {
                    _distanceTraveled = WalkDistance;
                    _hasStopped = true;
                    Velocity = Vector2.Zero;
                }
            }

            UpdateMovementAnimation(gameTime, _koopaAnimations);
        }

        public void Reset()
        {
            Position = _startingPos;
            Velocity = Vector2.Zero;
            Velocity.X = -WalkSpeed;

            _distanceTraveled = 0f;
            _hasStopped = false;

            StateMachine.Reset();
            Sprite.Reset();

            UpdateMovementAnimation(new GameTime(), _koopaAnimations);
        }
    }
}
