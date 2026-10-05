using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.World
{
    internal class Level
    {
        private readonly LevelDefinition _definition;
        private readonly IEnemyFactory _enemyFactory;
        private readonly IItemFactory _itemFactory;
        private readonly IProjectileFactory _projectileFactory;
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
                return _projectiles;
            }
        }

        public Level(LevelDefinition definition,
            IEnemyFactory enemyFactory, 
            IItemFactory itemFactory,
            IProjectileFactory projectileFactory)
        {
            ArgumentNullException.ThrowIfNull(definition);

            ArgumentNullException.ThrowIfNull(enemyFactory);

            ArgumentNullException.ThrowIfNull(itemFactory);

            ArgumentNullException.ThrowIfNull(projectileFactory);

            _definition = definition;
            _enemyFactory = enemyFactory;
            _itemFactory = itemFactory;
            _projectileFactory = projectileFactory;
            _readOnlyBlocks = _blocks.AsReadOnly();
            _readOnlyItems = _items.AsReadOnly();
            _readOnlyEnemies = _enemies.AsReadOnly();
            _readOnlyProjectiles = _projectiles.AsReadOnly();

            LoadDefinition();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (IItem item in _items)
            {
                item.Update(gameTime);
            }

            foreach (IEnemy enemy in _enemies)
            {
                enemy.Update(gameTime);
            }

            foreach (Block block in _blocks)
            {
                block.Update(gameTime);
            }

            CollectProjectiles();

            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                _projectiles[i].Update(gameTime);

                if (_projectiles[i].IsDead)
                {
                    _projectiles.RemoveAt(i);
                }
            }
        }

        private void CollectProjectiles()
        {
            foreach (IEnemy enemy in _enemies)
            {
                IProjectileEmitter emitter =
                    enemy as IProjectileEmitter;

                if (emitter == null)
                {
                    continue;
                }

                foreach (IProjectile projectile
                    in emitter.Projectiles)
                {
                    _projectiles.Add(projectile);
                }

                emitter.ClearProjectiles();
            }
        }

        public virtual void Reset()
        {
            LoadDefinition();
        }

        public void SpitFire()
        {
            Bowser bowser = null;

            foreach (IEnemy enemy in _enemies)
            {
                bowser = enemy as Bowser;

                if (bowser != null)
                {
                    break;
                }
            }

            if (bowser == null)
            {
                return;
            }
        }

        protected void ReplaceBlock(int index, BlockSpawnDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _blocks[index] = CreateBlock(definition);
        }

        protected void ReplaceItem(int index, ItemSpawnDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _items[index] = _itemFactory.Create(definition.Type, definition.Position);
        }

        protected void ReplaceEnemy(int index, EnemySpawnDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _enemies[index] = _enemyFactory.Create(definition.Type, definition.Position);
        }

        private void LoadDefinition()
        {
            _blocks.Clear();
            _items.Clear();
            _enemies.Clear();
            _projectiles.Clear();

            foreach (BlockSpawnDefinition block in Definition.Blocks)
            {
                _blocks.Add(CreateBlock(block));
            }

            foreach (ItemSpawnDefinition item in Definition.Items)
            {
                _items.Add(_itemFactory.Create(item.Type, item.Position));
            }

            foreach (EnemySpawnDefinition enemy in Definition.Enemies)
            {
                _enemies.Add(_enemyFactory.Create(enemy.Type, enemy.Position));
            }
        }

        private static Block CreateBlock(BlockSpawnDefinition definition)
        {
            return new Block(definition.Position, definition.Type, definition.Width, definition.Height);
        }

        public void AddProjectiles(IProjectile projectile)
        {
            ArgumentNullException.ThrowIfNull(projectile);

            _projectiles.Add(projectile);
        }
    }
}
