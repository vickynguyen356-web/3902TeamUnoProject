using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
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
            : base(position, sprite, ItemSpriteFactory.CreateOneUpMushroomAnimation())
            // the Item constructor sets up position, sprite, and animation
        {
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // move horizontally to the right without changing the height
            Position = Position + new Vector2(MoveSpeed * seconds, 0);
            // update sprite through the shared Item behavior
            base.Update(gameTime);
        }
    }
}
