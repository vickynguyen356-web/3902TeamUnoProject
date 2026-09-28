using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class Mushroom : IItem
    {
        private IItemSprite itemSprite;
        private Vector2 startingPosition;
        public ItemType Type
        {
            get { return ItemType.Mushroom; }
        }
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
        }

        private const float MoveSpeed = 60f;

        public Mushroom(Vector2 position, IItemSprite sprite)
        {
            this.position = position;
            startingPosition = position;
            itemSprite = sprite;
        }

        public void Update(GameTime gameTime)
        {
            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            position += new Vector2(MoveSpeed * seconds, 0);
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
