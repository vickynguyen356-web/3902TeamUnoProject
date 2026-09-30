using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public abstract class BlockSpriteSheetBlock : IBlock
    {
        protected BlockSpriteSheetBlock(BlockType baseType)
        {
            Type = baseType;
        }

        public BlockType Type { get; protected set; }

        public virtual Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 0, 16, 16);
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }

    public class BrickBlockSprite : BlockSpriteSheetBlock
    {
        public BrickBlockSprite() : base(BlockType.Brick)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(17, 0, 16, 16);
        }
    }

    public class QuestionBlockSprite : BlockSpriteSheetBlock
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

    public class UsedBlockSprite : BlockSpriteSheetBlock
    {
        public UsedBlockSprite() : base(BlockType.Used)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(51, 78, 16, 16);
        }
    }

    public class SolidBlockSprite : BlockSpriteSheetBlock
    {
        public SolidBlockSprite() : base(BlockType.Solid)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(51, 0, 16, 16);
        }
    }

    public class FlagPoleBlockSprite : BlockSpriteSheetBlock
    {
        public FlagPoleBlockSprite() : base(BlockType.FlagPole)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(139, 0, 16, 166);
        }
    }

    public class GroundBlockSprite : BlockSpriteSheetBlock
    {
        public GroundBlockSprite() : base(BlockType.Ground)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(17, 17, 16, 16);
        }
    }

    public class PipeBlockSprite : BlockSpriteSheetBlock
    {
        public PipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 94, 32, 65);
        }
    }

    public class TallPipeBlockSprite : BlockSpriteSheetBlock
    {
        public TallPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(40, 94, 32, 65);
        }
    }

    public class HorizontalPipeBlockSprite : BlockSpriteSheetBlock
    {
        public HorizontalPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(91, 109, 45, 49);
        }
    }

    public class TallPipeBlockSpriteTwo : BlockSpriteSheetBlock
    {
        public TallPipeBlockSpriteTwo() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(108, 94, 32, 65);
        }
    }

    public class SmallPipeBlockSprite : BlockSpriteSheetBlock
    {
        public SmallPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(76, 126, 16, 33);
        }
    }

    public static class BlockSpriteFactory
    {
        public static IBlock Create(BlockType type)
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
