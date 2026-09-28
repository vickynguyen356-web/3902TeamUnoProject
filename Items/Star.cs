using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class Star : IItem
    {
        private IItemSprite itemSprite;
        private Vector2 startingPosition;
        public ItemType Type
        {
            get { return ItemType.Star; }
        }
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
        }

        private const float MoveSpeed = 90f;
        private const float BounceHeight = 64f;
        private const float BounceSpeed = 160f;
        private float groundY;
        private bool movingUp = true;

        public Star(Vector2 position, IItemSprite sprite)
        {
            this.position = position;
            startingPosition = position;
            itemSprite = sprite;
            groundY = position.Y;
        }

        public void Update(GameTime gameTime)
        {
            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            float x = position.X + MoveSpeed * seconds;
            float y = position.Y;
            if (movingUp)
            {
                y -= BounceSpeed * seconds;
                if (y <= groundY - BounceHeight)
                {
                    y = groundY - BounceHeight;
                    movingUp = false;
                }
            }
            else
            {
                y += BounceSpeed * seconds;
                if (y >= groundY)
                {
                    y = groundY;
                    movingUp = true;
                }
            }
            position = new Vector2(x, y);
            itemSprite.Update(gameTime);
        }

        public void Reset()
        {
            position = startingPosition;
            itemSprite.Reset();
            movingUp = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            itemSprite.Draw(spriteBatch, position);
        }
    }
}
