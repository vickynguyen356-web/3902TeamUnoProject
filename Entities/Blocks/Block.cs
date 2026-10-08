using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Entities.Blocks
{
    internal enum BlockType
    {
        Brick,
        Question,
        Used,
        Ground,
        Pipe,
        Solid,
        FlagPole,
        Empty,
        Platform,
        Vine
    }

    internal class Block
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;

        private BlockSprite _sprite;
        private BlockType _type;
        private readonly Vector2 _position;
        private readonly int _width;
        private readonly int _height;

        public BlockType Type
        {
            get
            {
                return _type;
            }
            private set
            {
                _type = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public Block(Vector2 position, BlockType type, int width = DefaultWidth, int height = DefaultHeight)
        {
            _position = position;
            Type = type;
            _width = width;
            _height = height;
            _sprite = BlockSpriteFactory.Create(type);
        }

        public void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
        }

        internal void HitFromBelow()
        {
            if (Type != BlockType.Question)
            {
                return;
            }

            Type = BlockType.Used;
            _sprite = BlockSpriteFactory.Create(Type);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            if (blockTexture == null)
            {
                return;
            }

            Rectangle sourceRectangle = _sprite.GetSourceRectangle();
            if (sourceRectangle == Rectangle.Empty)
            {
                return;
            }

            spriteBatch.Draw(blockTexture, Bounds, sourceRectangle, Color.White);
        }
    }
}
