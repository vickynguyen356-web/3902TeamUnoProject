using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class FireFlower : Item
    // inherits from Item class
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.FireFlower;
            }
        }

        // passes the position, sprite, and fire flower animation to Item constructor to set up the item
        public FireFlower(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateFireFlowerAnimation())
        {
        }
    }
}
