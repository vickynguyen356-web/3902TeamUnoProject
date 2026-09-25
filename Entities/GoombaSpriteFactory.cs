using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class GoombaSpriteFactory
    {
        private const int FrameWidth = 23;
        private const int FrameHeight = 24;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateGoombaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateFrame(0, 0, 0, 0));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                CreateFrame(0, 1, 0, 1),
                CreateFrame(1, 1, 1, 1),
                CreateFrame(2, 1, 2, 1));
            /*
            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(0, 1, 0, 1, frameWidth, frameHeight);
            walkingFrames[1] = CreateFrame(1, 1, 1, 1, frameWidth, frameHeight);
            walkingFrames[2] = CreateFrame(2, 1, 2, 1, frameWidth, frameHeight);
            SpriteAnimation run = new SpriteAnimation(0.28f, walkingFrames); */

            SpriteAnimation dead = new SpriteAnimation(0.80f, CreateFrame(1, 4, 1, 4));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Dead, dead }
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
                rightCol *FrameWidth,
                rightRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            return new SpriteFrame(leftFrame, rightFrame);
        }
    }
}
