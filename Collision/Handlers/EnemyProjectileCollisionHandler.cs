using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Projectiles;
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
            if (first.IsDead || second.IsEnemyProjectile || second.IsDead)
            {
                return;
            }

            bool isFireball = second.Type == ProjectileType.Fireball;
            bool isKoopaShell = second.Type == ProjectileType.KoopaShell;
            if (!isFireball && !isKoopaShell)
            {
                return;
            }

            Bowser bowser = first as Bowser;
            if (bowser != null)
            {
                if (isFireball)
                {
                    bowser.TakeFireballDamage();
                    second.Kill();
                }

                return;
            }

            first.Kill();
            if (isFireball)
            {
                second.Kill();
            }
        }
    }
}
