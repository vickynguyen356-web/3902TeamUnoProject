using Microsoft.Xna.Framework;
using Sprint0.Interfaces;
using System.Collections.Generic;
using System;

namespace Sprint0.Entities
{
    public class Koopa : Enemy
    {
        /* koopa fields */
        public const int KoopaWidth = 64;
        public const int KoopaHeight = 64;

        // references available after creating koopa
        public override int Width => KoopaWidth;
        public override int Height => KoopaHeight;
        private const float WalkSpeed = 45f;
        private const float WalkDistance = 150f;
        private float _distanceTraveled;
        private bool _hasStopped;

        /* animation related fields */
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _koopaAnimations;
        private readonly Vector2 _startingPos;

        public Koopa(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _koopaAnimations = KoopaSpriteFactory.CreateKoopaAnimations();
            // moving left 150 pixels & stopping
            _startingPos = position;
            _distanceTraveled = 0f;
            Velocity.X = -WalkSpeed;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead && !_hasStopped)
            {
                float movement = Velocity.X * elapsedSeconds;

                Position += new Vector2(movement, 0f);
                _distanceTraveled += Math.Abs(movement);

                if (_distanceTraveled >= WalkDistance)
                {
                    _distanceTraveled = WalkDistance;
                    _hasStopped = true;
                    Velocity = Vector2.Zero;
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
            if (_koopaAnimations.TryGetValue(StateMachine.AnimationState, out SpriteAnimation animation))
            {
                return animation;
            }

            return _koopaAnimations[EntityAnimationState.Idle];
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

            UpdateAnimation(new GameTime());
        }
    }
}

