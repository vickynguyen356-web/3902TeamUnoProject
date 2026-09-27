using Microsoft.Xna.Framework;
using Sprint0.Entities;

namespace Sprint0.Interfaces
{
    public interface IBlock
    {
        BlockType Type { get; }
        Rectangle GetSourceRectangle();
    }
}
