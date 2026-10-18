using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class OneUpMushroom : Item
    {
        // start moving to the right
        private const float MoveSpeed = 60f;
        private const float Gravity = 1500f;

        public override ItemType Type
        {
            get
            {
                return ItemType.OneUpMushroom;
            }
        }

        public OneUpMushroom(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateOneUpMushroomAnimation())
            // the Item constructor sets up position, sprite, and animation
        {
            Velocity = new Vector2(MoveSpeed, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedSeconds = System.Math.Min(elapsedSeconds, 1f / 30f);
            Velocity.Y = Velocity.Y + Gravity * elapsedSeconds;
            // use the current velocity since a collision can change it
            Position = Position + Velocity * elapsedSeconds;
            // update the animation too
            base.Update(gameTime);
        }
    }
}
