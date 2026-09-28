using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.Entities
{
    public class FloatingCoin : Item
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.FloatingCoin;
            }
        }

        public FloatingCoin(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateFloatingCoinAnimation())
        {
        }
    }
}
