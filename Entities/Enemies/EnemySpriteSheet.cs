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
    }
}
