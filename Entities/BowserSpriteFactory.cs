using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class BowserSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateBowserAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.28f, CreateFrame(1, 5, 1, 5));

            SpriteAnimation walk = new SpriteAnimation(0.28f, CreateFrame(5, 4, 5, 4),
                CreateFrame(6, 4, 6, 4));

            //SpriteAnimation spitFire = new SpriteAnimation(0.28f,
            //    CreateFrame(5, 4, 5, 4),
            //    CreateFrame(0, 5, 0, 5));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, walk }
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
