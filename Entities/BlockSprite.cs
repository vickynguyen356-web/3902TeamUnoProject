using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.Entities
{
    internal abstract class BlockSprite
    {
        public abstract Rectangle GetSourceRectangle();

        public virtual void Update(GameTime gameTime)
        {
        }
    }

    internal class BrickBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(17, 0, 16, 16);
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
                _frameIndex = (_frameIndex + 1) % _frames.Length;
            }
        }
    }

    internal class UsedBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(51, 78, 16, 16);
        }
    }

    internal class SolidBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(51, 0, 16, 16);
        }
    }

    internal class FlagPoleBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(139, 0, 16, 166);
        }
    }

    internal class GroundBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(17, 17, 16, 16);
        }
    }

    internal class PipeBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 94, 32, 65);
        }
    }

    internal class EmptyBlockSprite : BlockSprite
    {
        public override Rectangle GetSourceRectangle()
        {
            return Rectangle.Empty;
        }
    }

    internal static class BlockSpriteFactory
    {
        public static BlockSprite Create(BlockType type)
        {
            switch (type)
            {
                case BlockType.Brick:
                    return new BrickBlockSprite();
                case BlockType.Question:
                    return new QuestionBlockSprite();
                case BlockType.Used:
                    return new UsedBlockSprite();
                case BlockType.Solid:
                    return new SolidBlockSprite();
                case BlockType.FlagPole:
                    return new FlagPoleBlockSprite();
                case BlockType.Ground:
                    return new GroundBlockSprite();
                case BlockType.Pipe:
                    return new PipeBlockSprite();
                case BlockType.Empty:
                    return new EmptyBlockSprite();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
