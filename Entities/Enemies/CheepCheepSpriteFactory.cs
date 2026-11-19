using TeamUno.Mario.Entities.Sprites;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class CheepCheepSpriteFactory
    {
        internal const int GreenTileId = 81;
        internal const int RedTileId = 498;

        internal static SpriteAnimation CreateSwimAnimation(int sourceTileId)
        {
            return new SpriteAnimation(
                1f / 6f,
                EnemySpriteSheet.CreateEnemyIdFrame(sourceTileId, 16, 16),
                EnemySpriteSheet.CreateEnemyIdFrame(sourceTileId + 1, 16, 16));
        }
    }
}
