using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public class FireFlower : Item
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.FireFlower;
            }
        }

        public FireFlower(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateFireFlowerAnimation())
        {
        }
    }
}
