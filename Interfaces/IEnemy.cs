using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.Interfaces
{
    internal interface IEnemy
    {
        Vector2 Position
        {
            get;
        }

        Vector2 CurrentVelocity
        {
            get;
        }

        Rectangle Bounds
        {
            get;
        }

        bool IsDead
        {
            get;
        }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void ApplyMotion(Vector2 position, Vector2 velocity);
        void TakeDamage();
        void Kill();
    }
}
