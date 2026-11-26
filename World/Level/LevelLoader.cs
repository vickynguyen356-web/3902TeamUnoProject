using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal static class LevelLoader
    {
        private const int TileSize = 48;

        public static bool Load(
            LevelDefinition definition,
            List<IBlock> blocks,
            List<IItem> items,
            List<IEnemy> enemies,
            List<IProjectile> projectiles,
            List<PulleyPlatformPair> pulleyPlatforms,
            List<Rectangle> pulleyRopes,
            IReadOnlyList<IBlock> readOnlyBlocks)
        {
            ArgumentNullException.ThrowIfNull(definition);

            blocks.Clear();
            items.Clear();
            enemies.Clear();
            projectiles.Clear();
            pulleyPlatforms.Clear();
            pulleyRopes.Clear();

            LoadBlocks(definition, blocks, pulleyPlatforms);
            LoadItems(definition, items);
            bool hasBowser = LoadEnemies(definition, enemies, readOnlyBlocks);
            LoadFireBars(definition, enemies);
            PulleyPlatformSystem.UpdateRopes(pulleyPlatforms, pulleyRopes);
            return hasBowser;
        }

        private static void LoadBlocks(
            LevelDefinition definition,
            List<IBlock> blocks,
            List<PulleyPlatformPair> pulleyPlatforms)
        {
            foreach (BlockSpawnDefinition blockDefinition in definition.Blocks)
            {
                blocks.Add(new Block(
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
                    isUnderwater: definition.IsUnderwater));
            }

            foreach (MovingPlatformSpawnDefinition platform in definition.MovingPlatforms)
            {
                blocks.Add(new Block(
                    platform.Position,
                    BlockType.Platform,
                    width: TileSize * platform.WidthInTiles,
                    height: TileSize,
                    isUnderground: platform.IsUnderground,
                    isUnderwater: definition.IsUnderwater,
                    movingPlatform: platform,
                    tileId: platform.SourceTileId >= 0
                        ? platform.SourceTileId
                        : (int?)null));
            }

            foreach (PlatformLevelSpawnDefinition platformLevel in definition.PlatformLevels)
            {
                IBlock leftPlatform = new Block(
                    platformLevel.LeftPlatform,
                    BlockType.Platform,
                    width: TileSize * 3,
                    height: TileSize / 2,
                    isUnderground: definition.IsUnderground,
                    isUnderwater: definition.IsUnderwater,
                    isPlatformLevel: true,
                    tileId: 809);
                IBlock rightPlatform = new Block(
                    platformLevel.RightPlatform,
                    BlockType.Platform,
                    width: TileSize * 3,
                    height: TileSize / 2,
                    isUnderground: definition.IsUnderground,
                    isUnderwater: definition.IsUnderwater,
                    isPlatformLevel: true,
                    tileId: 809);
                blocks.Add(leftPlatform);
                blocks.Add(rightPlatform);
                pulleyPlatforms.Add(new PulleyPlatformPair(
                    leftPlatform,
                    rightPlatform,
                    platformLevel.PulleyY));
            }
        }

        private static void LoadItems(
            LevelDefinition definition,
            List<IItem> items)
        {
            foreach (ItemSpawnDefinition item in definition.Items)
            {
                items.Add(ItemFactory.Create(item.Type, item.Position));
            }
        }

        private static bool LoadEnemies(
            LevelDefinition definition,
            List<IEnemy> enemies,
            IReadOnlyList<IBlock> readOnlyBlocks)
        {
            bool hasBowser = false;
            foreach (EnemySpawnDefinition enemy in definition.Enemies)
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

                if (newEnemy is Enemy walkingEnemy)
                {
                    walkingEnemy.SetWorldBlocks(readOnlyBlocks);
                }

                if (newEnemy is Bowser)
                {
                    hasBowser = true;
                }

                enemies.Add(newEnemy);
            }

            return hasBowser;
        }

        private static void LoadFireBars(
            LevelDefinition definition,
            List<IEnemy> enemies)
        {
            foreach (FireBarSpawnDefinition fireBar in definition.FireBars)
            {
                for (int segmentIndex = 0;
                    segmentIndex < fireBar.Length;
                    segmentIndex++)
                {
                    enemies.Add(EnemyFactory.CreateFireBarSegment(
                        fireBar.Position,
                        segmentIndex,
                        fireBar.StartingAngle,
                        fireBar.RotationDirection,
                        segmentIndex < fireBar.Length - 1));
                }
            }
        }

    }
}
