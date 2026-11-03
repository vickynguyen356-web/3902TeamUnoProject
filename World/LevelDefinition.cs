using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.World
{
    internal class LevelDefinition
    {
        private readonly int _width;
        private readonly int _height;
        private readonly string _id;
        private readonly string _nextAreaId;
        private readonly string _levelStartAreaId;
        private readonly IReadOnlyDictionary<string, Vector2> _spawnPoints;
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

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public string NextAreaId
        {
            get
            {
                return _nextAreaId;
            }
        }

        public string LevelStartAreaId
        {
            get
            {
                return _levelStartAreaId;
            }
        }

        public IReadOnlyDictionary<string, Vector2> SpawnPoints
        {
            get
            {
                return _spawnPoints;
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
            string id,
            int width,
            int height,
            IReadOnlyDictionary<string, Vector2> spawnPoints,
            IReadOnlyList<BlockSpawnDefinition> blocks,
            IReadOnlyList<ItemSpawnDefinition> items,
            IReadOnlyList<EnemySpawnDefinition> enemies,
            string nextAreaId = null,
            string levelStartAreaId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentNullException.ThrowIfNull(spawnPoints);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
            ArgumentNullException.ThrowIfNull(blocks);
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(enemies);

            _width = width;
            _height = height;
            _id = id;
            _nextAreaId = nextAreaId;
            _levelStartAreaId = id;
            if (levelStartAreaId != null)
            {
                _levelStartAreaId = levelStartAreaId;
            }
            _spawnPoints = new ReadOnlyDictionary<string, Vector2>(
                new Dictionary<string, Vector2>(spawnPoints));
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

        public Vector2 GetSpawnPoint(string id)
        {
            Vector2 position;
            if (!_spawnPoints.TryGetValue(id, out position))
            {
                throw new KeyNotFoundException("Area '" + Id + "' has no spawn named '" + id + "'");
            }

            return position;
        }
    }
}
