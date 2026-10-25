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
        private const float BreakAnimationDurationSeconds = 0.55f;
        private const float DebrisGravity = 720f;
        private static readonly Rectangle BrickSource = new Rectangle(17, 0, 16, 16);

        private BlockSprite _sprite;
        private BlockType _type;
        private bool _isDeleted;
        private bool _isBreaking;
        private float _breakAnimationElapsed;
        private readonly Vector2[] _debrisPositions = new Vector2[4];
        private readonly Vector2[] _debrisVelocities =
        {
            new Vector2(-150f, -260f),
            new Vector2(150f, -320f),
            new Vector2(-120f, -100f),
            new Vector2(120f, -160f)
        };
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

        internal bool IsRemovalReady
        {
            get
            {
                return _isDeleted && (!_isBreaking
                    || _breakAnimationElapsed >= BreakAnimationDurationSeconds);
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
            if (_isBreaking)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float remainingSeconds = BreakAnimationDurationSeconds - _breakAnimationElapsed;
                float animationStep = Math.Min(Math.Max(0, elapsedSeconds), remainingSeconds);
                _breakAnimationElapsed += animationStep;
                for (int index = 0; index < _debrisPositions.Length; index++)
                {
                    _debrisPositions[index] += _debrisVelocities[index] * animationStep;
                    _debrisVelocities[index] += new Vector2(0, DebrisGravity * animationStep);
                }

                return;
            }

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
                _isBreaking = true;
                int fragmentWidth = Width / 2;
                int fragmentHeight = Height / 2;
                for (int index = 0; index < _debrisPositions.Length; index++)
                {
                    int column = index % 2;
                    int row = index / 2;
                    _debrisPositions[index] = new Vector2(
                        Position.X + column * fragmentWidth,
                        Position.Y + row * fragmentHeight);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            if (blockTexture == null)
            {
                return;
            }

            if (_isBreaking)
            {
                DrawBrickDebris(spriteBatch, blockTexture);
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

        private void DrawBrickDebris(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            int fragmentWidth = Width / 2;
            int fragmentHeight = Height / 2;
            float opacity = 1f - _breakAnimationElapsed / BreakAnimationDurationSeconds;

            for (int index = 0; index < _debrisPositions.Length; index++)
            {
                int column = index % 2;
                int row = index / 2;
                Rectangle source = new Rectangle(
                    BrickSource.X + column * 8,
                    BrickSource.Y + row * 8,
                    8,
                    8);
                Rectangle destination = new Rectangle(
                    (int)_debrisPositions[index].X,
                    (int)_debrisPositions[index].Y,
                    fragmentWidth,
                    fragmentHeight);
                spriteBatch.Draw(blockTexture, destination, source, Color.White * opacity);
            }
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
