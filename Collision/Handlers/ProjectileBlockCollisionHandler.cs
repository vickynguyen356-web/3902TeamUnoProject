using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>IProjectile vs Block. 
    /// CHECKLIST:
    /// info.Side is relative to the IProjectile.
    /// Specifically for Mario projectile's, including Shell. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class ProjectileBlockCollisionHandler : ICollisionHandler<IProjectile, Block>
    {
        public void Handle(IProjectile first, Block second, CollisionInfo info)
        {
            // This receives ALL projectiles, both Mario's and enemies'.
            // Branch on first.IsEnemyProjectile and first.Type if they should behave differently.
            if (first.IsDead)
            {
                return;
            }

            // TODO: Player projectile update (bounce off or disappear, team to decide).
            // TODO: Block update.

            // Delete return upon implementing Handle(), just a placeholder for the build to run.
            return;
        }
    }
}