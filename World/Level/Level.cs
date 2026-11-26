using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class Level
    {
        private LevelDefinition _definition;
        // lists for the blocks, items, enemies, and projectiles in the level
        private readonly List<IBlock> _blocks = new List<IBlock>();
        private readonly List<IItem> _items = new List<IItem>();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly List<IProjectile> _projectiles = new List<IProjectile>();
        private readonly List<PulleyPlatformPair> _pulleyPlatforms = new List<PulleyPlatformPair>();
        private readonly List<Rectangle> _pulleyRopes = new List<Rectangle>();
        // read only lists
        private readonly IReadOnlyList<IBlock> _readOnlyBlocks;
        private readonly IReadOnlyList<IItem> _readOnlyItems;
        private readonly IReadOnlyList<IEnemy> _readOnlyEnemies;
        private readonly IReadOnlyList<IProjectile> _readOnlyProjectiles;
        private readonly IReadOnlyList<Rectangle> _readOnlyPulleyRopes;
        public Vector2? ListenerPosition { get; set; }
        public Rectangle? ListenerBounds { get; set; }
        public bool HasBowser { get; private set; }
        public bool IsBowserDefeated { get; private set; }

        public IReadOnlyList<Rectangle> PulleyRopes
        {
            get
            {
                return _readOnlyPulleyRopes;
            }
        }

        public LevelDefinition Definition
        {
            get
            {
                return _definition;
            }
        }

        public IReadOnlyList<IBlock> Blocks
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
            _readOnlyPulleyRopes = _pulleyRopes.AsReadOnly();
            LoadDefinition();
        }

        public void Update(GameTime gameTime)
        {
            UpdateItems(gameTime);
            UpdateEnemies(gameTime);
            UpdateBlocks(gameTime);
            UpdatePulleyPlatforms(gameTime);
            UpdateProjectiles(gameTime);
        }

        private void UpdateItems(GameTime gameTime)
        {
            LevelItemManager.Update(gameTime, Definition, _items);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            IsBowserDefeated = LevelEnemyManager.Update(
                gameTime,
                Definition,
                ListenerPosition,
                ListenerBounds,
                _readOnlyBlocks,
                _enemies,
                _projectiles,
                HasBowser);
        }

        private void UpdateBlocks(GameTime gameTime)
        {
            for (int index = _blocks.Count - 1; index >= 0; index = index - 1)
            {
                IBlock block = _blocks[index];
                if (block.IsRemovalReady)
                {
                    _blocks.RemoveAt(index);
                }
                else
                {
                    block.Update(gameTime);
                }
            }
        }

        private void UpdatePulleyPlatforms(GameTime gameTime)
        {
            PulleyPlatformSystem.Update(
                gameTime,
                _pulleyPlatforms,
                _pulleyRopes);
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            LevelProjectileManager.Update(
                gameTime,
                Definition,
                _projectiles,
                _readOnlyBlocks,
                _enemies);
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

        internal bool IsLavaAt(Rectangle bounds)
        {
            return LevelEnvironment.IsLavaAt(
                bounds,
                Definition);
        }

        internal bool TryActivateCastleAxe(Rectangle playerBounds)
        {
            bool defeated = LevelBlockBehavior.TryActivateCastleAxe(
                HasBowser,
                IsBowserDefeated,
                playerBounds,
                _readOnlyBlocks,
                _readOnlyEnemies);
            IsBowserDefeated = defeated || IsBowserDefeated;
            return defeated;
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
            return LevelBlockBehavior.CollectItems(
                Definition,
                _blocks,
                _items);
        }

        internal VineSpawnDefinition GetVineTravelRequest()
        {
            return LevelBlockBehavior.GetVineTravelRequest(_readOnlyBlocks);
        }

        internal void CarryPlayerWithMovingPlatforms(
            MarioPlayer player,
            Rectangle previousPlayerBounds)
        {
            LevelBlockBehavior.CarryPlayerWithMovingPlatforms(
                _readOnlyBlocks,
                player,
                previousPlayerBounds);
        }

        internal void BeginFlagpoleSequence()
        {
            LevelBlockBehavior.BeginFlagpoleSequence(_readOnlyBlocks);
        }

        internal void UpdateFlagpoleFlags(GameTime gameTime)
        {
            LevelBlockBehavior.UpdateFlagpoleFlags(_readOnlyBlocks, gameTime);
        }

        internal bool ResolveLevelFinishWalk(MarioPlayer player, Rectangle previousBounds)
        {
            return LevelBlockBehavior.ResolveLevelFinishWalk(
                Definition,
                _readOnlyBlocks,
                player,
                previousBounds);
        }

        public IBlock GetPipeTravelRequest()
        {
            return LevelBlockBehavior.GetPipeTravelRequest(_blocks);
        }

        private void LoadDefinition()
        {
            HasBowser = LevelLoader.Load(
                Definition,
                _blocks,
                _items,
                _enemies,
                _projectiles,
                _pulleyPlatforms,
                _pulleyRopes,
                _readOnlyBlocks);
            IsBowserDefeated = false;
        }

        internal void ClearEnemies()
        {
            _enemies.Clear();

            // remove any enemy projectiles
            _projectiles.RemoveAll(projectile => projectile.IsEnemyProjectile ||
            projectile is KoopaShell);
        }
    }
}
