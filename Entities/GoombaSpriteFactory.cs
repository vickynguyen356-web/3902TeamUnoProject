using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public class GoombaSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateGoombaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateFrame(0, 0, 0, 0));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                CreateFrame(0, 0, 0, 0),
                CreateFrame(1, 0, 1, 0));

            SpriteAnimation dead = new SpriteAnimation(0.80f, CreateFrame(2, 0, 2, 0));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Dead, dead }
            };
        }

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
