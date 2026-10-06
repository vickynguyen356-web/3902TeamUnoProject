using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    internal class ItemFactory : IItemFactory
    {
        // stores a function that creates a sprite when called
        private readonly Func<ISprite> _createSprite;

        public ItemFactory(Func<ISprite> createSprite)
        {
            // sprite-creation function is required to create items
            ArgumentNullException.ThrowIfNull(createSprite);

            _createSprite = createSprite;
        }

        public IItem Create(ItemType type, Vector2 position)
        {
            // create requested item at the given position. each item gets a new sprite so its animation runs seperately
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
                    // Reject any type that this factory does not support.
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
