using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.World;

namespace TeamUno.Mario.Collision.Handlers
{
    internal class MarioItemCollisionHandler : ICollisionHandler<MarioPlayer, IItem>
    {
        private readonly GameProgress _progress;

        public MarioItemCollisionHandler(GameProgress progress)
        {
            System.ArgumentNullException.ThrowIfNull(progress);
            _progress = progress;
        }

        public void Handle(MarioPlayer player, IItem item, CollisionInfo info)
        {
            if (player.IsDead || player.IsChangingForm || item.IsExpired)
            {
                return;
            }

            switch (item.Type)
            {
                case ItemType.Mushroom:
                    // only Small Mario needs to grow from a mushroom
                    if (player.IsSmall)
                    {
                        player.BeginFormTransition(PlayerForm.Super);
                    }
                    break;
                case ItemType.FireFlower:
                    if (player.Form != PlayerForm.Fire)
                    {
                        player.BeginFormTransition(PlayerForm.Fire);
                    }
                    break;
                case ItemType.FloatingCoin:
                    _progress.AddCoin();
                    break;
                case ItemType.OneUpMushroom:
                    _progress.AddLife();
                    break;
                default:
                    // star collection still needs Mario's immunity behavior
                    // block coins are just the animation from hitting a block
                    return;
            }

            // mark the item as collected so Level can remove it
            item.Expire();
        }
    }
}
