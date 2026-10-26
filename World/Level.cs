using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class Level
    {
        private LevelDefinition _definition;
        // lists for the blocks, items, enemies, and projectiles in the level
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<IItem> _items = new List<IItem>();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly List<IProjectile> _projectiles = new List<IProjectile>();
        // read only lists
        private readonly IReadOnlyList<Block> _readOnlyBlocks;
        private readonly IReadOnlyList<IItem> _readOnlyItems;
        private readonly IReadOnlyList<IEnemy> _readOnlyEnemies;
        private readonly IReadOnlyList<IProjectile> _readOnlyProjectiles;

        public LevelDefinition Definition
        {
            get
            {
                return _definition;
            }
        }

        public IReadOnlyList<Block> Blocks
        {
            get
            {
                return _readOnlyBlocks;
            }
        }

        public IReadOnlyList<IItem> Items
        {
            get
            {
                return _readOnlyItems;
            }
        }

        public IReadOnlyList<IEnemy> Enemies
        {
            get
            {
                return _readOnlyEnemies;
            }
        }

        public IReadOnlyList<IProjectile> Projectiles
        {
            get
            {
                return _readOnlyProjectiles;
            }
        }

        public Level(LevelDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _definition = definition;
            _readOnlyBlocks = _blocks.AsReadOnly();
            _readOnlyItems = _items.AsReadOnly();
            _readOnlyEnemies = _enemies.AsReadOnly();
            _readOnlyProjectiles = _projectiles.AsReadOnly();
            LoadDefinition();
        }

        public void Update(GameTime gameTime)
        {
            for (int index = _items.Count - 1; index >= 0; index = index - 1)
            {
                IItem item = _items[index];
                if (!item.IsExpired)
                {
                    item.Update(gameTime);
                }

                if (item.Bounds.Top > Definition.Height
                    || item.Bounds.Right < 0 || item.Bounds.Left > Definition.Width)
                {
                    item.Expire();
                }

                if (item.IsExpired)
                {
                    _items.RemoveAt(index);
                }
            }

            for (int index = _enemies.Count - 1; index >= 0; index = index - 1)
            {
                IEnemy enemy = _enemies[index];
                if (enemy.IsDead)
                {
                    _enemies.RemoveAt(index);
                    continue;
                }

                enemy.Update(gameTime);
                Enemy movingEnemy = enemy as Enemy;
                if (movingEnemy != null)
                {
                    movingEnemy.UpdateGravity(gameTime);
                }
                if (enemy.Bounds.Top >= Definition.Height)
                {
                    enemy.Kill();
                }

                if (enemy.IsDead)
                {
                    _enemies.RemoveAt(index);
                    continue;
                }

                IProjectileEmitter emitter = enemy as IProjectileEmitter;
                if (emitter != null)
                {
                    CollectProjectiles(emitter);
                }
            }

            for (int index = _blocks.Count - 1; index >= 0; index = index - 1)
            {
                Block block = _blocks[index];
                if (block.IsRemovalReady)
                {
                    _blocks.RemoveAt(index);
                }
                else
                {
                    block.Update(gameTime);
                }
            }

            for (int index = _projectiles.Count - 1; index >= 0; index = index - 1)
            {
                IProjectile projectile = _projectiles[index];
                if (!projectile.IsDead)
                {
                    projectile.Update(gameTime);
                }

                Rectangle bounds = projectile.Bounds;
                if (bounds.Right < 0 || bounds.Left > Definition.Width || bounds.Top > Definition.Height)
                {
                    projectile.Kill();
                }

                if (projectile.IsDead)
                {
                    _projectiles.RemoveAt(index);
                }
            }
        }

        public void AddProjectile(IProjectile projectile)
        {
            ArgumentNullException.ThrowIfNull(projectile);

            _projectiles.Add(projectile);
        }

        public void CollectProjectiles(IProjectileEmitter emitter)
        {
            foreach (IProjectile projectile in emitter.Projectiles)
            {
                AddProjectile(projectile);
            }

            emitter.ClearProjectiles();
        }

        internal void RemoveDeletedBlocks()
        {
            for (int index = _blocks.Count - 1; index >= 0; index = index - 1)
            {
                if (_blocks[index].IsRemovalReady)
                {
                    _blocks.RemoveAt(index);
                }
            }
        }

        public void Reset()
        {
            LoadDefinition();
        }

        public void LoadArea(LevelDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);
            _definition = definition;
            LoadDefinition();
        }

        public int CollectBlockItems()
        {
            int coinsCollected = 0;
            foreach (Block block in _blocks)
            {
                ItemType? contents = block.CollectItem();
                if (!contents.HasValue)
                {
                    continue;
                }

                IItem item = ItemFactory.Create(contents.Value, block.Position);
                Vector2 position = new Vector2(
                    block.Bounds.Center.X - item.Bounds.Width / 2f,
                    block.Bounds.Top - item.Bounds.Height);
                item.ApplyMotion(position, item.CurrentVelocity);
                _items.Add(item);
                if (contents.Value == ItemType.BlockCoin)
                {
                    coinsCollected = coinsCollected + 1;
                }
            }

            return coinsCollected;
        }

        public Block GetPipeTravelRequest()
        {
            foreach (Block block in _blocks)
            {
                if (block.IsTravelRequested)
                {
                    block.ClearTravelRequest();
                    return block;
                }
            }

            return null;
        }

        private void LoadDefinition()
        {
            _blocks.Clear();
            _items.Clear();
            _enemies.Clear();
            _projectiles.Clear();

            foreach (BlockSpawnDefinition blockDefinition in Definition.Blocks)
            {
                Block block = new Block(
                    blockDefinition.Position,
                    blockDefinition.Type,
                    blockDefinition.Width,
                    blockDefinition.Height,
                    blockDefinition.Contents,
                    blockDefinition.DestinationArea,
                    blockDefinition.DestinationSpawn);
                _blocks.Add(block);
            }

            foreach (ItemSpawnDefinition item in Definition.Items)
            {
                _items.Add(ItemFactory.Create(item.Type, item.Position));
            }

            foreach (EnemySpawnDefinition enemy in Definition.Enemies)
            {
                IEnemy newEnemy = EnemyFactory.Create(enemy.Type, enemy.Position);
                Enemy walkingEnemy = newEnemy as Enemy;
                if (walkingEnemy != null)
                {
                    walkingEnemy.SetWorldBlocks(_readOnlyBlocks);
                }
                _enemies.Add(newEnemy);
            }
        }
    }
}
