using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class HammerBroSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f, true);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateHammerBroAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(1.0f, CreateFrame(2, 4, 2, 4));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                CreateFrame(1, 4, 1, 4),
                CreateFrame(2, 4, 2, 4));

            SpriteAnimation throwHammer = new SpriteAnimation(0.28f, CreateFrame(3, 4, 3, 4),
                CreateFrame(1, 4, 1, 4));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.ThrowHammer, throwHammer }
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
