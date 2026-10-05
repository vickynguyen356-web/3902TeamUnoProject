using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    internal class KoopaSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateKoopaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateFrame(6, 1, 6, 1));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                CreateFrame(6, 1, 6, 1),
                CreateFrame(0, 2, 0, 2));

            SpriteAnimation dead = new SpriteAnimation(0.80f, CreateFrame(3, 2, 3, 2));

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
