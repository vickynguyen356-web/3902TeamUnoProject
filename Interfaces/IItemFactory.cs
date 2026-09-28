using Microsoft.Xna.Framework;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.Interfaces
{
    public interface IItemFactory
    {
        IItem Create(ItemType type, Vector2 position);
    }
}
