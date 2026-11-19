using System.Collections.Generic;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class PiranhaSpriteFactory
    {
        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreatePiranhaAnimations(
            int sourceTileId)
        {
            const float FrameDurationSeconds = 4f / 60f;
            SpriteAnimation closed = new SpriteAnimation(
                FrameDurationSeconds,
                EnemySpriteSheet.CreateEnemyIdFrame(sourceTileId, 16, 32));

            SpriteAnimation open = new SpriteAnimation(
                FrameDurationSeconds,
                EnemySpriteSheet.CreateEnemyIdFrame(sourceTileId + 1, 16, 32));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Closed, closed },
                { EntityAnimationState.Open, open }
            };
        }
    }
}
