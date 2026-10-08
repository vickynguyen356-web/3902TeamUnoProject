using System.Collections.Generic;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class PiranhaSpriteFactory
    {
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreatePiranhaAnimations()
        {
            SpriteAnimation closed = new SpriteAnimation(1.0f, EnemySpriteSheet.CreateFrame(88, 487, 16, 24));

            SpriteAnimation open = new SpriteAnimation(1.0f, EnemySpriteSheet.CreateFrame(23, 488, 16, 23));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Closed, closed },
                { EntityAnimationState.Open, open }
            };
        }
    }
}
