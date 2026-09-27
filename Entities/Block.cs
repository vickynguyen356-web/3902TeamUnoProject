using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public enum BlockType
    {
        Brick,
        Question,
        Used,
        Ground,
        Pipe,
        Solid,
        Coin,
        Empty
    }

    public class Block
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;

        private readonly IBlock _spriteBlock;

        public BlockType Type { get; private set; }
        public Vector2 Position { get; }
        public int Width { get; }
        public int Height { get; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public Block(Vector2 position, BlockType type, int width = DefaultWidth, int height = DefaultHeight)
        {
            Position = position;
            Type = type;
            Width = width;
            Height = height;
            _spriteBlock = BlockSpriteFactory.Create(type);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            if (blockTexture == null)
            {
                return;
            }

            Rectangle sourceRectangle = _spriteBlock.GetSourceRectangle();
            spriteBatch.Draw(blockTexture, Bounds, sourceRectangle, Color.White);
        }

    }
}
