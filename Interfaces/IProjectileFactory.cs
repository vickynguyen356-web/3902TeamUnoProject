using Microsoft.Xna.Framework;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    internal interface IProjectileFactory
    {
        IProjectile Create(ProjectileType type, Vector2 position, Vector2 velocity);
    }
}
