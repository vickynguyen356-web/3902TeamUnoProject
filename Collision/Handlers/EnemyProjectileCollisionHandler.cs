using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.World;

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
        private readonly GameProgress _progress;

        public EnemyProjectileCollisionHandler(GameProgress progress)
        {
            System.ArgumentNullException.ThrowIfNull(progress);
            _progress = progress;
        }

        public void Handle(IEnemy first, IProjectile second, CollisionInfo info)
        {
            if (first.IsDead || second.IsDead)
            {
                return;
            }

            KoopaShell shell = second as KoopaShell;
            if ( shell != null && shell.IsKnockOff)
            {
                return;
            }

            bool isMarioFireball = second.Type == ProjectileType.Fireball && !second.IsEnemyProjectile;
            bool isMovingShell = shell != null && shell.IsMoving;
            if (!isMarioFireball && !isMovingShell)
            {
                return;
            }

            if (first is Bowser && !isMarioFireball)
            {
                return;
            }

            if (first is Bowser)
            {
                first.TakeDamage();
                if (first.IsDead)
                {
                    _progress.AddScore(5000);
                }
            } else
            {
                first.Kill();
                _progress.AddScore(100);
            }

            if (isMarioFireball)
            {
                second.Kill();
            }
        }
    }
}
