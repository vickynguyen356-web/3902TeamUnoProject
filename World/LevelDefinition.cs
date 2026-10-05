using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal class LevelDefinition
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _floorY;
        private readonly Vector2 _playerSpawnPosition;
        private readonly IReadOnlyList<BlockSpawnDefinition> _blocks;
        private readonly IReadOnlyList<ItemSpawnDefinition> _items;
        private readonly IReadOnlyList<EnemySpawnDefinition> _enemies;

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

        public int FloorY
        {
            get
            {
                return _floorY;
            }
        }

        public Vector2 PlayerSpawnPosition
        {
            get
            {
                return _playerSpawnPosition;
            }
        }

        public IReadOnlyList<BlockSpawnDefinition> Blocks
        {
            get
            {
                return _blocks;
            }
        }

        public IReadOnlyList<ItemSpawnDefinition> Items
        {
            get
            {
                return _items;
            }
        }

        public IReadOnlyList<EnemySpawnDefinition> Enemies
        {
            get
            {
                return _enemies;
            }
        }

        public LevelDefinition(
            int width,
            int height,
            int floorY,
            Vector2 playerSpawnPosition,
            IReadOnlyList<BlockSpawnDefinition> blocks,
            IReadOnlyList<ItemSpawnDefinition> items,
            IReadOnlyList<EnemySpawnDefinition> enemies)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (floorY < 0 || floorY > height)
            {
                throw new ArgumentOutOfRangeException(nameof(floorY));
            }

            ArgumentNullException.ThrowIfNull(blocks);

            ArgumentNullException.ThrowIfNull(items);

            ArgumentNullException.ThrowIfNull(enemies);

            _width = width;
            _height = height;
            _floorY = floorY;
            _playerSpawnPosition = playerSpawnPosition;
            _blocks = new List<BlockSpawnDefinition>(blocks).AsReadOnly();
            _items = new List<ItemSpawnDefinition>(items).AsReadOnly();
            _enemies = new List<EnemySpawnDefinition>(enemies).AsReadOnly();

            foreach (BlockSpawnDefinition block in _blocks)
            {
                if (block == null)
                {
                    throw new ArgumentException("Block definitions cannot contain null entries", nameof(blocks));
                }
            }

            foreach (ItemSpawnDefinition item in _items)
            {
                if (item == null)
                {
                    throw new ArgumentException("Item definitions cannot contain null entries", nameof(items));
                }
            }

            foreach (EnemySpawnDefinition enemy in _enemies)
            {
                if (enemy == null)
                {
                    throw new ArgumentException("Enemy definitions cannot contain null entries", nameof(enemies));
                }
            }
        }
    }
}
