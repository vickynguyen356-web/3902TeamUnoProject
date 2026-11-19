using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Items;
using static TeamUno.Mario.World.LevelLayerReader;
using static TeamUno.Mario.World.BlockTileIds;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Turns the collectibles and items layers into item spawns and fills the contents of ?-blocks, bricks and hidden blocks.
    /// </summary>
    internal static class ItemSpawnParser
    {
        internal static void AddCollectibles(
            int[][] foreground,
            int[][] underground,
            int[][] collectibleGrid,
            List<ItemSpawnDefinition> items,
            List<BlockSpawnDefinition> blocks,
            List<VineSpawnDefinition> vines,
            bool isUnderground)
        {
            if (collectibleGrid == null)
            {
                return;
            }

            for (int row = 0; row < collectibleGrid.Length; row++)
            {
                for (int column = 0; column < collectibleGrid[row].Length; column++)
                {
                    int tileId = collectibleGrid[row][column];
                    if (tileId < 0)
                    {
                        continue;
                    }

                    ItemType? itemType = GetCollectibleItemType(tileId);
                    if (!itemType.HasValue)
                    {
                        continue;
                    }

                    ItemType contents = itemType.Value == ItemType.FloatingCoin
                        ? ItemType.BlockCoin
                        : itemType.Value;
                    if (contents == ItemType.Vine
                        && !HasVineAt(vines, new Vector2(column * TileSize, row * TileSize)))
                    {
                        throw new InvalidDataException(
                            "Vine collectible in cell (" + column.ToString(CultureInfo.InvariantCulture)
                            + ", " + row.ToString(CultureInfo.InvariantCulture)
                            + ") has no matching VINE property.");
                    }
                    if (IsCollectibleBlock(foreground[row][column]))
                    {
                        if (ReplaceBlockContents(blocks, column, row, contents))
                        {
                            continue;
                        }
                    }

                    int undergroundTile = underground == null ? -1 : underground[row][column];
                    if (IsCollectibleBlock(undergroundTile))
                    {
                        if (ReplaceBlockContents(blocks, column, row, contents))
                        {
                            continue;
                        }
                    }

                    blocks.Add(new BlockSpawnDefinition(
                        new Vector2(column * TileSize, row * TileSize),
                        BlockType.Invisible,
                        contents: contents,
                        isUnderground: undergroundTile >= 0 || isUnderground,
                        tileId: undergroundTile >= 0 ? undergroundTile
                            : foreground[row][column] >= 0 ? foreground[row][column] : (int?)null,
                        usedTileId: GetUsedTileId(undergroundTile >= 0
                            ? undergroundTile
                            : foreground[row][column])));
                }
            }
        }

        internal static void AddPlacedItems(int[][] itemGrid, List<ItemSpawnDefinition> items)
        {
            if (itemGrid == null)
            {
                return;
            }

            for (int row = 0; row < itemGrid.Length; row++)
            {
                for (int column = 0; column < itemGrid[row].Length; column++)
                {
                    ItemType? itemType = GetCollectibleItemType(itemGrid[row][column]);
                    if (!itemType.HasValue)
                    {
                        continue;
                    }

                    if (itemType.Value == ItemType.Vine)
                    {
                        throw new InvalidDataException(
                            "Vine in the placed-items layer must be defined in the collectibles layer with VINE metadata.");
                    }

                    float x = column * TileSize
                        + (itemType.Value == ItemType.FloatingCoin ? (TileSize - 24) / 2f : 0f);
                    items.Add(new ItemSpawnDefinition(
                        itemType.Value,
                        new Vector2(x, row * TileSize)));
                }
            }
        }

        internal static HashSet<long> GetHiddenCollectibleCells(
            int[][] foreground,
            int[][] underground,
            int[][] collectibleGrid)
        {
            HashSet<long> cells = new HashSet<long>();
            if (collectibleGrid == null)
            {
                return cells;
            }

            for (int row = 0; row < collectibleGrid.Length; row++)
            {
                for (int column = 0; column < collectibleGrid[row].Length; column++)
                {
                    if (!GetCollectibleItemType(collectibleGrid[row][column]).HasValue)
                    {
                        continue;
                    }

                    int undergroundTile = underground == null ? -1 : underground[row][column];
                    if (!IsCollectibleBlock(foreground[row][column])
                        && !IsCollectibleBlock(undergroundTile))
                    {
                        cells.Add(GetCellKey(column, row));
                    }
                }
            }

            return cells;
        }

        private static bool IsCollectibleBlock(int tileId)
        {
            int referenceTileId = GetReferenceBlockId(tileId);
            return referenceTileId == 192 || IsBrickTile(tileId) || referenceTileId == 720;
        }

        private static ItemType? GetCollectibleItemType(int tileId)
        {
            switch (GetReferenceBlockId(tileId))
            {
                case 48:
                    return ItemType.FireFlower;
                case 144:
                    return ItemType.FloatingCoin;
                case 52:
                    return ItemType.OneUpMushroom;
                case 96:
                    return ItemType.Star;
                case 148:
                    return ItemType.Vine;
                case 608:
                    return ItemType.FireFlowerMushroom;
                default:
                    return null;
            }
        }

        private static bool HasVineAt(List<VineSpawnDefinition> vines, Vector2 position)
        {
            foreach (VineSpawnDefinition vine in vines)
            {
                if (vine.Position == position)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ReplaceBlockContents(
            List<BlockSpawnDefinition> blocks,
            int column,
            int row,
            ItemType contents)
        {
            Vector2 position = new Vector2(column * TileSize, row * TileSize);
            for (int index = 0; index < blocks.Count; index++)
            {
                BlockSpawnDefinition block = blocks[index];
                if ((block.Type == BlockType.Question
                        || block.Type == BlockType.Brick
                        || block.Type == BlockType.Invisible)
                    && block.Position == position)
                {
                    blocks[index] = new BlockSpawnDefinition(
                        position,
                        block.Type,
                        block.Width,
                        block.Height,
                        contents,
                        block.DestinationArea,
                        block.DestinationSpawn,
                        block.DestinationPosition,
                        block.EntryDirection,
                        block.DestinationCameraPosition,
                        block.FreezeCamera,
                        block.IsTopBrick,
                        block.IsHorizontalPipeAttachment,
                        block.ExitDirection,
                        block.DestinationBackgroundColor,
                        block.DestinationIsUnderwater,
                        block.DestinationMusicTrack,
                        block.IsUnderground,
                        block.TileId,
                        block.UsedTileId);
                    return true;
                }
            }

            return false;
        }
    }
}
