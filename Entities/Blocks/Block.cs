using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

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
        Invisible,
        FlagPole,
        Empty,
        Platform,
        Vine
    }

    internal class Block : ICollidable
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;

        private BlockSprite _sprite;
        private BlockType _type;
        private bool _isDeleted;
        private readonly Vector2 _position;
        private readonly int _width;
        private readonly int _height;
        private Block _pipeDestination;

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

        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
        }

        internal Block PipeDestination
        {
            get
            {
                return _pipeDestination;
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

        internal void SetPipeDestination(Block destination)
        {
            if (Type != BlockType.Pipe)
            {
                throw new InvalidOperationException("Only pipe blocks can have pipe destinations.");
            }

            ArgumentNullException.ThrowIfNull(destination);
            _pipeDestination = destination;
        }

        internal void HitFromBelow(bool canBreak)
        {
            if (Type == BlockType.Question || Type == BlockType.Invisible)
            {
                Type = BlockType.Used;
                _sprite = BlockSpriteFactory.Create(Type);
            }
            else if (Type == BlockType.Brick && canBreak)
            {
                _isDeleted = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            if (blockTexture == null)
            {
                return;
            }

            if (Type == BlockType.Vine)
            {
                DrawVine(spriteBatch, blockTexture);
                return;
            }

            Rectangle sourceRectangle = _sprite.GetSourceRectangle();
            if (sourceRectangle == Rectangle.Empty)
            {
                return;
            }

            spriteBatch.Draw(blockTexture, Bounds, sourceRectangle, Color.White);
        }

        private void DrawVine(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            Rectangle brightGreenPixel = new Rectangle(10, 150, 1, 1);
            Rectangle darkGreenPixel = new Rectangle(20, 150, 1, 1);
            int stemWidth = 6;
            int stemX = Bounds.Center.X - stemWidth / 2;

            spriteBatch.Draw(blockTexture,
                new Rectangle(stemX, Bounds.Top, stemWidth, Bounds.Height),
                brightGreenPixel, Color.White);

            for (int y = Bounds.Top + 12, leafIndex = 0; y < Bounds.Bottom; y += 24, leafIndex++)
            {
                int leafY = Math.Min(y, Bounds.Bottom - 6);
                int leafX = leafIndex % 2 == 0 ? stemX - 8 : stemX + stemWidth;
                Rectangle leafSource = leafIndex % 2 == 0 ? brightGreenPixel : darkGreenPixel;
                spriteBatch.Draw(blockTexture,
                    new Rectangle(leafX, leafY, 10, 6), leafSource, Color.White);
            }
        }
    }
}
