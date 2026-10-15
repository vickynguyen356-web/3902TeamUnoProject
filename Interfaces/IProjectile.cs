using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    internal interface IProjectile : ICollidable
    {
        Vector2 Position
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
        void Kill();
    }
}
