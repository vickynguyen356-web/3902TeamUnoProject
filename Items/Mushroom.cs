using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public class Mushroom : Item
    // inherits drawing, animation handling, and reset from Item
    {
        // move right at 60 pixels per second
        private const float MoveSpeed = 60f;

        public override ItemType Type
        {
            get
            {
                return ItemType.Mushroom;
            }
        }

        public Mushroom(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateMushroomAnimation())
        {
            // Item constructor sets up position, sprite, and animation
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // increase X to move right
            Position = Position + new Vector2(MoveSpeed * seconds, 0);
            // let Item update the sprite's animation
            base.Update(gameTime);
        }
    }
}
