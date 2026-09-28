using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class FireFlower : IItem
    {
        private IItemSprite itemSprite;
        private Vector2 startingPosition;
        public ItemType Type
        {
            get { return ItemType.FireFlower; }
        }
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
        }

        public FireFlower(Vector2 position, IItemSprite sprite)
        {
            this.position = position;
            startingPosition = position;
            itemSprite = sprite;
        }
        public void Update(GameTime gameTime)
        {
            itemSprite.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            itemSprite.Draw(spriteBatch, position);
        }
        public void Reset()
        {
            position = startingPosition;
            itemSprite.Reset();
        }
    }
}
