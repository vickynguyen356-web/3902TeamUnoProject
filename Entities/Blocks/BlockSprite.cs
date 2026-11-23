using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.Entities.Blocks
{
    internal abstract class BlockSprite
    {
        public abstract Rectangle GetSourceRectangle();
        public virtual int FrameIndex
        {
            get
            {
                return 0;
            }
        }

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
        private readonly Rectangle[] _frames;
        private float _elapsedSeconds;
        private int _frameIndex;

        public QuestionBlockSprite(bool isUnderground, bool isUnderwater)
        {
            _frames = new[]
            {
                BlockSpriteFactory.GetQuestionBlockSource(
                    new Rectangle(298, 78, 16, 16), isUnderground, isUnderwater),
                BlockSpriteFactory.GetQuestionBlockSource(
                    new Rectangle(315, 78, 16, 16), isUnderground, isUnderwater),
                BlockSpriteFactory.GetQuestionBlockSource(
                    new Rectangle(332, 78, 16, 16), isUnderground, isUnderwater),
                BlockSpriteFactory.GetQuestionBlockSource(
                    new Rectangle(349, 78, 16, 16), isUnderground, isUnderwater)
            };
        }

        public override int FrameIndex
        {
            get
            {
                return _frameIndex;
            }
        }

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
        private const int UndergroundQuestionBlockPaletteOffset = 96;
        private const int UnderwaterQuestionBlockPaletteOffset = 288;
        private static readonly Rectangle TopBrickSource = new Rectangle(17, 16, 16, 16);
        private static readonly Rectangle BrickBodySource = new Rectangle(34, 16, 16, 16);
        private static readonly Rectangle[] PlatformSources =
        {
            new Rectangle(0, 196, 16, 16),
            new Rectangle(17, 196, 16, 16),
            new Rectangle(34, 196, 16, 16)
        };

        public static BlockSprite Create(
            BlockType type,
            bool isUnderground = false,
            bool isTopBrick = false,
            bool isUnderwater = false)
        {
            switch (type)
            {
                case BlockType.Brick:
                    Rectangle brickSource = isTopBrick ? TopBrickSource : BrickBodySource;
                    return new StaticBlockSprite(
                        GetThemedSource(brickSource, isUnderground, isUnderwater));
                case BlockType.Question:
                    return new QuestionBlockSprite(isUnderground, isUnderwater);
                case BlockType.Used:
                    return new StaticBlockSprite(GetThemedSource(
                        new Rectangle(51, 16, 16, 16), isUnderground, isUnderwater));
                case BlockType.Solid:
                    return new StaticBlockSprite(GetThemedSource(
                        new Rectangle(0, 33, 16, 16), isUnderground, isUnderwater));
                case BlockType.Invisible:
                    return new StaticBlockSprite(Rectangle.Empty);
                case BlockType.FlagPole:
                    return new StaticBlockSprite(GetThemedSource(
                        new Rectangle(0, 600, 16, 176), isUnderground, isUnderwater));
                case BlockType.Flag:
                    return new StaticBlockSprite(Rectangle.Empty);
                case BlockType.Ground:
                    return new StaticBlockSprite(GetThemedSource(
                        new Rectangle(0, 16, 16, 16), isUnderground));
                case BlockType.Pipe:
                    return new StaticBlockSprite(Rectangle.Empty);
                case BlockType.Platform:
                    return new StaticBlockSprite(GetThemedSource(
                        PlatformSources[1], isUnderground, isUnderwater));
                case BlockType.Empty:
                case BlockType.Vine:
                case BlockType.Tile:
                case BlockType.Spring:
                case BlockType.Axe:
                case BlockType.Bridge:
                    return new StaticBlockSprite(Rectangle.Empty);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        internal static Rectangle GetThemedSource(
            Rectangle source,
            bool isUnderground,
            bool isUnderwater = false)
        {
            if (isUnderwater)
            {
                source.X += 147;
                if (source.Y < 164)
                {
                    source.Y += 80;
                }
                else if (source.Y < 360)
                {
                    source.Y += 96;
                }
            }
            else if (isUnderground)
            {
                source.X += 147;
            }

            return source;
        }

        internal static Rectangle GetQuestionBlockSource(
            Rectangle source,
            bool isUnderground,
            bool isUnderwater)
        {
            if (isUnderwater)
            {
                source.X += UnderwaterQuestionBlockPaletteOffset;
            }
            else if (isUnderground)
            {
                source.X += UndergroundQuestionBlockPaletteOffset;
            }

            return source;
        }

        internal static Rectangle GetPlatformSource(
            int segment,
            bool isUnderground,
            bool isUnderwater = false)
        {
            if (segment < 0 || segment >= PlatformSources.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(segment));
            }

            return GetThemedSource(PlatformSources[segment], isUnderground, isUnderwater);
        }

        internal static Rectangle GetBrickSource(
            bool isUnderground,
            bool isTopBrick,
            bool isUnderwater = false)
        {
            return GetThemedSource(
                isTopBrick ? TopBrickSource : BrickBodySource,
                isUnderground,
                isUnderwater);
        }
    }
}
