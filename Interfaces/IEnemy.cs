using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Interfaces
{
    internal interface IEnemy : ICollidable
    {
        Vector2 Position
        {
            get;
        }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void TakeDamage();
    }
}
