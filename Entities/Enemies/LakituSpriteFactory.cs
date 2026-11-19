using TeamUno.Mario.Entities.Sprites;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class LakituSpriteFactory
    {
        private const float FrameDurationSeconds = 4f / 60f;
        private const int LakituTileId = 87;
        private const int CloudTileId = 86;

        internal static SpriteAnimation CreateLakituAnimation()
        {
            return new SpriteAnimation(
                FrameDurationSeconds,
                EnemySpriteSheet.CreateEnemyIdFrame(LakituTileId, 16, 32));
        }

        internal static SpriteAnimation CreateCloudAnimation()
        {
            return new SpriteAnimation(
                FrameDurationSeconds,
                EnemySpriteSheet.CreateEnemyIdFrame(CloudTileId, 16, 16));
        }
    }
}