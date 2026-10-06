using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    internal interface IProjectile
    {
        Vector2 Position
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
            set;
        }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Reset();
        void Kill();
    }
}
