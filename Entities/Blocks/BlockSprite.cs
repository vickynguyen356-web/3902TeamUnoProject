using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.Entities.Blocks
{
    internal abstract class BlockSprite
    {
        public abstract Rectangle GetSourceRectangle();

        public virtual void Update(GameTime gameTime)
        {
        }
    }

    internal class StaticBlockSprite : BlockSprite
    {
        private readonly Rectangle _sourceRectangle;

        public StaticBlockSprite(Rectangle sourceRectangle)
        {
            _sourceRectangle = sourceRectangle;
        }

        public override Rectangle GetSourceRectangle()
        {
            return _sourceRectangle;
        }
    }

    internal class QuestionBlockSprite : BlockSprite
    {
        private const float FrameDurationSeconds = 0.16f;
        private readonly Rectangle[] _frames =
        {
            new Rectangle(0, 78, 16, 16),
            new Rectangle(17, 78, 16, 16),
            new Rectangle(34, 78, 16, 16)
        };
        private float _elapsedSeconds;
        private int _frameIndex;

        public override Rectangle GetSourceRectangle()
        {
            return _frames[_frameIndex];
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedSeconds = _elapsedSeconds + (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_elapsedSeconds >= FrameDurationSeconds)
            {
                _elapsedSeconds = _elapsedSeconds - FrameDurationSeconds;
                _frameIndex = _frameIndex + 1;
                if (_frameIndex >= _frames.Length)
                {
                    _frameIndex = 0;
                }
            }
        }
    }

    internal static class BlockSpriteFactory
    {
        public static BlockSprite Create(BlockType type)
        {
            switch (type)
            {
                case BlockType.Brick:
                    return new StaticBlockSprite(new Rectangle(17, 0, 16, 16));
                case BlockType.Question:
                    return new QuestionBlockSprite();
                case BlockType.Used:
                    return new StaticBlockSprite(new Rectangle(51, 78, 16, 16));
                case BlockType.Solid:
                    return new StaticBlockSprite(new Rectangle(51, 0, 16, 16));
                case BlockType.FlagPole:
                    return new StaticBlockSprite(new Rectangle(139, 0, 16, 166));
                case BlockType.Ground:
                    return new StaticBlockSprite(new Rectangle(17, 17, 16, 16));
                case BlockType.Pipe:
                    return new StaticBlockSprite(new Rectangle(0, 94, 32, 65));
                case BlockType.Empty:
                case BlockType.Platform:
                case BlockType.Vine:
                    return new StaticBlockSprite(Rectangle.Empty);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
