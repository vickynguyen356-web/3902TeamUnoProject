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
        private readonly Vector2 _cameraStart;
        private readonly string _nextAreaId;
        private readonly string _levelStartAreaId;
        private readonly IReadOnlyDictionary<string, Vector2> _spawnPoints;
        private readonly IReadOnlyList<BlockSpawnDefinition> _blocks;
        private readonly IReadOnlyList<ItemSpawnDefinition> _items;
        private readonly IReadOnlyList<EnemySpawnDefinition> _enemies;
        private readonly IReadOnlyList<MovingPlatformSpawnDefinition> _movingPlatforms;
        private readonly IReadOnlyList<FireBarSpawnDefinition> _fireBars;
        private readonly IReadOnlyList<VineSpawnDefinition> _vines;
        private readonly IReadOnlyList<PlatformLevelSpawnDefinition> _platformLevels;
        private readonly IReadOnlyList<FloatingTextSpawnDefinition> _floatingTexts;
        private readonly string _musicTrack;
        private readonly bool _isUnderwater;
        private readonly bool _isUnderground;
        private readonly Color _backgroundColor;
        private readonly IReadOnlyList<LevelTile> _backgroundTiles;
        private readonly IReadOnlyList<LevelTile> _foregroundTiles;
        private readonly IReadOnlyList<LevelTile> _aboveForegroundTiles;

        public bool IsUnderwater
        {
            get
            {
                return _isUnderwater;
            }
        }

        public bool IsUnderground
        {
            get
            {
                return _isUnderground;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
        }

        public Vector2 CameraStart
        {
            get
            {
                return _cameraStart;
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

        public string MusicTrack
        {
            get
            {
                return _musicTrack;
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

        public IReadOnlyList<MovingPlatformSpawnDefinition> MovingPlatforms
        {
            get
            {
                return _movingPlatforms;
            }
        }

        public IReadOnlyList<FireBarSpawnDefinition> FireBars
        {
            get
            {
                return _fireBars;
            }
        }

        public IReadOnlyList<VineSpawnDefinition> Vines
        {
            get
            {
                return _vines;
            }
        }

        public IReadOnlyList<PlatformLevelSpawnDefinition> PlatformLevels
        {
            get
            {
                return _platformLevels;
            }
        }

        public IReadOnlyList<FloatingTextSpawnDefinition> FloatingTexts
        {
            get
            {
                return _floatingTexts;
            }
        }

        public IReadOnlyList<LevelTile> BackgroundTiles
        {
            get
            {
                return _backgroundTiles;
            }
        }

        public IReadOnlyList<LevelTile> ForegroundTiles
        {
            get
            {
                return _foregroundTiles;
            }
        }

        public IReadOnlyList<LevelTile> AboveForegroundTiles
        {
            get
            {
                return _aboveForegroundTiles;
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
            string levelStartAreaId = null,
            string musicTrack = null,
            bool isUnderwater = false,
            IReadOnlyList<MovingPlatformSpawnDefinition> movingPlatforms = null,
            IReadOnlyList<FireBarSpawnDefinition> fireBars = null,
            IReadOnlyList<PlatformLevelSpawnDefinition> platformLevels = null,
            IReadOnlyList<FloatingTextSpawnDefinition> floatingTexts = null,
            IReadOnlyList<VineSpawnDefinition> vines = null,
            bool isUnderground = false,
            Color? backgroundColor = null,
            IReadOnlyList<LevelTile> backgroundTiles = null,
            IReadOnlyList<LevelTile> foregroundTiles = null,
            IReadOnlyList<LevelTile> aboveForegroundTiles = null,
            Vector2 cameraStart = default(Vector2))
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
            _cameraStart = cameraStart;
            _nextAreaId = nextAreaId;
            _levelStartAreaId = id;
            _musicTrack = musicTrack;
            _isUnderwater = isUnderwater;
            _isUnderground = isUnderground;
            _backgroundColor = backgroundColor ?? new Color(92, 148, 252);
            _backgroundTiles = new List<LevelTile>(
                backgroundTiles ?? Array.Empty<LevelTile>()).AsReadOnly();
            _foregroundTiles = new List<LevelTile>(
                foregroundTiles ?? Array.Empty<LevelTile>()).AsReadOnly();
            _aboveForegroundTiles = new List<LevelTile>(
                aboveForegroundTiles ?? Array.Empty<LevelTile>()).AsReadOnly();
            if (levelStartAreaId != null)
            {
                _levelStartAreaId = levelStartAreaId;
            }
            _spawnPoints = new ReadOnlyDictionary<string, Vector2>(
                new Dictionary<string, Vector2>(spawnPoints));
            _blocks = new List<BlockSpawnDefinition>(blocks).AsReadOnly();
            _items = new List<ItemSpawnDefinition>(items).AsReadOnly();
            _enemies = new List<EnemySpawnDefinition>(enemies).AsReadOnly();
            _movingPlatforms = new List<MovingPlatformSpawnDefinition>(
                movingPlatforms ?? Array.Empty<MovingPlatformSpawnDefinition>()).AsReadOnly();
            _fireBars = new List<FireBarSpawnDefinition>(
                fireBars ?? Array.Empty<FireBarSpawnDefinition>()).AsReadOnly();
            _vines = new List<VineSpawnDefinition>(
                vines ?? Array.Empty<VineSpawnDefinition>()).AsReadOnly();
            _platformLevels = new List<PlatformLevelSpawnDefinition>(
                platformLevels ?? Array.Empty<PlatformLevelSpawnDefinition>()).AsReadOnly();
            _floatingTexts = new List<FloatingTextSpawnDefinition>(
                floatingTexts ?? Array.Empty<FloatingTextSpawnDefinition>()).AsReadOnly();

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

            foreach (MovingPlatformSpawnDefinition platform in _movingPlatforms)
            {
                if (platform == null)
                {
                    throw new ArgumentException(
                        "Moving platform definitions cannot contain null entries", nameof(movingPlatforms));
                }
            }

            foreach (FireBarSpawnDefinition fireBar in _fireBars)
            {
                if (fireBar == null)
                {
                    throw new ArgumentException(
                        "Fire bar definitions cannot contain null entries", nameof(fireBars));
                }
            }

            foreach (VineSpawnDefinition vine in _vines)
            {
                if (vine == null)
                {
                    throw new ArgumentException(
                        "Vine definitions cannot contain null entries", nameof(vines));
                }
            }

            foreach (PlatformLevelSpawnDefinition platformLevel in _platformLevels)
            {
                if (platformLevel == null)
                {
                    throw new ArgumentException(
                        "Platform level definitions cannot contain null entries", nameof(platformLevels));
                }
            }

            foreach (FloatingTextSpawnDefinition floatingText in _floatingTexts)
            {
                if (floatingText == null)
                {
                    throw new ArgumentException(
                        "Floating text definitions cannot contain null entries", nameof(floatingTexts));
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
