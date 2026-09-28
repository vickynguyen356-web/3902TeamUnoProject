using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public class ItemFactory : IItemFactory
    {
        private readonly Func<ISprite> _createSprite;

        public ItemFactory(Func<ISprite> createSprite)
        {
            if (createSprite == null)
            {
                throw new ArgumentNullException(nameof(createSprite));
            }

            _createSprite = createSprite;
        }

        public IItem Create(ItemType type, Vector2 position)
        {
            switch (type)
            {
                case ItemType.Mushroom:
                    return new Mushroom(position, _createSprite());
                case ItemType.FireFlower:
                    return new FireFlower(position, _createSprite());
                case ItemType.FloatingCoin:
                    return new FloatingCoin(position, _createSprite());
                case ItemType.Star:
                    return new Star(position, _createSprite());
                case ItemType.OneUpMushroom:
                    return new OneUpMushroom(position, _createSprite());
                case ItemType.BlockCoin:
                    return new BlockCoin(position, _createSprite());
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
