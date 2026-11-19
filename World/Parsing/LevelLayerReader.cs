using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Reads the CSV tile layers of a level and provides the shared grid access helpers (alias decoding, cell keys, tile lookup, tile size).
    /// </summary>
    internal static class LevelLayerReader
    {
        internal const int TileSize = 48;

        private static readonly IReadOnlyDictionary<string, int> MapTileAliases =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "brick", 289 },
                { "brick-alt", 290 },
                { "cannon", 63 },
                { "coin", 144 },
                { "flag", 152 },
                { "flagpole", 101 },
                { "flagpole-alt", 149 },
                { "ground", 384 },
                { "invisible", 720 },
                { "horizontal-pipe-bottom-inner-left", 293 },
                { "horizontal-pipe-bottom-inner-right", 294 },
                { "horizontal-pipe-bottom-left", 292 },
                { "horizontal-pipe-bottom-right", 295 },
                { "platform", 609 },
                { "pipe-left-top", 150 },
                { "pipe-right-top", 151 },
                { "pipe-left-body", 198 },
                { "pipe-right-body", 199 },
                { "horizontal-pipe-top-inner-left", 245 },
                { "horizontal-pipe-top-inner-right", 246 },
                { "horizontal-pipe-top-left", 244 },
                { "horizontal-pipe-top-right", 247 },
                { "question", 192 },
                { "spring", 346 },
                { "used", 196 }
            };
        private static readonly IReadOnlyDictionary<string, int> UndergroundTileAliases =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "coin", 160 },
                { "horizontal-pipe-bottom-inner-left", 309 },
                { "horizontal-pipe-bottom-inner-right", 310 },
                { "horizontal-pipe-bottom-left", 308 },
                { "horizontal-pipe-bottom-right", 311 },
                { "horizontal-pipe-top-inner-left", 261 },
                { "horizontal-pipe-top-inner-right", 262 },
                { "horizontal-pipe-top-left", 260 },
                { "horizontal-pipe-top-right", 263 },
                { "pipe-left-body", 214 },
                { "pipe-right-body", 215 },
                { "underground-brick", 306 },
                { "underground-floor", 400 }
            };
        private static readonly IReadOnlyDictionary<string, int> EnemyTileAliases =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "bowser", 61 },
                { "bullet-bill", 90 },
                { "cheep-cheep", 81 },
                { "cheep-cheep-alt", 498 },
                { "goomba", 70 },
                { "goomba-alt", 71 },
                { "hammer-bro", 56 },
                { "koopa", 38 },
                { "koopa-alt", 39 },
                { "lakitu", 50 },
                { "piranha-plant", 44 },
                { "blooper", 48 }
            };
        private static readonly IReadOnlyDictionary<string, int> ItemTileAliases =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "coin", 144 },
                { "fire-flower", 48 },
                { "fire-flower-mushroom", 608 },
                { "one-up", 52 },
                { "star", 96 },
                { "vine", 148 }
            };
        private static readonly IReadOnlyDictionary<string, int> NoTileAliases =
            new Dictionary<string, int>();

        internal static int[][] ReadGrid(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Required level layer was not found.", path);
            }

            string[] lines = File.ReadAllLines(path);
            List<int[]> rows = new List<int[]>(lines.Length);
            int expectedWidth = -1;
            IReadOnlyDictionary<string, int> aliases = GetTileAliases(path);
            for (int rowIndex = 0; rowIndex < lines.Length; rowIndex++)
            {
                if (string.IsNullOrWhiteSpace(lines[rowIndex]))
                {
                    continue;
                }

                string[] cells = lines[rowIndex].Split(',');
                if (expectedWidth < 0)
                {
                    expectedWidth = cells.Length;
                }
                else if (cells.Length != expectedWidth)
                {
                    throw new InvalidDataException(
                        "Layer " + path + " has an inconsistent row width at line "
                        + (rowIndex + 1).ToString(CultureInfo.InvariantCulture));
                }

                int[] values = new int[cells.Length];
                for (int columnIndex = 0; columnIndex < cells.Length; columnIndex++)
                {
                    string cell = cells[columnIndex].Trim();
                    if (cell.Equals(".", StringComparison.Ordinal)
                        || cell.Equals("empty", StringComparison.OrdinalIgnoreCase))
                    {
                        values[columnIndex] = -1;
                    }
                    else if (!aliases.TryGetValue(cell, out values[columnIndex])
                        && !int.TryParse(
                            cell,
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out values[columnIndex]))
                    {
                        throw new InvalidDataException(
                            "Invalid tile value or alias '" + cell + "' in " + path + " at row "
                            + (rowIndex + 1).ToString(CultureInfo.InvariantCulture) + ", column "
                            + (columnIndex + 1).ToString(CultureInfo.InvariantCulture));
                    }
                }

                rows.Add(values);
            }

            if (rows.Count == 0 || expectedWidth <= 0)
            {
                throw new InvalidDataException("Layer contains no tile rows: " + path);
            }

            return rows.ToArray();
        }

        private static IReadOnlyDictionary<string, int> GetTileAliases(string path)
        {
            string layerName = Path.GetFileNameWithoutExtension(path);
            if (layerName.EndsWith("_Enemies", StringComparison.OrdinalIgnoreCase))
            {
                return EnemyTileAliases;
            }

            if (layerName.EndsWith("_Collectibles", StringComparison.OrdinalIgnoreCase)
                || layerName.EndsWith("_Items", StringComparison.OrdinalIgnoreCase))
            {
                return ItemTileAliases;
            }

            if (layerName.EndsWith("_Underground", StringComparison.OrdinalIgnoreCase))
            {
                return UndergroundTileAliases;
            }

            if (layerName.EndsWith("_Foreground", StringComparison.OrdinalIgnoreCase))
            {
                return MapTileAliases;
            }

            return NoTileAliases;
        }

        internal static int[][] ReadOptionalGrid(string path)
        {
            return File.Exists(path) ? ReadGrid(path) : null;
        }

        internal static void EnsureSameDimensions(int[][] reference, int[][] candidate, string layerName)
        {
            if (candidate == null)
            {
                return;
            }

            if (candidate.Length != reference.Length || candidate[0].Length != reference[0].Length)
            {
                throw new InvalidDataException("Layer dimensions do not match the foreground: " + layerName);
            }
        }

        internal static List<LevelTile> GetLayerTiles(int[][] grid, HashSet<long> excludedCells)
        {
            List<LevelTile> tiles = new List<LevelTile>();
            AddLayerTiles(grid, excludedCells, tiles);
            return tiles;
        }

        internal static void AddLayerTiles(
            int[][] grid,
            HashSet<long> excludedCells,
            List<LevelTile> tiles)
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
                    if (tileId < 0
                        || excludedCells != null && excludedCells.Contains(GetCellKey(column, row)))
                    {
                        continue;
                    }

                    try
                    {
                        tiles.Add(new LevelTile(tileId, column * TileSize, row * TileSize));
                    }
                    catch (ArgumentOutOfRangeException exception)
                    {
                        throw new InvalidDataException(
                            "Tile ID " + tileId.ToString(CultureInfo.InvariantCulture)
                            + " is outside the tile atlas at row "
                            + (row + 1).ToString(CultureInfo.InvariantCulture) + ", column "
                            + (column + 1).ToString(CultureInfo.InvariantCulture), exception);
                    }
                }
            }
        }

        internal static long GetCellKey(int column, int row)
        {
            return ((long)row << 32) | (uint)column;
        }

        internal static int GetLayerTile(int[][] grid, int column, int row)
        {
            if (grid == null || row < 0 || row >= grid.Length
                || column < 0 || column >= grid[row].Length)
            {
                return -1;
            }

            return grid[row][column];
        }
    }
}
