using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class EnemySpriteSheet
    {
        internal static SpriteFrame CreateFrame(int x, int y, int width, int height)
        {
            Rectangle source = new Rectangle(x, y, width, height);
            return new SpriteFrame(source, source);
        }

        internal static SpriteFrame CreateEnemyIdFrame(int enemyId, int width, int height)
        {
            const int columns = 35;
            const int frameSize = 16;
            const int sourceOffset = 1;
            return CreateFrame(
                sourceOffset + enemyId % columns * frameSize,
                sourceOffset + enemyId / columns * frameSize,
                width,
                height);
        }
    }
}
