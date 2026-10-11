using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>MarioPlayer vs IItem.
    /// CHECKLIST:
    /// info.Side is relative to the IPlayer. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class MarioItemCollisionHandler : ICollisionHandler<MarioPlayer, IItem>
    {
        public void Handle(MarioPlayer first, IItem second, CollisionInfo info)
        {
            // TODO: Item update (collected / expired), player update (apply power-up effect).

            // Delete return upon implementing Handle(), just a placeholder for the build to run.
            return;
        }
    }
}