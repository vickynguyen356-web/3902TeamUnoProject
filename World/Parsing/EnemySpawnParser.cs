using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Enemies;
using static TeamUno.Mario.World.LevelLayerReader;
using static TeamUno.Mario.World.BlockTileIds;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Turns the enemies layer (plus cannon tiles found in the map layers) into enemy spawn definitions.
    /// </summary>
    internal static class EnemySpawnParser
    {
        internal static void AddEnemies(
            int[][] grid,
            int[][] background,
            List<EnemySpawnDefinition> enemies)
        {
            if (grid == null)
            {
                return;
            }

            for (int row = 0; row < grid.Length; row++)
            {
                for (int column = 0; column < grid[row].Length; column++)
                {
                    int tileId = grid[row][column];
                    EnemyType? enemyType = GetEnemyType(tileId);
                    if (!enemyType.HasValue || IsEnemyComponent(grid, tileId, column, row))
                    {
                        continue;
                    }

                    float xOffset = tileId == 39 || tileId == 44 ? TileSize / 2f
                        : tileId == 71 ? 16f : 0f;
                    float yOffset = enemyType.Value == EnemyType.PiranhaPlant
                        ? 2 * TileSize - PiranhaPlant.PiranhaHeight
                        : 0f;

                    enemies.Add(new EnemySpawnDefinition(
                        enemyType.Value,
                        new Vector2(column * TileSize + xOffset, row * TileSize + yOffset),
                        tileId,
                        isUnderwater: tileId == 81 || GetLayerTile(background, column, row) == 186));
                }
            }
        }

        internal static void AddCannonSpawners(int[][] grid, List<EnemySpawnDefinition> enemies)
        {
            if (grid == null)
            {
                return;
            }

            for (int row = 0; row < grid.Length; row++)
            {
                for (int column = 0; column < grid[row].Length; column++)
                {
                    if (GetReferenceBlockId(grid[row][column]) == 63)
                    {
                        Vector2 position = new Vector2(column * TileSize, row * TileSize);
                        bool alreadyAdded = false;
                        foreach (EnemySpawnDefinition enemy in enemies)
                        {
                            if (enemy.Type == EnemyType.BulletBillCannon && enemy.Position == position)
                            {
                                alreadyAdded = true;
                                break;
                            }
                        }

                        if (alreadyAdded)
                        {
                            continue;
                        }

                        enemies.Add(new EnemySpawnDefinition(
                            EnemyType.BulletBillCannon,
                            position,
                            grid[row][column]));
                    }
                }
            }
        }

        private static EnemyType? GetEnemyType(int tileId)
        {
            switch (tileId)
            {
                case 38:
                case 39:
                    return EnemyType.Koopa;
                case 70:
                case 71:
                    return EnemyType.Goomba;
                case 81:
                case 498:
                    return EnemyType.CheepCheep;
                case 40:
                    return EnemyType.Koopa;
                case 44:
                    return EnemyType.PiranhaPlant;
                case 48:
                    return EnemyType.Blooper;
                case 50:
                    return EnemyType.Lakitu;
                case 56:
                    return EnemyType.HammerBro;
                case 61:
                    return EnemyType.Bowser;
                case 90:
                    return EnemyType.BulletBill;
                default:
                    return null;
            }
        }

        private static bool IsEnemyComponent(int[][] grid, int tileId, int column, int row)
        {
            return tileId == 73 && row > 0 && grid[row - 1][column] == 38;
        }
    }
}
