using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class Star : Item
    {
        private const float MoveSpeed = 90f;
        private const float Gravity = 1500f;
        public const float BounceSpeed = 450f;

        public override ItemType Type
        {
            get { return ItemType.Star; }
        }

        public Star(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateStarAnimation())
        {
            Velocity = new Vector2(MoveSpeed, -BounceSpeed);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedSeconds = System.Math.Min(elapsedSeconds, 1f / 30f);
            // gravity brings the star back down to the terrain
            Velocity.Y = Velocity.Y + Gravity * elapsedSeconds;
            Position = Position + Velocity * elapsedSeconds;
            base.Update(gameTime);
        }
    }
}
