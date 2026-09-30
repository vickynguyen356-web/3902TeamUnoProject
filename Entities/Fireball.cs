using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public class Fireball : Enemy
    {
        public const int FireballWidth = 64;
        public const int FireballHeight = 64;
        public override int Width
        {
            get
            {
                return FireballWidth;
            }
        }
        public override int Height
        {
            get
            {
                return FireballHeight;
            }
        }
        private readonly ISprite _sprite;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _fireballAnimations;

        public Fireball(ISprite sprite, Vector2 position, Vector2 velocity)
            : base(sprite, position)
        {
            _fireballAnimations = FireballSpriteFactory.CreateFireballAnimations();
            Velocity = velocity;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                Position += Velocity * elapsedSeconds;
            }

            UpdateAnimation(gameTime);
        }

        public void OnCollision()
        {
            Kill();
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            Sprite.Update(gameTime, _fireballAnimations[EntityAnimationState.Fireball]);
        }
    }
}
