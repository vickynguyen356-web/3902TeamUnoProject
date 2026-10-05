using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Entities
{
    internal readonly struct SpriteFrame
    {
        private readonly Rectangle _leftSource;
        private readonly Rectangle _rightSource;
        private readonly float _offsetX;
        private readonly float _offsetY;

        public Rectangle LeftSource
        {
            get
            {
                return _leftSource;
            }
        }

        public Rectangle RightSource
        {
            get
            {
                return _rightSource;
            }
        }

        public float OffsetX
        {
            get
            {
                return _offsetX;
            }
        }

        public float OffsetY
        {
            get
            {
                return _offsetY;
            }
        }

        public SpriteFrame(Rectangle leftSource, Rectangle rightSource, float offsetX = 0f, float offsetY = 0f)
        {
            _leftSource = leftSource;
            _rightSource = rightSource;
            _offsetX = offsetX;
            _offsetY = offsetY;
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
