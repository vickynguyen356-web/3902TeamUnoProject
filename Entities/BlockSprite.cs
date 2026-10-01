using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public abstract class BlockSprite : IBlockSprite
    {
        private BlockType _type;

        protected BlockSprite(BlockType baseType)
        {
            Type = baseType;
        }

        public BlockType Type
        {
            get
            {
                return _type;
            }
            protected set
            {
                _type = value;
            }
        }

        public virtual Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 0, 16, 16);
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }

    public class BrickBlockSprite : BlockSprite
    {
        public BrickBlockSprite() : base(BlockType.Brick)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(17, 0, 16, 16);
        }
    }

    public class QuestionBlockSprite : BlockSprite
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

        public QuestionBlockSprite() : base(BlockType.Question)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return _frames[_frameIndex];
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_elapsedSeconds >= FrameDurationSeconds)
            {
                _elapsedSeconds -= FrameDurationSeconds;
                _frameIndex = (_frameIndex + 1) % _frames.Length;
            }
        }
    }

    public class UsedBlockSprite : BlockSprite
    {
        public UsedBlockSprite() : base(BlockType.Used)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(332, 78, 16, 16);
        }
    }

    public class SolidBlockSprite : BlockSprite
    {
        public SolidBlockSprite() : base(BlockType.Solid)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(51, 0, 16, 16);
        }
    }

    public class FlagPoleBlockSprite : BlockSprite
    {
        public FlagPoleBlockSprite() : base(BlockType.FlagPole)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(139, 0, 16, 166);
        }
    }

    public class GroundBlockSprite : BlockSprite
    {
        public GroundBlockSprite() : base(BlockType.Ground)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(17, 17, 16, 16);
        }
    }

    public class PipeBlockSprite : BlockSprite
    {
        public PipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 94, 32, 65);
        }
    }

    public class TallPipeBlockSprite : BlockSprite
    {
        public TallPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(40, 94, 32, 65);
        }
    }

    public class HorizontalPipeBlockSprite : BlockSprite
    {
        public HorizontalPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(91, 109, 45, 49);
        }
    }

    public class TallPipeBlockSpriteTwo : BlockSprite
    {
        public TallPipeBlockSpriteTwo() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(108, 94, 32, 65);
        }
    }

    public static class BlockSpriteFactory
    {
        public static IBlockSprite Create(BlockType type)
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
                default:
                    return new BrickBlockSprite();
            }
        }
    }
}
