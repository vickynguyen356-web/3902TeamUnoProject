using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class PiranhaSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreatePiranhaAnimations()
        {
            SpriteAnimation closed = new SpriteAnimation(1.0f, CreateFrame(1, 7, 1, 7));

            SpriteAnimation open = new SpriteAnimation(1.0f, CreateFrame(0, 7, 0, 7));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Closed, closed },
                { EntityAnimationState.Open, open }
            };
        }

        /* For evenly spaced spritesheets */
        private static SpriteFrame CreateFrame(int leftCol, int leftRow, int rightCol, int rightRow)
        {
            Rectangle leftFrame = new Rectangle(
                leftCol * FrameWidth,
                leftRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            Rectangle rightFrame = new Rectangle(
                rightCol * FrameWidth,
                rightRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            return new SpriteFrame(leftFrame, rightFrame);
        }
    }
}
