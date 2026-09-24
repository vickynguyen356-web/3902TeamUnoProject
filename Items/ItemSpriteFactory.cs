using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class ItemSpriteFactory
    {
        private static readonly ItemSpriteFactory instance = new ItemSpriteFactory();
        private Texture2D itemSheet;
        private const float FrameDuration = 0.15f;

        public static ItemSpriteFactory Instance
        {
            get { return instance; }
        }

        private ItemSpriteFactory()
        {
        }

        public void LoadAllTextures(ContentManager content)
        {
            itemSheet = content.Load<Texture2D>("items");
        }

        public IItemSprite CreateMushroomSprite()
        {
            return new ItemSprite(itemSheet,
                new Rectangle[] { new Rectangle(0, 8, 16, 16) }, FrameDuration);
        }

        public IItemSprite CreateOneUpMushroomSprite()
        {
            return new ItemSprite(itemSheet,
                new Rectangle[] { new Rectangle(0, 26, 16, 16) }, FrameDuration);
        }

        public IItemSprite CreateFireFlowerSprite()
        {
            return CreateAnimatedSprite(32, 8, 16, 18);
        }

        public IItemSprite CreateStarSprite()
        {
            return CreateAnimatedSprite(106, 8, 16, 18);
        }

        public IItemSprite CreateCoinSprite()
        {
            return CreateAnimatedSprite(180, 36, 8, 10);
        }

        private IItemSprite CreateAnimatedSprite(int x, int y, int width, int spacing)
        {
            const int frameCount = 4;
            const int height = 16;
            Rectangle[] frames = new Rectangle[frameCount];
            for (int index = 0; index < frames.Length; index++)
            {
                frames[index] = new Rectangle(x + index * spacing, y, width, height);
            }

            return new ItemSprite(itemSheet, frames, FrameDuration);
        }
    }
}
