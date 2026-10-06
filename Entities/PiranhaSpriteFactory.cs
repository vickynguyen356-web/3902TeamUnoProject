using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public static class PiranhaSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreatePiranhaAnimations()
        {
            SpriteAnimation closed = new SpriteAnimation(1.0f, EnemySpriteSheet.CreateFrame(1, 7, 1, 7));

            SpriteAnimation open = new SpriteAnimation(1.0f, EnemySpriteSheet.CreateFrame(0, 7, 0, 7));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Closed, closed },
                { EntityAnimationState.Open, open }
            };
        }
    }
}
