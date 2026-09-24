using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class ItemSprite : IItemSprite
    {
        private const float Scale = 2f;
        private readonly Texture2D texture;
        private readonly Rectangle[] frames;
        private readonly float frameDuration;
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
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            Vector2 origin = new Vector2(frame.Width / 2f, frame.Height);
            spriteBatch.Draw(texture, position, frame, Color.White,
                0f, origin, Scale, SpriteEffects.None, 0f);
        }

        public void Reset()
        {
            frameTimer = 0;
            frameIndex = 0;
        }
    }
}
