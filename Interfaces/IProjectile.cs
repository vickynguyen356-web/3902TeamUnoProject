using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    internal interface IProjectile
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

        ProjectileType Type
        {
            get;
        }

        bool IsDead
        {
            get;
        }

        bool IsEnemyProjectile
        {
            get;
        }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void ApplyMotion(Vector2 position, Vector2 velocity);
        void Kill();
    }
}
