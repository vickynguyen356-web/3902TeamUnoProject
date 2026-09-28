using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Interfaces
{
    public interface IEnemy
    {
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
        void TakeDamage();
    }
}
