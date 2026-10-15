using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>IItem vs Block. 
    /// CHECKLIST:
    /// info.Side is relative to the IItem. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class ItemBlockCollisionHandler : ICollisionHandler<IItem, Block>
    {
        public void Handle(IItem first, Block second, CollisionInfo info)
        {
            // TODO: Item update (land on block, turn around at walls).
            // TODO: Block update.

            // Delete return upon implementing Handle(), just a placeholder for the build to run.
            return;
        }
    }
}