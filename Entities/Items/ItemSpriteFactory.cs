// this file selects the art for each item
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal static class ItemSpriteFactory
    {
        // draw sprites at twice their original size
        public const float SpriteScale = 2f;
        // each animation frame lasts 0.15 seconds
        private const float FrameDuration = 0.15f;

        public static ISprite Create(Texture2D itemSheet)
        {
            // create a sprite using item sheet
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
            // start at (32, 8): frames are 16 pixels wide with 18 pixels between the start of each frame
            return CreateAnimation(32, 8, 16, 18);
        }

        internal static SpriteAnimation CreateStarAnimation()
        {
            // use the four star pictures
            return CreateAnimation(106, 8, 16, 18);
        }

        internal static SpriteAnimation CreateFloatingCoinAnimation()
        {
            // coin pictures are 8 pixels wide with starts 10 pixels apart
            return CreateAnimation(180, 36, 8, 10);
        }

        internal static SpriteAnimation CreateBlockCoinAnimation()
        {
            // both coin types use same pictures but their item classes have different movement
            return CreateFloatingCoinAnimation();
        }

        private static SpriteAnimation CreateAnimation(int x, int y, int width, int spacing)
        {
            const int frameCount = 4;
            SpriteFrame[] frames = new SpriteFrame[frameCount];
            // move across sheet to select 4 animation frames
            for (int index = 0; index < frames.Length; index = index + 1)
            {
                frames[index] = CreateFrame(x + index * spacing, y, width);
            }

            // combine pictures with time each picture stays visible.
            return new SpriteAnimation(FrameDuration, frames);
        }

        private static SpriteFrame CreateFrame(int x, int y, int width)
        {
            Rectangle sourceRectangle = new Rectangle(x, y, width, 16);
            // items use same picture for both directions they face
            return new SpriteFrame(sourceRectangle, sourceRectangle);
        }
    }
}
