using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>IEnemy vs Block. 
    /// CHECKLIST:
    /// info.Side is relative to the IEnemy. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up).
    /// TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class EnemyBlockCollisionHandler : ICollisionHandler<IEnemy, Block>
    {
        public void Handle(IEnemy first, Block second, CollisionInfo info)
        {
            // TODO: Enemy update (turn around on side hit, deleted upon falling thru floor).
            // TODO: Block update.

            // Delete return upon implementing Handle(), just a placeholder for the build to run.
            return;
        }
    }
}