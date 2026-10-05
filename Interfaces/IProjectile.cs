using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    public interface IProjectile
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

        bool IsDead { get; }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Reset();
    }
}
