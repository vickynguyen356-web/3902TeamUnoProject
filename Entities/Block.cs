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
        FlagPole,
        Empty
    }

    public class Block
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;

        private IBlock _spriteBlock;

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
            spriteBatch.Draw(blockTexture, Bounds, sourceRectangle, Color.White);
        }

    }
}
