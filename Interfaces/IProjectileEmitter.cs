using System.Collections.Generic;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    public interface IProjectileEmitter
    {
        IReadOnlyList<IProjectile> Projectiles
        {
            get;
        }

        void ClearProjectiles();
    }
}