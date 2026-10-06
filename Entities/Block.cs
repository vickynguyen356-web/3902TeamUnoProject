using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Entities
{
    public enum BlockType
    {
        Brick,
        Question,
        Used,
        Ground,
        Pipe,
        Solid,
        FlagPole,
        Empty
    }

    public class Block
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;

        private BlockSprite _spriteBlock;
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
            _spriteBlock = BlockSpriteFactory.Create(type);
        }

        public void Update(GameTime gameTime)
        {
            _spriteBlock.Update(gameTime);
        }

        internal void HitFromBelow()
        {
            if (Type != BlockType.Question)
            {
                return;
            }

            Type = BlockType.Solid;
            _spriteBlock = BlockSpriteFactory.Create(Type);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            if (blockTexture == null)
            {
                return;
            }

            Rectangle sourceRectangle = _spriteBlock.GetSourceRectangle();
            if (sourceRectangle == Rectangle.Empty)
            {
                return;
            }

            spriteBatch.Draw(blockTexture, Bounds, sourceRectangle, Color.White);
        }

    }
}
