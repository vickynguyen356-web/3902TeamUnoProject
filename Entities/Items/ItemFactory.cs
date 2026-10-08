using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal static class ItemFactory
    {
        private static Texture2D _itemTexture;

        public static void Initialize(Texture2D itemTexture)
        {
            ArgumentNullException.ThrowIfNull(itemTexture);

            _itemTexture = itemTexture;
        }

        public static IItem Create(ItemType type, Vector2 position)
        {
            // create requested item at the given position. each item gets a new sprite so its animation runs seperately
            ISprite sprite = ItemSpriteFactory.Create(_itemTexture);

            switch (type)
            {
                case ItemType.Mushroom:
                    return new Mushroom(position, sprite);
                case ItemType.FireFlower:
                    return new FireFlower(position, sprite);
                case ItemType.FloatingCoin:
                    return new FloatingCoin(position, sprite);
                case ItemType.Star:
                    return new Star(position, sprite);
                case ItemType.OneUpMushroom:
                    return new OneUpMushroom(position, sprite);
                case ItemType.BlockCoin:
                    return new BlockCoin(position, sprite);
                default:
                    // Reject any type that this factory does not support.
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
