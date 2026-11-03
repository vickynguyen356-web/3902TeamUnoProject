using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    internal class ProjectileProjectileCollisionHandler : ICollisionHandler<IProjectile, IProjectile>
    {
        public void Handle(IProjectile firstProjectile, IProjectile secondProjectile, CollisionInfo info)
        {
            if (firstProjectile.IsDead || secondProjectile.IsDead)
            {
                return;
            }

            KoopaShell firstShell = firstProjectile as KoopaShell;
            KoopaShell secondShell = secondProjectile as KoopaShell;
            bool movingShellsCollided = firstShell != null && secondShell != null
                && (firstShell.IsMoving || secondShell.IsMoving);
            bool firstIsMarioFireball = firstProjectile.Type == ProjectileType.Fireball
                && !firstProjectile.IsEnemyProjectile;
            bool secondIsMarioFireball = secondProjectile.Type == ProjectileType.Fireball
                && !secondProjectile.IsEnemyProjectile;
            bool fireballHitShell = (firstIsMarioFireball && secondShell != null)
                || (secondIsMarioFireball && firstShell != null);

            if (movingShellsCollided || fireballHitShell)
            {
                firstProjectile.Kill();
                secondProjectile.Kill();
            }
        }
    }
}
