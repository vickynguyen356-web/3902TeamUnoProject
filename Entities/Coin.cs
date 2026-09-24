using Microsoft.Xna.Framework;
using Sprint0.Interfaces;
using Sprint0.Items;

namespace Sprint0.Entities
{
    public class Coin : Item
    {
        public Coin(Vector2 position)
            : this(position, ItemSpriteFactory.Instance.CreateCoinSprite())
        {
        }

        public Coin(Vector2 position, IItemSprite sprite)
            : base(ItemType.Coin, position, sprite)
        {
        }
    }
}
