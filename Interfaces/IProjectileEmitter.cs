using System.Collections.Generic;

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
