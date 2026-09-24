using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class FireFlower : Item
    {
        public FireFlower(Vector2 position, IItemSprite sprite)
            : base(ItemType.FireFlower, position, sprite)
        {
        }
    }
}
