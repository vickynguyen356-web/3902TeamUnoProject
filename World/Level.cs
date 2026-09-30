using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    public class Level
    {
        private readonly LevelDefinition _definition;
        private readonly IEnemyFactory _enemyFactory;
        private readonly IItemFactory _itemFactory;
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<IItem> _items = new List<IItem>();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly IReadOnlyList<Block> _readOnlyBlocks;
        private readonly IReadOnlyList<IItem> _readOnlyItems;
        private readonly IReadOnlyList<IEnemy> _readOnlyEnemies;

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

        public Level(LevelDefinition definition, IEnemyFactory enemyFactory, IItemFactory itemFactory)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (enemyFactory == null)
            {
                throw new ArgumentNullException(nameof(enemyFactory));
            }

            if (itemFactory == null)
            {
                throw new ArgumentNullException(nameof(itemFactory));
            }

            _definition = definition;
            _enemyFactory = enemyFactory;
            _itemFactory = itemFactory;
            _readOnlyBlocks = _blocks.AsReadOnly();
            _readOnlyItems = _items.AsReadOnly();
            _readOnlyEnemies = _enemies.AsReadOnly();
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
        }

        public virtual void Reset()
        {
            LoadDefinition();
        }

        protected void ReplaceBlock(int index, BlockSpawnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _blocks[index] = CreateBlock(definition);
        }

        protected void ReplaceItem(int index, ItemSpawnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _items[index] = _itemFactory.Create(definition.Type, definition.Position);
        }

        protected void ReplaceEnemy(int index, EnemySpawnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _enemies[index] = _enemyFactory.Create(definition.Type, definition.Position);
        }

        private void LoadDefinition()
        {
            _blocks.Clear();
            _items.Clear();
            _enemies.Clear();

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
    }
}
