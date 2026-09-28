using Microsoft.Xna.Framework;
using Sprint0.Interfaces;
using System;
using System.Collections.Generic;

namespace Sprint0.Entities
{
    public class Bowser : Enemy
    {
        /* bowser fields */
        public const int BowserWidth = 64;
        public const int BowserHeight = 64;

        // references available after creating hammer bro
        public override int Width => BowserWidth;
        public override int Height => BowserWidth;
        private const float WalkSpeed = 45f;
        private const float WalkDistance = 150f;
        private float _distanceTraveled;
        private bool _hasStopped;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _bowserAnimations;
        private readonly Vector2 _startingPos;

        public Bowser(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _bowserAnimations = BowserSpriteFactory.CreateBowserAnimations();
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
            if (_bowserAnimations.TryGetValue(StateMachine.AnimationState, out SpriteAnimation animation))
            {
                return animation;
            }

            return _bowserAnimations[EntityAnimationState.Idle];
        }
    }
}


