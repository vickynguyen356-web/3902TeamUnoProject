using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public class Bowser : Enemy
    {
        public const int BowserWidth = 64;
        public const int BowserHeight = 64;
        public override int Width
        {
            get
            {
                return BowserWidth;
            }
        }

        public override int Height
        {
            get
            {
                return BowserWidth;
            }
        }

        private const float WalkSpeed = 45f;
        private const float WalkDistance = 150f;
        private const float FireballSpeed = 150f;
        private float _distanceTraveled;
        private bool _hasStopped;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _bowserAnimations;

        public Bowser(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _bowserAnimations = BowserSpriteFactory.CreateBowserAnimations();
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

                Position = Position + new Vector2(movement, 0f);
                _distanceTraveled = _distanceTraveled + Math.Abs(movement);

                if (_distanceTraveled >= WalkDistance)
                {
                    _distanceTraveled = WalkDistance;
                    _hasStopped = true;
                    Velocity = Vector2.Zero;
                }
            }

            UpdateAnimation(gameTime);
        }

        public Fireball SpitFire(ISprite fireballSprite)
        {
            // spawns fireball left of bowser, at height where its mouth is
            float direction = StateMachine.IsFlipped ? -1f : 1f;

            Vector2 fireballPosition = new Vector2(direction < 0
              ? Position.X - Fireball.FireballWidth
              : Position.X + Width,
          Position.Y + (Height / 2) - Fireball.FireballHeight);

            Vector2 fireballVelocity = new Vector2(direction * FireballSpeed, 0f);

            return new Fireball(fireballSprite, fireballPosition, fireballVelocity);

        }
        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);

            Sprite.Update(gameTime, GetCurrentAnimation());
        }

        private SpriteAnimation GetCurrentAnimation()
        {
            SpriteAnimation animation;

            if (_bowserAnimations.TryGetValue(StateMachine.AnimationState, out animation))
            {
                return animation;
            }

            return _bowserAnimations[EntityAnimationState.Idle];
        }
    }
}
