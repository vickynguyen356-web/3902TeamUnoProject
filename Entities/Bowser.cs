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
        private bool _isSpittingFire;
        private float _spitFireTimer;
        private const float SpitFireDuration = 0.6f;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _bowserAnimations;

        public Bowser(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _bowserAnimations = BowserSpriteFactory.CreateBowserAnimations();
            _distanceTraveled = 0f;
            Velocity.X = -WalkSpeed;
            _isSpittingFire = false;

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

            if (_isSpittingFire)
            {
                _spitFireTimer += elapsedSeconds;
                if (_spitFireTimer >= SpitFireDuration)
                {
                    _isSpittingFire = false;
                    _spitFireTimer = 0f;
                }
            }

            UpdateAnimation(gameTime);
        }

        public BowserFireball SpitFire(ISprite fireballSprite)
        {
            _isSpittingFire = true;
            _spitFireTimer = 0f;

            // spawns fireball to left if -1 and right if 1 
            float direction = StateMachine.IsFlipped ? -1f : 1f;

            Vector2 fireballPosition = new Vector2(direction < 0
              ? Position.X - BowserFireball.FireballWidth
              : Position.X + Width,
              // fireball y coordinate goes halfway down bowser's height and subtracts fireball height to align with the mouth
          Position.Y + (Height / 2) - BowserFireball.FireballHeight);

            Vector2 fireballVelocity = new Vector2(direction * FireballSpeed, 0f);

            return new BowserFireball(fireballSprite, fireballPosition, fireballVelocity);

        }
        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);

            Sprite.Update(gameTime, GetCurrentAnimation());
        }

        private SpriteAnimation GetCurrentAnimation()
        {
            if (_isSpittingFire && _bowserAnimations.TryGetValue(EntityAnimationState.SpitFire, 
                out SpriteAnimation spitFireanimation))
            {
                return spitFireanimation;
            }

            if (_bowserAnimations.TryGetValue(StateMachine.AnimationState, out SpriteAnimation animation))
            {
                return animation;
            }

            return _bowserAnimations[EntityAnimationState.Idle];
        }
    }
}
