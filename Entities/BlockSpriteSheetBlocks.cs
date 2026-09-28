using Microsoft.Xna.Framework;
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
    }

    public class BrickBlockSprite : BlockSpriteSheetBlock
    {
        public BrickBlockSprite() : base(BlockType.Brick)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(315, 78, 16, 16);
        }
    }

    public class QuestionBlockSprite : BlockSpriteSheetBlock
    {
        public QuestionBlockSprite() : base(BlockType.Question)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(298, 78, 16, 16);
        }
    }

    public class UsedBlockSprite : BlockSpriteSheetBlock
    {
        public UsedBlockSprite() : base(BlockType.Used)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(332, 78, 16, 16);
        }
    }

    public class CoinBlockSprite : BlockSpriteSheetBlock
    {
        public CoinBlockSprite() : base(BlockType.Coin)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(298, 95, 16, 16);
        }
    }

    public class SolidBlockSprite : BlockSpriteSheetBlock
    {
        public SolidBlockSprite() : base(BlockType.Solid)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(349, 78, 16, 16);
        }
    }

    public class GroundBlockSprite : BlockSpriteSheetBlock
    {
        public GroundBlockSprite() : base(BlockType.Ground)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 48, 16, 16);
        }
    }

    public class PipeBlockSprite : BlockSpriteSheetBlock
    {
        public PipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(112, 623, 32, 65);
        }
    }

    public class TallPipeBlockSprite : BlockSpriteSheetBlock
    {
        public TallPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(152, 623, 32, 65);
        }
    }

    public class HorizontalPipeBlockSprite : BlockSpriteSheetBlock
    {
        public HorizontalPipeBlockSprite() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(192, 655, 48, 33);
        }
    }

    public class TallPipeBlockSpriteTwo : BlockSpriteSheetBlock
    {
        public TallPipeBlockSpriteTwo() : base(BlockType.Pipe)
        {
        }

        public override Rectangle GetSourceRectangle()
        {
            return new Rectangle(224, 623, 32, 65);
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
                case BlockType.Coin:
                    return new CoinBlockSprite();
                case BlockType.Solid:
                    return new SolidBlockSprite();
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
