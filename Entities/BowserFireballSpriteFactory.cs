using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    internal class BowserFireballSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;

        private readonly Texture2D _enemiesTexture;
        public BowserFireballSpriteFactory(Texture2D enemyTexture)
        {
            _enemiesTexture = enemyTexture;
        }

        public ISprite Create()
        {
            return new SpriteSheetSprite(_enemiesTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateFireballAnimations()
        {
            SpriteAnimation fireball = new SpriteAnimation(0.28f, CreateFrame(4, 5, 4, 5),
                CreateFrame(5, 5, 5, 5));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Fireball, fireball }
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
