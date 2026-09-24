using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public abstract class Item : IItem
    {
        private readonly IItemSprite itemSprite;
        private readonly Vector2 startingPosition;

        public ItemType Type { get; }
        public Vector2 Position { get; protected set; }

        protected Item(ItemType type, Vector2 position, IItemSprite sprite)
        {
            Type = type;
            Position = position;
            startingPosition = position;
            itemSprite = sprite;
        }

        public virtual void Update(GameTime gameTime)
        {
            itemSprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            itemSprite.Draw(spriteBatch, Position);
        }

        public virtual void Reset()
        {
            Position = startingPosition;
            itemSprite.Reset();
        }
    }
}
