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
        private const int TileSize = 48;
        private LevelDefinition _definition;
        // lists for the blocks, items, enemies, and projectiles in the level
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<IItem> _items = new List<IItem>();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly List<IProjectile> _projectiles = new List<IProjectile>();
        private readonly List<PulleyPlatformPair> _pulleyPlatforms = new List<PulleyPlatformPair>();
        private readonly List<Rectangle> _pulleyRopes = new List<Rectangle>();
        // read only lists
        private readonly IReadOnlyList<Block> _readOnlyBlocks;
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
            _readOnlyPulleyRopes = _pulleyRopes.AsReadOnly();
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
                Enemy movingEnemy = enemy as Enemy;

                if (enemy.IsDead)
                {
                    if (movingEnemy != null)
                    {
                        movingEnemy.UpdateDeathFall(gameTime);
                    }

                    if (movingEnemy == null || enemy.Bounds.Top >= Definition.Height)
                    {
                        _enemies.RemoveAt(index);
                        continue;
                    }
            }

                Bowser bowser = enemy as Bowser;
                if (bowser != null)
                {
                    bowser.ListenerPosition = ListenerPosition;
                }

                Blooper blooper = enemy as Blooper;
                if (blooper != null)
                {
                    blooper.ListenerPosition = ListenerPosition;
                }

                Lakitu lakitu = enemy as Lakitu;
                if (lakitu != null)
                {
                    lakitu.ListenerPosition = ListenerPosition;
                    lakitu.FlagPosition = FindFlagPosition();
                }

                PiranhaPlant piranha = enemy as PiranhaPlant;
                if (piranha != null)
                {
                    piranha.PlayerBounds = ListenerBounds;
                }

                BulletBillCannon cannon = enemy as BulletBillCannon;
                if (cannon != null)
                {
                    cannon.ListenerPosition = ListenerPosition;
                }

                enemy.Update(gameTime);

                if (lakitu != null)
                {
                    foreach (Spiny spiny in lakitu.DrainSpinySpawns())
                    {
                        spiny.SetWorldBlocks(_readOnlyBlocks);
                        _enemies.Add(spiny);
                    }

                    if (lakitu.ShouldDespawn)
                    {
                        _enemies.RemoveAt(index);
                        continue;
                    }
                }

                if (cannon != null)
                {
                    foreach (BulletBill bulletBill in cannon.DrainSpawns())
                    {
                        bulletBill.SetWorldBlocks(_readOnlyBlocks);
                        _enemies.Add(bulletBill);
                    }
                }

                if (movingEnemy != null)
                {
                    movingEnemy.UpdateGravity(gameTime);
                }

                if (enemy.Bounds.Top >= Definition.Height)
                {
                    // enemy fell off map while alive, remove directly
                    _enemies.RemoveAt(index);
                    continue;
                }

                IProjectileEmitter emitter = enemy as IProjectileEmitter;
                if (emitter != null)
                {
                    CollectProjectiles(emitter);
                }
            }

            if (HasBowser)
            {
                IsBowserDefeated = true;
                foreach (IEnemy enemy in _enemies)
                {
                    if (enemy is Bowser && !enemy.IsDead)
                    {
                        IsBowserDefeated = false;
                        break;
                    }
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

            foreach (PulleyPlatformPair platform in _pulleyPlatforms)
            {
                platform.Update(gameTime);
            }
            UpdatePulleyRopes();

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

                KoopaShell shell = projectile as KoopaShell;
                if (shell != null && shell.ShouldRevive)
                {
                    TryReviveShell(shell);
                }

                if (projectile.IsDead)
                {
                    _projectiles.RemoveAt(index);
                }
            }
        }

        private void TryReviveShell(KoopaShell shell)
        {
            float x = shell.Position.X + (KoopaShell.ShellWidth - Koopa.KoopaWidth) / 2f;
            float y = shell.Position.Y + KoopaShell.ShellHeight - Koopa.KoopaHeight;
            Rectangle spawnBounds = new Rectangle((int)x, (int)y, Koopa.KoopaWidth, Koopa.KoopaHeight);

            // the Koopa is taller than the shell, so don't spawn it inside a ceiling or wall
            foreach (Block block in _blocks)
            {
                if (block.IsDeleted || block.Type == BlockType.Empty || block.Type == BlockType.Invisible
                    || block.Type == BlockType.Vine || block.Type == BlockType.FlagPole
                    || block.Type == BlockType.Flag
                    || block.Type == BlockType.Platform)
                {
                    continue;
                }

                if (spawnBounds.Intersects(block.Bounds))
                {
                    return;   // try again next frame
                }
            }

            IEnemy newEnemy = EnemyFactory.Create(EnemyType.Koopa, new Vector2(x, y));   // match your enum name
            Enemy walkingEnemy = newEnemy as Enemy;
            if (walkingEnemy != null)
            {
                walkingEnemy.SetWorldBlocks(_readOnlyBlocks);
            }

            _enemies.Add(newEnemy);
            shell.Kill();   // silent removal, then the loop above cleans it up
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
            return ContainsLava(bounds, Definition.BackgroundTiles)
                || ContainsLava(bounds, Definition.ForegroundTiles)
                || ContainsLava(bounds, Definition.AboveForegroundTiles);
        }

        private static bool ContainsLava(Rectangle bounds, IReadOnlyList<LevelTile> tiles)
        {
            foreach (LevelTile tile in tiles)
            {
                if ((tile.TileId == 634 || tile.TileId == 682) && bounds.Intersects(tile.Bounds))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool TryActivateCastleAxe(Rectangle playerBounds)
        {
            if (!HasBowser || IsBowserDefeated)
            {
                return false;
            }

            Block axe = null;
            foreach (Block block in _blocks)
            {
                if (!block.IsDeleted && block.Type == BlockType.Axe
                    && block.Bounds.Intersects(playerBounds))
                {
                    axe = block;
                    break;
                }
            }

            if (axe == null)
            {
                return false;
            }

            axe.MarkDeleted();
            foreach (Block block in _blocks)
            {
                if (block.Type == BlockType.Bridge)
                {
                    block.MarkDeleted();
                }
            }

            bool defeatedBowser = false;
            foreach (IEnemy enemy in _enemies)
            {
                Bowser bowser = enemy as Bowser;
                if (bowser == null || bowser.IsDead)
                {
                    continue;
                }

                while (!bowser.IsDead)
                {
                    bowser.TakeDamage();
                }

                defeatedBowser = true;
            }

            IsBowserDefeated = true;
            return defeatedBowser;
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
            List<Block> newVines = new List<Block>();
            foreach (Block block in _blocks)
            {
                ItemType? contents = block.CollectItem();
                if (!contents.HasValue)
                {
                    continue;
                }

                if (contents.Value == ItemType.Vine)
                {
                    VineSpawnDefinition vine = FindVineSpawn(block.Position);
                    if (vine == null)
                    {
                        throw new InvalidOperationException(
                            "A vine block was hit without matching VINE metadata in level '"
                            + Definition.Id + "'.");
                    }

                    newVines.Add(new Block(
                        block.Position,
                        BlockType.Vine,
                        width: TileSize,
                        height: TileSize,
                        isUnderground: block.IsUnderground,
                        isUnderwater: Definition.IsUnderwater,
                        vineSpawn: vine));
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

            _blocks.AddRange(newVines);
            return coinsCollected;
        }

        private VineSpawnDefinition FindVineSpawn(Vector2 position)
        {
            foreach (VineSpawnDefinition vine in Definition.Vines)
            {
                if (vine.Position == position)
                {
                    return vine;
                }
            }

            return null;
        }

        internal VineSpawnDefinition GetVineTravelRequest()
        {
            foreach (Block block in _blocks)
            {
                VineSpawnDefinition vine = block.ConsumeVineTravelRequest();
                if (vine != null)
                {
                    return vine;
                }
            }

            return null;
        }

        internal void CarryPlayerWithMovingPlatforms(
            MarioPlayer player,
            Rectangle previousPlayerBounds)
        {
            if (player.IsDead || player.Velocity.Y < 0f)
            {
                return;
            }

            foreach (Block block in _blocks)
            {
                Vector2 movement = block.MovementDelta;
                if (block.Type != BlockType.Platform
                    || (movement.X == 0f && movement.Y == 0f)
                    || Math.Abs(movement.X) > TileSize
                    || Math.Abs(movement.Y) > TileSize)
                {
                    continue;
                }

                Rectangle previousPlatformBounds = block.PreviousBounds;
                bool stoodOnPlatform = previousPlayerBounds.Bottom
                        >= previousPlatformBounds.Top - 1
                    && previousPlayerBounds.Bottom <= previousPlatformBounds.Top + 2
                    && previousPlayerBounds.Right > previousPlatformBounds.Left
                    && previousPlayerBounds.Left < previousPlatformBounds.Right;
                if (!stoodOnPlatform)
                {
                    continue;
                }

                Vector2 position = player.Position + movement;
                position.Y = block.Bounds.Top - player.Bounds.Height;
                Vector2 velocity = player.Velocity;
                velocity.Y = 0f;
                player.ApplyMotion(position, velocity, isGrounded: true);
                return;
            }
        }

        internal void BeginFlagpoleSequence()
        {
            foreach (Block flag in _blocks)
            {
                if (flag.Type != BlockType.Flag)
                {
                    continue;
                }

                Block closestPole = null;
                int closestDistance = int.MaxValue;
                foreach (Block pole in _blocks)
                {
                    if (pole.Type != BlockType.FlagPole)
                    {
                        continue;
                    }

                    int distance = Math.Abs(flag.Bounds.Center.X - pole.Bounds.Center.X);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPole = pole;
                    }
                }

                if (closestPole != null)
                {
                    flag.BeginFlagpoleDescent(closestPole.Bounds.Bottom);
                }
            }
        }

        internal void UpdateFlagpoleFlags(GameTime gameTime)
        {
            float elapsedSeconds = Math.Min(
                1f / 30f, Math.Max(0f, (float)gameTime.ElapsedGameTime.TotalSeconds));
            foreach (Block block in _blocks)
            {
                if (block.Type == BlockType.Flag)
                {
                    block.UpdateFlagpoleDescent(elapsedSeconds);
                }
            }
        }

        internal bool ResolveLevelFinishWalk(MarioPlayer player, Rectangle previousBounds)
        {
            Rectangle currentBounds = player.Bounds;
            foreach (Block block in _blocks)
            {
                if (!IsLevelFinishSolid(block))
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                bool crossedWall = previousBounds.Right <= blockBounds.Left
                    && currentBounds.Right >= blockBounds.Left
                    && currentBounds.Bottom > blockBounds.Top
                    && currentBounds.Top < blockBounds.Bottom;
                if (!crossedWall)
                {
                    continue;
                }

                Vector2 position = player.Position;
                position.X = blockBounds.Left - player.Bounds.Width;
                Vector2 velocity = player.Velocity;
                velocity.X = 0f;
                player.ApplyMotion(position, velocity, player.IsGrounded);
                return true;
            }

            Block support = null;
            int smallestGap = TileSize + 1;
            foreach (Block block in _blocks)
            {
                if (!IsLevelFinishSolid(block) && block.Type != BlockType.Platform)
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                int gap = blockBounds.Top - currentBounds.Bottom;
                bool horizontalSupport = currentBounds.Right > blockBounds.Left
                    && currentBounds.Left < blockBounds.Right;
                if (horizontalSupport && gap >= -1 && gap <= TileSize && gap < smallestGap)
                {
                    smallestGap = Math.Max(0, gap);
                    support = block;
                }
            }

            if (support != null)
            {
                Vector2 position = player.Position;
                position.Y = support.Bounds.Top - player.Bounds.Height;
                Vector2 velocity = player.Velocity;
                velocity.Y = 0f;
                player.ApplyMotion(position, velocity, isGrounded: true);
            }

            if (player.Bounds.Right >= Definition.Width)
            {
                Vector2 position = player.Position;
                position.X = Definition.Width - player.Bounds.Width;
                player.ApplyMotion(position, Vector2.Zero, isGrounded: true);
                return true;
            }

            return false;
        }

        private static bool IsLevelFinishSolid(Block block)
        {
            return !block.IsDeleted
                && block.Type != BlockType.Empty
                && block.Type != BlockType.Invisible
                && block.Type != BlockType.Vine
                && block.Type != BlockType.FlagPole
                && block.Type != BlockType.Flag
                && block.Type != BlockType.Platform;
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

        private Vector2? FindFlagPosition()
        {
            foreach (Block block in _blocks)
            {
                if (block.Type == BlockType.Flag)
                {
                    return new Vector2(block.Bounds.Center.X, block.Bounds.Center.Y);
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
            _pulleyPlatforms.Clear();
            _pulleyRopes.Clear();
            HasBowser = false;
            IsBowserDefeated = false;

            foreach (BlockSpawnDefinition blockDefinition in Definition.Blocks)
            {
                Block block = new Block(
                    blockDefinition.Position,
                    blockDefinition.Type,
                    blockDefinition.Width,
                    blockDefinition.Height,
                    blockDefinition.Contents,
                    blockDefinition.DestinationArea,
                    blockDefinition.DestinationSpawn,
                    blockDefinition.DestinationPosition,
                    blockDefinition.EntryDirection,
                    blockDefinition.DestinationCameraPosition,
                    blockDefinition.FreezeCamera,
                    blockDefinition.IsUnderground,
                    blockDefinition.IsTopBrick,
                    blockDefinition.IsHorizontalPipeAttachment,
                    blockDefinition.ExitDirection,
                    blockDefinition.DestinationBackgroundColor,
                    blockDefinition.DestinationIsUnderwater,
                    blockDefinition.DestinationMusicTrack,
                    tileId: blockDefinition.TileId,
                    usedTileId: blockDefinition.UsedTileId,
                    isUnderwater: Definition.IsUnderwater);
                _blocks.Add(block);
            }

            foreach (MovingPlatformSpawnDefinition platform in Definition.MovingPlatforms)
            {
                _blocks.Add(new Block(
                    platform.Position,
                    BlockType.Platform,
                    width: TileSize * platform.WidthInTiles,
                    height: TileSize,
                    isUnderground: platform.IsUnderground,
                    isUnderwater: Definition.IsUnderwater,
                    movingPlatform: platform,
                    tileId: platform.SourceTileId >= 0
                        ? platform.SourceTileId
                        : (int?)null));
            }

            foreach (PlatformLevelSpawnDefinition platformLevel in Definition.PlatformLevels)
            {
                Block leftPlatform = new Block(
                    platformLevel.LeftPlatform,
                    BlockType.Platform,
                    width: TileSize * 3,
                    height: TileSize / 2,
                    isUnderground: Definition.IsUnderground,
                    isUnderwater: Definition.IsUnderwater,
                    isPlatformLevel: true,
                    tileId: 809);
                Block rightPlatform = new Block(
                    platformLevel.RightPlatform,
                    BlockType.Platform,
                    width: TileSize * 3,
                    height: TileSize / 2,
                    isUnderground: Definition.IsUnderground,
                    isUnderwater: Definition.IsUnderwater,
                    isPlatformLevel: true,
                    tileId: 809);
                _blocks.Add(leftPlatform);
                _blocks.Add(rightPlatform);
                _pulleyPlatforms.Add(new PulleyPlatformPair(
                    leftPlatform,
                    rightPlatform,
                    platformLevel.PulleyY));
            }

            foreach (ItemSpawnDefinition item in Definition.Items)
            {
                _items.Add(ItemFactory.Create(item.Type, item.Position));
            }

            foreach (EnemySpawnDefinition enemy in Definition.Enemies)
            {
                IEnemy newEnemy;
                if (enemy.Type == EnemyType.CheepCheep)
                {
                    CheepCheepMovementMode movementMode = enemy.IsUnderwater
                        ? CheepCheepMovementMode.Swimming
                        : CheepCheepMovementMode.Flying;
                    newEnemy = EnemyFactory.Create(
                        enemy.Type,
                        enemy.Position,
                        enemy.SourceTileId,
                        movementMode);
                }
                else
                {
                    newEnemy = EnemyFactory.Create(
                        enemy.Type,
                        enemy.Position,
                        enemy.SourceTileId);
                }

                Enemy walkingEnemy = newEnemy as Enemy;
                if (walkingEnemy != null)
                {
                    walkingEnemy.SetWorldBlocks(_readOnlyBlocks);
                }

                if (newEnemy is Bowser)
                {
                    HasBowser = true;
                }

                _enemies.Add(newEnemy);
            }

            foreach (FireBarSpawnDefinition fireBar in Definition.FireBars)
            {
                for (int segmentIndex = 0; segmentIndex < fireBar.Length; segmentIndex++)
                {
                    _enemies.Add(EnemyFactory.CreateFireBarSegment(
                        fireBar.Position,
                        segmentIndex,
                        fireBar.StartingAngle,
                        fireBar.RotationDirection,
                        segmentIndex < fireBar.Length - 1));
                }
            }

            UpdatePulleyRopes();
        }

        private void UpdatePulleyRopes()
        {
            _pulleyRopes.Clear();
            foreach (PulleyPlatformPair platform in _pulleyPlatforms)
            {
                Rectangle leftRope = platform.LeftRope;
                Rectangle rightRope = platform.RightRope;
                if (leftRope.Height > 0)
                {
                    _pulleyRopes.Add(leftRope);
                }

                if (rightRope.Height > 0)
                {
                    _pulleyRopes.Add(rightRope);
                }
            }
        }

        private sealed class PulleyPlatformPair
        {
            private const float Acceleration = 540f;
            private const float DampingPerFrame = 0.92f;
            private readonly int _pulleyY;
            private float _leftVelocity;
            private float _fallVelocity;
            private Block _fallingPlatform;
            private bool _hasDetached;

            public Block Left { get; }
            public Block Right { get; }

            public Rectangle LeftRope
            {
                get
                {
                    return GetRope(Left);
                }
            }

            public Rectangle RightRope
            {
                get
                {
                    return GetRope(Right);
                }
            }

            public PulleyPlatformPair(Block left, Block right, int pulleyY)
            {
                Left = left;
                Right = right;
                _pulleyY = pulleyY;
            }

            public void Update(GameTime gameTime)
            {
                float elapsedSeconds = Math.Min(
                    1f / 30f, Math.Max(0f, (float)gameTime.ElapsedGameTime.TotalSeconds));
                if (elapsedSeconds == 0f)
                {
                    return;
                }

                if (_hasDetached)
                {
                    _fallingPlatform.ConsumePlatformLevelContact();
                    _fallVelocity += Acceleration * elapsedSeconds;
                    Vector2 fallingPosition = _fallingPlatform.Position;
                    fallingPosition.Y += _fallVelocity * elapsedSeconds;
                    _fallingPlatform.MoveTo(fallingPosition);
                    return;
                }

                bool leftContact = Left.ConsumePlatformLevelContact();
                bool rightContact = Right.ConsumePlatformLevelContact();
                if (leftContact != rightContact)
                {
                    _leftVelocity += (leftContact ? Acceleration : -Acceleration) * elapsedSeconds;
                }
                else
                {
                    _leftVelocity *= (float)Math.Pow(DampingPerFrame, elapsedSeconds * 60f);
                }

                float leftY = Left.Position.Y + _leftVelocity * elapsedSeconds;
                float rightY = Right.Position.Y - _leftVelocity * elapsedSeconds;
                if (leftY <= _pulleyY)
                {
                    leftY = _pulleyY;
                    _fallingPlatform = Right;
                    _fallVelocity = Math.Max(0f, -_leftVelocity);
                    _hasDetached = true;
                }
                else if (rightY <= _pulleyY)
                {
                    rightY = _pulleyY;
                    _fallingPlatform = Left;
                    _fallVelocity = Math.Max(0f, _leftVelocity);
                    _hasDetached = true;
                }

                Left.MoveTo(new Vector2(Left.Position.X, leftY));
                Right.MoveTo(new Vector2(Right.Position.X, rightY));
            }

            private Rectangle GetRope(Block platform)
            {
                int top = _pulleyY;
                int bottom = (int)platform.Bounds.Top;
                return new Rectangle(
                    (int)platform.Position.X + TileSize,
                    top,
                    TileSize,
                    Math.Max(0, bottom - top));
            }
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
