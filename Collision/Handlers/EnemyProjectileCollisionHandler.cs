using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>IEnemy vs IProjectile. 
    /// CHECKLIST:
    /// info.Side is relative to the IEnemy. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class EnemyProjectileCollisionHandler : ICollisionHandler<IEnemy, IProjectile>
    {
        public void Handle(IEnemy first, IProjectile second, CollisionInfo info)
        {
            // NOTE: this handler receives ALL projectiles. Ignore enemy-owned projectiles.
            if (second.IsEnemyProjectile || second.IsDead)
            {
                return;
            }

            // TODO: Enemy update: most enemies die in 1 hit, Bowser takes multiple hits.
            // TODO: Projectile disappears (Kill).

            // Delete return upon implementing Handle(), just a placeholder for the build to run.
            return;
        }
    }
}