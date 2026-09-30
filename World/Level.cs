using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Entities;

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
        private readonly Texture2D _goombaTexture;
        private readonly List<Rectangle> _solidTiles = new List<Rectangle>();
        private readonly List<Coin> _coins = new List<Coin>();
        private readonly List<Goomba> _enemies = new List<Goomba>();
        private readonly List<Block> _blocks = new List<Block>();

        public IReadOnlyList<Rectangle> SolidTiles { get; }
        public IReadOnlyList<Coin> Coins { get; }
        public IReadOnlyList<Goomba> Enemies { get; }
        public IReadOnlyList<Block> Blocks { get; }
        public Rectangle Goal { get; private set; }

        public Level() : this(LevelDefinition.CreateDefault(), null)
        {
        }

        public Level(Texture2D goombaTexture) : this(LevelDefinition.CreateDefault(), goombaTexture)
        {
        }

        public Level(LevelDefinition levelDefinition, Texture2D goombaTexture = null)
        {
            if (levelDefinition == null)
            {
                throw new ArgumentNullException(nameof(levelDefinition));
            }

            _levelDefinition = levelDefinition;
            _goombaTexture = goombaTexture;

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
            _blocks.Add(new Block(new Vector2(256, 384), BlockType.Brick));
            _blocks.Add(new Block(new Vector2(304, 384), BlockType.Question));
            _blocks.Add(new Block(new Vector2(352, 384), BlockType.Question));
            _blocks.Add(new Block(new Vector2(400, 384), BlockType.Question));
            _blocks.Add(new Block(new Vector2(448, 384), BlockType.Used));
            _blocks.Add(new Block(new Vector2(496, 384), BlockType.Ground));
            _blocks.Add(new Block(new Vector2(544, 384), BlockType.Solid));
            _blocks.Add(new Block(new Vector2(592, 384), BlockType.Pipe, 64, 64));
            _blocks.Add(new Block(new Vector2(645, 30), BlockType.FlagPole, 48, 498));

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

            if (_goombaTexture != null)
            {
                foreach (EnemySpawnDefinition enemySpawn in _levelDefinition.Enemies)
                {
                    Vector2 spawnPosition = new Vector2(enemySpawn.TileX * TileSize, GroundY - Goomba.GoombaHeight);
                    _enemies.Add(new Goomba(GoombaSpriteFactory.Create(_goombaTexture), spawnPosition));
                }
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

            foreach (Goomba enemy in _enemies)
            {
                enemy.Update(gameTime);
            }

            foreach (Block block in _blocks)
            {
                block.Update(gameTime);
            }
        }
    }
}
