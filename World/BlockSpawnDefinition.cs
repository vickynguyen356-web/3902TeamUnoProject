using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.World
{
    internal class BlockSpawnDefinition
    {
        private readonly Vector2 _position;
        private readonly BlockType _type;
        private readonly int _width;
        private readonly int _height;
        private readonly ItemType? _contents;
        private readonly string _destinationArea;
        private readonly string _destinationSpawn;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public BlockType Type
        {
            get
            {
                return _type;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public ItemType? Contents
        {
            get
            {
                return _contents;
            }
        }

        public string DestinationArea
        {
            get
            {
                return _destinationArea;
            }
        }

        public string DestinationSpawn
        {
            get
            {
                return _destinationSpawn;
            }
        }

        public BlockSpawnDefinition(
            Vector2 position,
            BlockType type,
            int width = 48,
            int height = 48,
            ItemType? contents = null,
            string destinationArea = null,
            string destinationSpawn = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

            _position = position;
            _type = type;
            _width = width;
            _height = height;
            _contents = contents;
            _destinationArea = destinationArea;
            _destinationSpawn = destinationSpawn;
        }
    }
}
