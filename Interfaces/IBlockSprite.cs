using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Interfaces
{
    public interface IBlockSprite
    {
        BlockType Type
        {
            get;
        }

        Rectangle GetSourceRectangle();
        void Update(GameTime gameTime);
    }
}
