using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<IItem> _items = new List<IItem>();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly List<IProjectile> _projectiles = new List<IProjectile>();
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

        public Level(LevelDefinition definition, IEnemyFactory enemyFactory, IItemFactory itemFactory, IProjectileFactory projectileFactory)
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

        public void Update(GameTime gameTime)
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

        public void SpawnMarioFireball(MarioPlayer source)
        {
            ArgumentNullException.ThrowIfNull(source);

            float direction = -1f;
            if (source.FacingDirection == SpriteEffects.FlipHorizontally)
            {
                direction = 1f;
            }

            Rectangle sourceBounds = source.Bounds;
            Vector2 position = new Vector2(
                sourceBounds.Right,
                sourceBounds.Center.Y - Fireball.FireballHeight / 2f);
            if (direction < 0)
            {
                position.X = sourceBounds.Left - Fireball.FireballWidth;
            }

            Vector2 velocity = new Vector2(direction * 200f, -100f);
            IProjectile fireball = _projectileFactory.Create(ProjectileType.Fireball, position, velocity);
            fireball.IsEnemyProjectile = false;
            AddProjectiles(fireball);
        }

        public void AddProjectiles(IProjectile projectile)
        {
            ArgumentNullException.ThrowIfNull(projectile);

            _projectiles.Add(projectile);
        }

        private void CollectProjectiles()
        {
            foreach (IEnemy enemy in _enemies)
            {
                IProjectileEmitter emitter = enemy as IProjectileEmitter;
                if (emitter == null)
                {
                    continue;
                }

                foreach (IProjectile projectile in emitter.Projectiles)
                {
                    AddProjectiles(projectile);
                }

                emitter.ClearProjectiles();
            }
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
