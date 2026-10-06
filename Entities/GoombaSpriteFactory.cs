using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public static class GoombaSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateGoombaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(0, 0, 0, 0));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(0, 0, 0, 0),
                EnemySpriteSheet.CreateFrame(1, 0, 1, 0));

            SpriteAnimation dead = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(2, 0, 2, 0));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Dead, dead }
            };
        }
    }
}
