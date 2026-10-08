using System.Collections.Generic;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class HammerBroSpriteFactory
    {
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateHammerBroAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(1.0f, EnemySpriteSheet.CreateFrame(152, 295, 16, 24));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(87, 295, 16, 24),
                EnemySpriteSheet.CreateFrame(152, 295, 16, 24));

            SpriteAnimation throwHammer = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(281, 221, 15, 34),
                EnemySpriteSheet.CreateFrame(87, 295, 16, 24));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.ThrowHammer, throwHammer }
            };
        }
    }
}
