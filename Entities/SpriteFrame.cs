using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Entities
{
    public struct SpriteFrame
    {
        public Rectangle LeftSource { get; private set; }
        public Rectangle RightSource { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }

        public SpriteFrame(Rectangle leftSource, Rectangle rightSource, float offsetX = 0f, float offsetY = 0f)
        {
            LeftSource = leftSource;
            RightSource = rightSource;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public Rectangle GetSourceRectangle(SpriteEffects facingDirection)
        {
            if (facingDirection == SpriteEffects.FlipHorizontally)
            {
                return RightSource;
            }

            return LeftSource;
        }
    }
}
