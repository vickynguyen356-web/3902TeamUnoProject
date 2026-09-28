using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;
using Sprint0.Items;

namespace Sprint0.Entities
{
    public class Coin : IItem
    {
        private IItemSprite itemSprite;
        private Vector2 startingPosition;
        public ItemType Type
        {
            get { return ItemType.Coin; }
        }
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
        }

        public Coin(Vector2 position)
        {
            this.position = position;
            startingPosition = position;
            itemSprite = ItemSpriteFactory.Instance.CreateCoinSprite();
        }

        public Coin(Vector2 position, IItemSprite sprite)
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
