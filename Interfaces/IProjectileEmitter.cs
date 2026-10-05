using System.Collections.Generic;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.Interfaces
{
    internal interface IProjectileEmitter
    {
        IReadOnlyList<IProjectile> Projectiles
        {
            get;
        }

        void ClearProjectiles();
    }
}