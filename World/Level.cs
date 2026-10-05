using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.World
{
    public class Level
    {
        private readonly LevelDefinition _definition;
        private readonly IEnemyFactory _enemyFactory;
        private readonly IItemFactory _itemFactory;
        private readonly IProjectileFactory _projectileFactory;

        private readonly BowserFireballSpriteFactory _fireballSpriteFactory;

        private readonly List<BowserFireball> _fireballs = new List<BowserFireball>();
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<IItem> _items = new List<IItem>();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly List<IProjectile> _projectiles = new List<IProjectile>();

        private readonly IReadOnlyList<BowserFireball> _readOnlyFireballs;
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
        
        public IReadOnlyList<BowserFireball> Fireballs
        {
            get 
            {
                return _readOnlyFireballs;
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
            BowserFireballSpriteFactory fireballSpriteFactory,
            IProjectileFactory projectileFactory)
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
            
            if (fireballSpriteFactory == null)
            {
                throw new ArgumentNullException(nameof(fireballSpriteFactory));
            }

            if (projectileFactory == null)
            {
                throw new ArgumentNullException(nameof(projectileFactory));
            }

            _definition = definition;
            _enemyFactory = enemyFactory;
            _itemFactory = itemFactory;
            _projectileFactory = projectileFactory;
            _readOnlyBlocks = _blocks.AsReadOnly();
            _readOnlyItems = _items.AsReadOnly();
            _readOnlyEnemies = _enemies.AsReadOnly();
            _readOnlyFireballs = _fireballs.AsReadOnly();
            _fireballSpriteFactory = fireballSpriteFactory;

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
        }

        public virtual void Reset()
        {
            LoadDefinition();
        }

        public void AddFireball(BowserFireball fireball)
        {
            if (fireball == null)
            {
                throw new ArgumentNullException();
            }

            _fireballs.Add(fireball);
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

            ISprite fireballSprite = _fireballSpriteFactory.Create();

            BowserFireball fireball = bowser.SpitFire(fireballSprite);

            AddFireball(fireball);
        }

        public void ThrowHammer(HammerBro hammerBro)
        {
            float direction = hammerBro.HammerThrowDirection;

            Vector2 position = new Vector2(hammerBro.Position.X + direction * HammerBro.HammerBroWidth,
                hammerBro.Position.Y);

            Vector2 velocity = new Vector2(direction * 150f,
                -180f);

            IProjectile hammer = _projectileFactory.Create(ProjectileType.Hammer, position, velocity);

            AddProjectiles(hammer);
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
            _fireballs.Clear();

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
            if (projectile == null)
            {
                throw new ArgumentNullException(nameof(projectile));
            }

            _projectiles.Add(projectile);
        }
    }
}
