using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Items;
using static TeamUno.Mario.World.LevelLayerReader;
using static TeamUno.Mario.World.BlockTileIds;
using static TeamUno.Mario.World.WarpPipeParser;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Turns foreground/underground layer tiles into block spawn definitions (map blocks, flag poles) and reports which cells are already modelled by a block.
    /// </summary>
    internal static class BlockSpawnParser
    {
        internal static void AddMapBlocks(
            int[][] grid,
            List<BlockSpawnDefinition> blocks,
            HashSet<long> pipeCells,
            bool isUnderground,
            bool isUnderwater,
            List<ItemSpawnDefinition> items,
            HashSet<long> coinTileCells,
            HashSet<long> hiddenCollectibleCells)
        {
            for (int row = 0; row < grid.Length; row++)
            {
                for (int column = 0; column < grid[row].Length; column++)
                {
                    long cellKey = GetCellKey(column, row);
                    if (pipeCells.Contains(cellKey) || hiddenCollectibleCells.Contains(cellKey))
                    {
                        continue;
                    }

                    int tileId = grid[row][column];
                    if (tileId < 0)
                    {
                        continue;
                    }

                    if (GetMapItemType(tileId) == ItemType.FloatingCoin)
                    {
                        items.Add(new ItemSpawnDefinition(
                            ItemType.FloatingCoin,
                            new Vector2(column * TileSize + (TileSize - 24) / 2f, row * TileSize)));
                        coinTileCells.Add(cellKey);
                        continue;
                    }

                    int referenceTileId = GetReferenceBlockId(tileId);
                    if (referenceTileId == 346)
                    {
                        blocks.Add(new BlockSpawnDefinition(
                            new Vector2(column * TileSize, row * TileSize),
                            BlockType.Spring,
                            3 * TileSize,
                            2 * TileSize,
                            isUnderground: isUnderground,
                            tileId: tileId));
                        continue;
                    }

                    if (IsSpringPartTile(referenceTileId))
                    {
                        continue;
                    }

                    if (referenceTileId == 150
                        && column + 1 < grid[row].Length
                        && GetReferenceBlockId(grid[row][column + 1]) == 151)
                    {
                        int pipeHeight = FindPipeHeight(grid, column, row);
                        MarkPipeCells(pipeCells, column, row, 2, pipeHeight);
                        blocks.Add(new BlockSpawnDefinition(
                            new Vector2(column * TileSize, row * TileSize),
                            BlockType.Pipe,
                            2 * TileSize,
                            pipeHeight * TileSize,
                            isHorizontalPipeAttachment: HasHorizontalPipeBelow(grid, column, row),
                            isUnderground: isUnderground));
                        continue;
                    }

                    if (IsPipeTile(tileId))
                    {
                        continue;
                    }

                    BlockType? blockType = GetBlockType(tileId);
                    if ((isUnderground || isUnderwater) && GetReferenceBlockId(tileId) == 384)
                    {
                        blockType = BlockType.Ground;
                    }
                    if (IsNonCollidableTile(tileId))
                    {
                        continue;
                    }

                    bool isTopBrick = blockType.HasValue && blockType.Value == BlockType.Brick
                        && (row == 0 || !IsBrickTile(grid[row - 1][column]));
                    bool useThemedSprite = (isUnderground || isUnderwater)
                        && blockType.HasValue
                        && blockType.Value != BlockType.Invisible
                        && blockType.Value != BlockType.Platform;
                    blocks.Add(CreateTileBlock(
                        column,
                        row,
                        blockType ?? BlockType.Tile,
                        isTopBrick,
                        isUnderground,
                        useThemedSprite ? -1 : tileId,
                        useThemedSprite ? null : GetUsedTileId(tileId)));
                }
            }
        }

        private static BlockType? GetBlockType(int tileId)
        {
            if (tileId == 720)
            {
                return BlockType.Invisible;
            }

            switch (GetReferenceBlockId(tileId))
            {
                case 196:
                    return BlockType.Used;
                case 240:
                    return BlockType.Axe;
                case 192:
                    return BlockType.Question;
                case 289:
                case 290:
                    return BlockType.Brick;
                case 392:
                    return BlockType.Bridge;
                case 609:
                    return BlockType.Platform;
                default:
                    return null;
            }
        }

        private static BlockSpawnDefinition CreateTileBlock(
            int column,
            int row,
            BlockType type,
            bool isTopBrick = false,
            bool isUnderground = false,
            int tileId = -1,
            int? usedTileId = null)
        {
            return new BlockSpawnDefinition(
                new Vector2(column * TileSize, row * TileSize),
                type,
                isTopBrick: isTopBrick,
                isUnderground: isUnderground,
                tileId: tileId >= 0 ? tileId : (int?)null,
                usedTileId: usedTileId);
        }

        internal static void AddFlagpoles(
            int[][] grid,
            List<BlockSpawnDefinition> blocks,
            bool isUnderground)
        {
            for (int column = 0; column < grid[0].Length; column++)
            {
                for (int row = 0; row < grid.Length; row++)
                {
                    if (!IsFlagPoleTile(grid[row][column])
                        || (row > 0 && IsFlagPoleTile(grid[row - 1][column])))
                    {
                        continue;
                    }

                    int height = 1;
                    while (row + height < grid.Length && IsFlagPoleTile(grid[row + height][column]))
                    {
                        height++;
                    }

                    blocks.Add(new BlockSpawnDefinition(
                        new Vector2(column * TileSize, row * TileSize),
                        BlockType.FlagPole,
                        TileSize,
                        height * TileSize,
                        isUnderground: isUnderground));
                }

                for (int row = 0; row < grid.Length; row++)
                {
                    if (GetReferenceBlockId(grid[row][column]) == 152)
                    {
                        blocks.Add(new BlockSpawnDefinition(
                            new Vector2(column * TileSize + TileSize / 2, row * TileSize),
                            BlockType.Flag,
                            isUnderground: isUnderground));
                    }
                }
            }
        }

        internal static HashSet<long> GetModeledBlockCells(
            List<BlockSpawnDefinition> blocks,
            List<MovingPlatformSpawnDefinition> movingPlatforms,
            List<PlatformLevelSpawnDefinition> platformLevels,
            bool isUnderground)
        {
            HashSet<long> cells = new HashSet<long>();
            foreach (BlockSpawnDefinition block in blocks)
            {
                if (block.IsUnderground != isUnderground
                    || block.Type == BlockType.Invisible
                    || block.Type == BlockType.Tile
                    || block.Type == BlockType.Pipe
                    || block.Type == BlockType.FlagPole)
                {
                    continue;
                }

                int firstColumn = (int)block.Position.X / TileSize;
                int firstRow = (int)block.Position.Y / TileSize;
                int columnCount = (block.Width + TileSize - 1) / TileSize;
                int rowCount = (block.Height + TileSize - 1) / TileSize;
                for (int row = firstRow; row < firstRow + rowCount; row++)
                {
                    for (int column = firstColumn; column < firstColumn + columnCount; column++)
                    {
                        cells.Add(GetCellKey(column, row));
                    }
                }
            }

            foreach (MovingPlatformSpawnDefinition platform in movingPlatforms)
            {
                if (platform.IsUnderground != isUnderground)
                {
                    continue;
                }

                int firstColumn = (int)platform.Position.X / TileSize;
                int row = (int)platform.Position.Y / TileSize;
                for (int column = firstColumn;
                    column < firstColumn + platform.WidthInTiles;
                    column++)
                {
                    cells.Add(GetCellKey(column, row));
                }
            }

            foreach (PlatformLevelSpawnDefinition platformLevel in platformLevels)
            {
                AddPlatformLevelCells(cells, platformLevel.LeftPlatform);
                AddPlatformLevelCells(cells, platformLevel.RightPlatform);
            }

            return cells;
        }

        private static void AddPlatformLevelCells(HashSet<long> cells, Vector2 position)
        {
            int column = (int)position.X / TileSize;
            int row = (int)position.Y / TileSize;
            for (int offset = 0; offset < 3; offset++)
            {
                cells.Add(GetCellKey(column + offset, row));
            }
        }
    }
}
