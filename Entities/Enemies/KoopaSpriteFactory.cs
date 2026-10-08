using System.Collections.Generic;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class KoopaSpriteFactory
    {
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateKoopaAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(407, 103, 16, 23));

            SpriteAnimation run = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(407, 103, 16, 23),
                EnemySpriteSheet.CreateFrame(24, 166, 16, 24));

            SpriteAnimation dead = new SpriteAnimation(0.80f, EnemySpriteSheet.CreateFrame(216, 175, 16, 14));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Dead, dead }
            };
        }
    }
}
