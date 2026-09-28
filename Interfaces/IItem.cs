using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Items;

namespace Sprint0.Interfaces
{
    public interface IItem
    {
        ItemType Type { get; }
        Vector2 Position { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Reset();
    }
}
