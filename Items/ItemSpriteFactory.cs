using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public static class ItemSpriteFactory
    {
        public const float SpriteScale = 2f;
        private const float FrameDuration = 0.15f;

        public static ISprite Create(Texture2D itemSheet)
        {
            return new SpriteSheetSprite(itemSheet, SpriteScale);
        }

        internal static SpriteAnimation CreateMushroomAnimation()
        {
            return new SpriteAnimation(FrameDuration, CreateFrame(0, 8, 16));
        }

        internal static SpriteAnimation CreateOneUpMushroomAnimation()
        {
            return new SpriteAnimation(FrameDuration, CreateFrame(0, 26, 16));
        }

        internal static SpriteAnimation CreateFireFlowerAnimation()
        {
            return CreateAnimation(32, 8, 16, 18);
        }

        internal static SpriteAnimation CreateStarAnimation()
        {
            return CreateAnimation(106, 8, 16, 18);
        }

        internal static SpriteAnimation CreateFloatingCoinAnimation()
        {
            return CreateAnimation(180, 36, 8, 10);
        }

        internal static SpriteAnimation CreateBlockCoinAnimation()
        {
            return CreateFloatingCoinAnimation();
        }

        private static SpriteAnimation CreateAnimation(int x, int y, int width, int spacing)
        {
            const int frameCount = 4;
            SpriteFrame[] frames = new SpriteFrame[frameCount];
            for (int index = 0; index < frames.Length; index = index + 1)
            {
                frames[index] = CreateFrame(x + index * spacing, y, width);
            }

            return new SpriteAnimation(FrameDuration, frames);
        }

        private static SpriteFrame CreateFrame(int x, int y, int width)
        {
            Rectangle sourceRectangle = new Rectangle(x, y, width, 16);
            return new SpriteFrame(sourceRectangle, sourceRectangle);
        }
    }
}
