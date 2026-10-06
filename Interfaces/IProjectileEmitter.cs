using System.Collections.Generic;

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
