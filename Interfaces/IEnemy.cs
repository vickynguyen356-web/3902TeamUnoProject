using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Interfaces
{
    internal interface IEnemy : ICollidable
    {
        bool IsDead { get; }
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void TakeDamage();
        void Kill();
    }
}
