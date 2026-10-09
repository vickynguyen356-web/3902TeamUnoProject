using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class FloatingCoin : Item
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.FloatingCoin;
            }
        }

        public FloatingCoin(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateFloatingCoinAnimation(), 16, 32)
        {
        }
    }
}
