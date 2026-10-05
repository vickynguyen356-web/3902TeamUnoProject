using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.Interfaces
{
    internal interface IItem
    {
        ItemType Type
        {
            get;
        }

        Vector2 Position
        {
            get;
        }

        Rectangle Bounds
        {
            get;
        }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Reset();
    }
}
