using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public static class KoopaSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateKoopaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(6, 1, 6, 1));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(6, 1, 6, 1),
                EnemySpriteSheet.CreateFrame(0, 2, 0, 2));

            SpriteAnimation dead = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(3, 2, 3, 2));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Dead, dead }
            };
        }
    }
}
