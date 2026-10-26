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
            // this handler receives ALL projectiles. Ignore enemy-owned projectiles.
            if (second.IsEnemyProjectile || second.IsDead)
            {
                return;
            }

            // enemy is already dead
            if (first.IsDead)
            {
                return;
            }

            // projectile disappears (Kill).
            second.Kill();

            // enemy is hit
            first.TakeDamage();
        }
    }
}