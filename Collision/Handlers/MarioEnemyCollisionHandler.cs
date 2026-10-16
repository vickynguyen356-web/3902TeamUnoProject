using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>MarioPlayer vs IEnemy. 
    /// CHECKLIST:
    /// info.Side is relative to the MarioPlayer. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class MarioEnemyCollisionHandler : ICollisionHandler<MarioPlayer, IEnemy>
    {
        public void Handle(MarioPlayer first, IEnemy second, CollisionInfo info)
        {
            // TODO: Stomp (info.Side == Bottom while falling): enemy defeated/stomped, player bounces.
            // TODO: Any other side: player takes damage.
            // TODO: Starman Mario behaves differently than other states.
            // TODO: Per-enemy differences (Koopa bounces, Goomba slight velocity decrease).

            // Delete return upon implementing Handle(), just a placeholder for the build to run.
            return;
        }
    }
}