using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Entities;

namespace Sprint0.Interfaces
{
    public interface IBlock
    {
        BlockType Type { get; }
        Rectangle GetSourceRectangle();
        void Update(GameTime gameTime);
    }
}
