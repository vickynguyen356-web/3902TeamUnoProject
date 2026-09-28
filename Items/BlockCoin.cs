using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class BlockCoin : IItem
    {
        private IItemSprite itemSprite;
        private Vector2 startingPosition;
        public ItemType Type
        {
            get { return ItemType.BlockCoin; }
        }
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
        }

        private const float MoveSpeed = 180f;
        private const float DisplayTime = 0.6f;
        private float timer = DisplayTime;

        private bool visible = true;

        public bool IsVisible
        {
            get { return visible; }
        }

        public BlockCoin(Vector2 position, IItemSprite sprite)
        {
            this.position = position;
            startingPosition = position;
            itemSprite = sprite;
        }

        public void Update(GameTime gameTime)
        {
            if (!visible)
            {
                return;
            }

            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            timer -= seconds;
            if (timer <= 0)
            {
                visible = false;
                return;
            }

            if (timer >= DisplayTime / 2)
            {
                position.Y -= MoveSpeed * seconds;
            }
            else
            {
                position.Y += MoveSpeed * seconds;
            }
            itemSprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                itemSprite.Draw(spriteBatch, position);
            }
        }

        public void Reset()
        {
            position = startingPosition;
            itemSprite.Reset();
            timer = DisplayTime;
            visible = true;
        }
    }
}
