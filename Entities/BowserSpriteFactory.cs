using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public static class BowserSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateBowserAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.28f, EnemySpriteSheet.CreateFrame(1, 5, 1, 5));

            SpriteAnimation walk = new SpriteAnimation(0.28f, EnemySpriteSheet.CreateFrame(0, 5, 0, 5),
                EnemySpriteSheet.CreateFrame(1, 5, 1, 5));

            SpriteAnimation spitFire = new SpriteAnimation(0.28f, EnemySpriteSheet.CreateFrame(6, 4, 6, 4),
                EnemySpriteSheet.CreateFrame(5, 4, 5, 4));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, walk },
                { EntityAnimationState.SpitFire, spitFire }
            };
        }
    }
}
