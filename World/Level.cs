using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sprint0.Commands;
using Sprint0.Entities;
using Sprint0.Interfaces;

namespace Sprint0.World
{
    // The demo starts with an empty level.
    public class Level
    {
        public const int TileSize = 48;
        public const int WorldWidth = 96 * TileSize;
        public const int WorldHeight = 15 * TileSize;
        public const int GroundY = 11 * TileSize;

        private readonly LevelDefinition _levelDefinition;
        /* Lists, enemy spawn position, and current enemy indexing fields */
        private readonly List<Rectangle> _solidTiles = new List<Rectangle>();
        private readonly List<Coin> _coins = new List<Coin>();
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private readonly List<Block> _blocks = new List<Block>();
        private readonly EnemyType[] _enemyTypes = 
            { 
            EnemyType.Goomba, EnemyType.Koopa, EnemyType.PiranhaPlant, EnemyType.HammerBro, EnemyType.Bowser
            };
        private readonly IEnemyFactory _enemyFactory;
        private int _currentEnemyIndex;

        private Vector2 _enemySpawnPosition;

        public IReadOnlyList<Rectangle> SolidTiles { get; }
        public IReadOnlyList<Coin> Coins { get; }
        public IReadOnlyList<Enemy> Enemies { get; }
        public IReadOnlyList<Block> Blocks { get; }
        public Rectangle Goal { get; private set; }


        public Level(LevelDefinition levelDefinition, IEnemyFactory enemyFactory)
        {
            if (levelDefinition == null)
            {
                throw new ArgumentNullException(nameof(levelDefinition));
            }

            if (enemyFactory == null)
            {
                throw new ArgumentNullException(nameof(enemyFactory));
            }

            _levelDefinition = levelDefinition;
            _enemyFactory = enemyFactory;

            // Other classes can read these lists.
            SolidTiles = _solidTiles.AsReadOnly();
            Coins = _coins.AsReadOnly();
            Enemies = _enemies.AsReadOnly();
            Blocks = _blocks.AsReadOnly();
            Reset();
        }

        public void Reset()
        {
            _solidTiles.Clear();
            _coins.Clear();
            _enemies.Clear();
            _blocks.Clear();
            Goal = Rectangle.Empty;

            // start with first enemy type
            _currentEnemyIndex = 0;

            // Convert tile positions to pixel positions.
            foreach (PlatformDefinition platform in _levelDefinition.Platforms)
            {
                for (int tileOffset = 0; tileOffset < platform.Length; tileOffset++)
                {
                    int tilePositionX = (platform.TileX + tileOffset) * TileSize;
                    int tilePositionY = platform.TileY * TileSize;
                    Rectangle tileBounds = new Rectangle(tilePositionX, tilePositionY, TileSize, TileSize);
                    _solidTiles.Add(tileBounds);
                }
            }

            // Demo block selection for Sprint 2. These are meant to cycle with T/Y and to render as decorative obstacles.
            _blocks.Add(new Block(new Vector2(256, 420), BlockType.Brick, 48, 48));
            _blocks.Add(new Block(new Vector2(304, 420), BlockType.Question, 48, 48));
            _blocks.Add(new Block(new Vector2(352, 420), BlockType.Used, 48, 48));
            _blocks.Add(new Block(new Vector2(400, 420), BlockType.Ground, 48, 48));
            _blocks.Add(new Block(new Vector2(448, 420), BlockType.Solid, 48, 48));
            _blocks.Add(new Block(new Vector2(496, 420), BlockType.Coin, 48, 48));
            _blocks.Add(new Block(new Vector2(544, 420), BlockType.Pipe, 64, 64));

            foreach (CoinLineDefinition coinLine in _levelDefinition.CoinLines)
            {
                for (int coinIndex = 0; coinIndex < coinLine.Count; coinIndex++)
                {
                    float coinCenterX = (coinLine.TileX + coinIndex) * TileSize + TileSize / 2f;
                    float coinCenterY = coinLine.TileY * TileSize + TileSize / 2f;
                    Vector2 coinPosition = new Vector2(coinCenterX, coinCenterY);
                    _coins.Add(new Coin(coinPosition));
                }
            }

            if (_levelDefinition.Enemies.Count > 0)
            {
                EnemySpawnDefinition enemySpawn = _levelDefinition.Enemies[0];
                _enemySpawnPosition = new Vector2(enemySpawn.TileX * TileSize,
                    GroundY - Goomba.GoombaHeight);

                _enemies.Add(_enemyFactory.Create(GetCurrentEnemyType(), _enemySpawnPosition));
            }

            if (_levelDefinition.Goal.HeightInTiles > 0)
            {
                int goalHeight = _levelDefinition.Goal.HeightInTiles * TileSize;
                Goal = new Rectangle(
                    _levelDefinition.Goal.TileX * TileSize,
                    GroundY - goalHeight,
                    TileSize,
                    goalHeight);
            }

        }

        public void Update(GameTime gameTime)
        {
            // Example of how to update the enemies/items/etc. 
            foreach (Coin coin in _coins)
            {
                coin.Update(gameTime);
            }

            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(gameTime);
            }
        }

        public void PreviousEnemy()
        {
            _currentEnemyIndex--;

            // if currently at first enemy, go to last enemy in cycle
            if (_currentEnemyIndex < 0)
            {
                _currentEnemyIndex = _enemyTypes.Length - 1;
            }

            ReplaceEnemy();

        }

        public void NextEnemy()
        {
            _currentEnemyIndex++;

            // if currently at last enemy, go to first enemy in cycle
            if (_currentEnemyIndex >= _enemyTypes.Length )
            {
                _currentEnemyIndex = 0;
            }

            ReplaceEnemy();
        }

        private void ReplaceEnemy()
        {
            _enemies.Clear();

            _enemies.Add(_enemyFactory.Create(GetCurrentEnemyType(), _enemySpawnPosition));
        }

        private EnemyType GetCurrentEnemyType()
        {
            return _enemyTypes[_currentEnemyIndex];
        }

        //private void SpitFire()
        //{
            
        //}
    }
}
