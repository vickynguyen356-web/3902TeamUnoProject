using Microsoft.Xna.Framework;

namespace TeamUno.Mario.Entities
{
    internal static class EnemySpriteSheet
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;

        internal static SpriteFrame CreateFrame(int leftCol, int leftRow, int rightCol, int rightRow)
        {
            Rectangle leftFrame = new Rectangle(
                leftCol * FrameWidth,
                leftRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            Rectangle rightFrame = new Rectangle(
                rightCol * FrameWidth,
                rightRow * FrameHeight,
                FrameWidth,
                FrameHeight);

            return new SpriteFrame(leftFrame, rightFrame);
        }
    }
}
