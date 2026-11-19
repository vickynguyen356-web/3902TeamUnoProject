using TeamUno.Mario.Entities.Sprites;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class SpinySpriteFactory
    {
        internal static SpriteAnimation CreateEggAnimation()
        {
            return new SpriteAnimation(4f / 60f,
                EnemySpriteSheet.CreateEnemyIdFrame(500, 16, 16),
                EnemySpriteSheet.CreateEnemyIdFrame(501, 16, 16));
        }

        internal static SpriteAnimation CreateWalkingAnimation()
        {
            return new SpriteAnimation(4f / 60f,
                EnemySpriteSheet.CreateEnemyIdFrame(502, 16, 16),
                EnemySpriteSheet.CreateEnemyIdFrame(503, 16, 16));
        }
    }
}
