using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class Star : Item
    {
        private const float MoveSpeed = 90f;
        private const float BounceHeight = 64f;
        private const float BounceDuration = 0.8f;
        private readonly float groundY;
        private float bounceTimer;

        public Star(Vector2 position, IItemSprite sprite)
            : base(ItemType.Star, position, sprite)
        {
            groundY = position.Y;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bounceTimer = (bounceTimer + seconds) % BounceDuration;
            float progress = bounceTimer / BounceDuration;
            float height = 4f * BounceHeight * progress * (1f - progress);
            Position = new Vector2(Position.X + MoveSpeed * seconds, groundY - height);
            base.Update(gameTime);
        }

        public override void Reset()
        {
            base.Reset();
            bounceTimer = 0;
        }
    }
}
