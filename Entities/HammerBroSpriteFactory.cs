using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    internal static class HammerBroSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f, true);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateHammerBroAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(1.0f, EnemySpriteSheet.CreateFrame(2, 4, 2, 4));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(1, 4, 1, 4),
                EnemySpriteSheet.CreateFrame(2, 4, 2, 4));

            SpriteAnimation throwHammer = new SpriteAnimation(0.28f, EnemySpriteSheet.CreateFrame(4, 3, 4, 3),
                EnemySpriteSheet.CreateFrame(1, 4, 1, 4));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.ThrowHammer, throwHammer }
            };
        }
    }
}
