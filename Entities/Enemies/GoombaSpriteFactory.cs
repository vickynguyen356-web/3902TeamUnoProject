using System.Collections.Generic;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class GoombaSpriteFactory
    {
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateGoombaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(23, 47, 16, 16));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(23, 47, 16, 16),
                EnemySpriteSheet.CreateFrame(88, 47, 16, 16));

            SpriteAnimation dead = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(152, 55, 16, 8));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Dead, dead }
            };
        }
    }
}
