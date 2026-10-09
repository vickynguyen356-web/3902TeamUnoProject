using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class OneUpMushroom : Item
    {
        // move right at 60 pixels per second
        private const float MoveSpeed = 60f;

        public override ItemType Type
        {
            get
            {
                return ItemType.OneUpMushroom;
            }
        }

        public OneUpMushroom(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateOneUpMushroomAnimation(), 32, 32)
            // the Item constructor sets up position, sprite, and animation
        {
            Velocity = new Vector2(MoveSpeed, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position = Position + Velocity * elapsedSeconds;
            // update sprite through the shared Item behavior
            base.Update(gameTime);
        }
    }
}
