using System.Collections.Generic;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class BowserSpriteFactory
    {
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateBowserAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.28f, EnemySpriteSheet.CreateFrame(80, 351, 32, 32));

            SpriteAnimation walk = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(16, 351, 32, 32),
                EnemySpriteSheet.CreateFrame(80, 351, 32, 32));

            SpriteAnimation spitFire = new SpriteAnimation(0.28f,
                EnemySpriteSheet.CreateFrame(400, 287, 32, 32),
                EnemySpriteSheet.CreateFrame(335, 287, 32, 32));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, walk },
                { EntityAnimationState.SpitFire, spitFire }
            };
        }
    }
}
