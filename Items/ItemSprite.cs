using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class ItemSprite : IItemSprite
    {
        private const float Scale = 2f;
        private Texture2D texture;
        private Rectangle[] frames;
        private float frameDuration;
        private float frameTimer;
        private int frameIndex;

        public ItemSprite(Texture2D texture, Rectangle[] frames, float frameDuration)
        {
            this.texture = texture;
            this.frames = frames;
            this.frameDuration = frameDuration;
        }

        public void Update(GameTime gameTime)
        {
            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            frameTimer += seconds;
            while (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex++;

                if (frameIndex >= frames.Length)
                {
                    frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Rectangle frame = frames[frameIndex];
            int width = (int)(frame.Width * Scale);
            int height = (int)(frame.Height * Scale);
            Rectangle destination = new Rectangle(
                (int)position.X - width / 2, (int)position.Y - height, width, height);
            spriteBatch.Draw(texture, destination, frame, Color.White);
        }

        public void Reset()
        {
            frameTimer = 0;
            frameIndex = 0;
        }
    }
}
