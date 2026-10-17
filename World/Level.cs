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
        private readonly LevelDefinition _definition;
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

                if (item.IsExpired)
                {
                    _items.RemoveAt(index);
                }
            }

            foreach (IEnemy enemy in _enemies)
            {
                enemy.Update(gameTime);
                IProjectileEmitter emitter = enemy as IProjectileEmitter;
                if (emitter != null)
                {
                    CollectProjectiles(emitter);
                }
            }

            foreach (Block block in _blocks)
            {
                block.Update(gameTime);
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

        public void Reset()
        {
            LoadDefinition();
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
                    blockDefinition.Height);
                _blocks.Add(block);
            }

            foreach (ItemSpawnDefinition item in Definition.Items)
            {
                _items.Add(ItemFactory.Create(item.Type, item.Position));
            }

            foreach (EnemySpawnDefinition enemy in Definition.Enemies)
            {
                _enemies.Add(EnemyFactory.Create(enemy.Type, enemy.Position));
            }
        }
    }
}
