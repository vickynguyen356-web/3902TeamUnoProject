using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class OneUpMushroom : Mushroom
    {
        public OneUpMushroom(Vector2 position, IItemSprite sprite)
            : base(ItemType.OneUpMushroom, position, sprite)
        {
        }
    }
}
