using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;

namespace TeamUno.Mario.World
{
    internal class BlockSpawnDefinition
    {
        private readonly Vector2 _position;
        private readonly BlockType _type;
        private readonly int _width;
        private readonly int _height;

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

        public BlockSpawnDefinition(Vector2 position, BlockType type, int width = 48, int height = 48)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

            _position = position;
            _type = type;
            _width = width;
            _height = height;
        }
    }
}
