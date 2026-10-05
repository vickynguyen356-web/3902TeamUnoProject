using Microsoft.Xna.Framework;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.World
{
    internal class ItemSpawnDefinition
    {
        private readonly ItemType _type;
        private readonly Vector2 _position;

        public ItemType Type
        {
            get
            {
                return _type;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public ItemSpawnDefinition(ItemType type, Vector2 position)
        {
            _type = type;
            _position = position;
        }
    }
}
