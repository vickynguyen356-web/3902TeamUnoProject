using TeamUno.Mario.Audio;
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
                    _progress.AddScore(1000);
                    // only Small Mario needs to grow from a mushroom
                    if (player.IsSmall)
                    {
                        SoundPlayer.Play("Audio/SoundEffects/powerup");
                        player.BeginFormTransition(PlayerForm.Super);
                    }
                    break;
                case ItemType.FireFlower:
                    _progress.AddScore(1000);
                    if (player.Form != PlayerForm.Fire)
                    {
                        SoundPlayer.Play("Audio/SoundEffects/powerup");
                        player.BeginFormTransition(PlayerForm.Fire);
                    }
                    break;
                case ItemType.FloatingCoin:
                    SoundPlayer.Play("Audio/SoundEffects/coin");
                    _progress.AddCoin();
                    break;
                case ItemType.OneUpMushroom:
                    SoundPlayer.Play("Audio/SoundEffects/1up");
                    _progress.AddLife();
                    _progress.AddScore(10000);
                    break;
                case ItemType.Star:
                    _progress.AddScore(1000);
                    player.ActivateStar();
                    break;
                default:
                    // block coins are just the animation from hitting a block
                    return;
            }

            // mark the item as collected so Level can remove it
            item.Expire();
        }
    }
}
