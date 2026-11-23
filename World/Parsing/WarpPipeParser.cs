using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using static TeamUno.Mario.World.LevelLayerReader;
using static TeamUno.Mario.World.LevelProperties;
using static TeamUno.Mario.World.BlockTileIds;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Turns the WARP_PIPE / WARP_PIPE_AREA properties into pipe blocks and owns the pipe geometry helpers (height, cell marking, horizontal footprints).
    /// </summary>
    internal static class WarpPipeParser
    {
        private static readonly Regex WarpPipePattern = new Regex(
            @"\((?<x>\d+)\s*,\s*(?<y>\d+)\)\s*\((?<targetX>\d+)\s*,\s*(?<targetY>\d+)\)\s*" +
            @"\((?<cameraX>\d+)\s*,\s*(?<cameraY>\d+)\)\s*(?<entry>\w+)\s+(?<exit>\w+)\s+" +
            @"(?<freeze>\w+)\s+(?<background>\w+)\s+(?<levelType>\w+)\s+" +
            @"\((?<levelX>\d+)\s*,\s*(?<levelY>\d+)\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex AreaWarpPipePattern = new Regex(
            @"\((?<x>\d+)\s*,\s*(?<y>\d+)\)\s+(?<area>[A-Za-z][A-Za-z0-9_-]*)\s+" +
            @"(?<spawn>[A-Za-z][A-Za-z0-9_-]*)\s+(?<entry>\w+)\s+(?<exit>\w+)\s+" +
            @"(?<freeze>\w+)\s+(?<background>\w+)\s+(?<levelType>\w+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static void AddWarpPipeBlocks(
            string properties,
            string levelId,
            int[][] foreground,
            int[][] underground,
            bool isUnderground,
            List<BlockSpawnDefinition> blocks,
            HashSet<long> foregroundPipeCells,
            HashSet<long> undergroundPipeCells)
        {
            List<WarpPipeRecord> warpPipes = ParseWarpPipes(GetProperty(properties, "WARP_PIPE"));
            warpPipes.AddRange(ParseAreaWarpPipes(GetProperty(properties, "WARP_PIPE_AREA")));

            for (int index = 0; index < warpPipes.Count; index++)
            {
                WarpPipeRecord pipe = warpPipes[index];
                bool foregroundHasPipe = HasPipeNear(foreground, pipe.X, pipe.Y);
                bool undergroundHasPipe = HasPipeNear(underground, pipe.X, pipe.Y);
                bool pipeIsUnderground = undergroundHasPipe
                    && (!foregroundHasPipe || isUnderground);
                int[][] pipeGrid = pipeIsUnderground ? underground : foreground;
                HashSet<long> pipeCells = pipeIsUnderground
                    ? undergroundPipeCells
                    : foregroundPipeCells;
                bool targetsAnotherLevel = pipe.LevelX != 0 || pipe.LevelY != 0;
                if (targetsAnotherLevel && (pipe.LevelX <= 0 || pipe.LevelY <= 0))
                {
                    throw new InvalidDataException(
                        "Warp pipe in '" + levelId + "' has an incomplete destination level coordinate.");
                }

                bool supportsEntry = pipe.EntryDirection.Equals("DOWN", StringComparison.OrdinalIgnoreCase)
                    || pipe.EntryDirection.Equals("LEFT", StringComparison.OrdinalIgnoreCase)
                    || pipe.EntryDirection.Equals("RIGHT", StringComparison.OrdinalIgnoreCase);
                string destinationArea = pipe.DestinationArea;
                string destinationSpawn = null;
                Vector2? destinationPosition = null;
                Vector2? destinationCameraPosition = null;
                int pipeX = pipe.X;
                int pipeY = pipe.Y;
                int pipeWidth = 2;
                int pipeHeight;
                if (supportsEntry)
                {
                    if (destinationArea == null)
                    {
                        destinationArea = targetsAnotherLevel
                            ? "World" + pipe.LevelX.ToString(CultureInfo.InvariantCulture)
                                + "-" + pipe.LevelY.ToString(CultureInfo.InvariantCulture)
                            : levelId;
                    }
                    if (pipe.DestinationSpawn != null)
                    {
                        destinationSpawn = pipe.DestinationSpawn;
                    }
                    else if (targetsAnotherLevel && pipe.TargetX == 0 && pipe.TargetY == 0)
                    {
                        destinationSpawn = "start";
                    }
                    else if (pipe.TargetX != 0 || pipe.TargetY != 0)
                    {
                        destinationPosition = new Vector2(
                            pipe.TargetX * TileSize,
                            pipe.TargetY * TileSize);
                    }

                    destinationCameraPosition = new Vector2(
                        pipe.CameraX * TileSize,
                        pipe.CameraY * TileSize);
                }

                bool isHorizontalPipe = pipe.EntryDirection.Equals(
                    "LEFT", StringComparison.OrdinalIgnoreCase)
                    || pipe.EntryDirection.Equals("RIGHT", StringComparison.OrdinalIgnoreCase);
                if (isHorizontalPipe
                    && TryGetHorizontalPipeFootprint(pipeGrid, pipe.X, pipe.Y, out int horizontalWidth))
                {
                    pipeY = pipe.Y - 1;
                    pipeWidth = horizontalWidth;
                    pipeHeight = 2;
                }
                else
                {
                    pipeHeight = FindPipeHeight(pipeGrid, pipeX, pipeY);
                }

                MarkPipeCells(pipeCells, pipeX, pipeY, pipeWidth, pipeHeight);
                blocks.Add(new BlockSpawnDefinition(
                    new Vector2(pipeX * TileSize, pipeY * TileSize),
                    BlockType.Pipe,
                    pipeWidth * TileSize,
                    pipeHeight * TileSize,
                    destinationArea: destinationArea,
                    destinationSpawn: destinationSpawn,
                    destinationPosition: destinationPosition,
                    entryDirection: pipe.EntryDirection,
                    destinationCameraPosition: destinationCameraPosition,
                    freezeCamera: pipe.FreezeCamera,
                    exitDirection: pipe.ExitDirection,
                    destinationBackgroundColor: ParseBackgroundColor(pipe.BackgroundColor),
                    destinationIsUnderwater: pipe.LevelType.Contains(
                        "UNDERWATER", StringComparison.OrdinalIgnoreCase),
                    destinationMusicTrack: GetMusicTrack(pipe.LevelType),
                    isUnderground: isUnderground || pipeIsUnderground));
            }
        }

        internal static int FindPipeHeight(int[][] grid, int column, int row)
        {
            if (row < 0 || row >= grid.Length || column < 0 || column + 1 >= grid[row].Length)
            {
                return 2;
            }

            int height = 0;
            for (int currentRow = row; currentRow < grid.Length; currentRow++)
            {
                bool hasPipeTile = IsPipeTile(grid[currentRow][column])
                    || IsPipeTile(grid[currentRow][column + 1]);
                if (!hasPipeTile)
                {
                    break;
                }

                height++;
            }

            return Math.Max(2, height);
        }

        internal static void MarkPipeCells(HashSet<long> cells, int column, int row, int width, int height)
        {
            for (int y = row; y < row + height; y++)
            {
                for (int x = column; x < column + width; x++)
                {
                    cells.Add(GetCellKey(x, y));
                }
            }
        }

        private static bool HasPipeNear(int[][] grid, int column, int row)
        {
            for (int currentRow = row - 1; currentRow <= row + 1; currentRow++)
            {
                for (int currentColumn = column - 1; currentColumn <= column + 2; currentColumn++)
                {
                    if (IsPipeTile(GetLayerTile(grid, currentColumn, currentRow)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool HasHorizontalPipeBelow(int[][] grid, int column, int row)
        {
            int horizontalColumn = column - 2;
            int topRow = row + 2;
            int bottomRow = row + 3;
            if (horizontalColumn < 0 || bottomRow >= grid.Length
                || horizontalColumn + 3 >= grid[0].Length)
            {
                return false;
            }

            for (int offset = 0; offset < 4; offset++)
            {
                if (GetReferenceBlockId(grid[topRow][horizontalColumn + offset]) != 244 + offset
                    || GetReferenceBlockId(grid[bottomRow][horizontalColumn + offset]) != 292 + offset)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetHorizontalPipeFootprint(
            int[][] grid,
            int column,
            int row,
            out int width)
        {
            width = 0;
            int topRow = row - 1;
            if (topRow < 0 || row >= grid.Length || column < 0 || column + 3 >= grid[row].Length)
            {
                return false;
            }

            bool matchesRightFacingPipe = true;
            bool matchesLeftFacingPipe = true;
            for (int offset = 0; offset < 4; offset++)
            {
                int topTileId = GetReferenceBlockId(grid[topRow][column + offset]);
                int bottomTileId = GetReferenceBlockId(grid[row][column + offset]);
                matchesRightFacingPipe &= topTileId == 244 + offset
                    && bottomTileId == 292 + offset;
                matchesLeftFacingPipe &= topTileId == 247 - offset
                    && bottomTileId == 295 - offset;
            }

            if (!matchesRightFacingPipe && !matchesLeftFacingPipe)
            {
                return false;
            }

            width = 4;
            return true;
        }

        private static List<WarpPipeRecord> ParseWarpPipes(string value)
        {
            List<WarpPipeRecord> result = new List<WarpPipeRecord>();
            foreach (Match match in WarpPipePattern.Matches(value))
            {
                result.Add(new WarpPipeRecord(
                    ParseGroup(match, "x"),
                    ParseGroup(match, "y"),
                    ParseGroup(match, "targetX"),
                    ParseGroup(match, "targetY"),
                    ParseGroup(match, "levelX"),
                    ParseGroup(match, "levelY"),
                    match.Groups["entry"].Value,
                    match.Groups["exit"].Value,
                    ParseGroup(match, "cameraX"),
                    ParseGroup(match, "cameraY"),
                    bool.Parse(match.Groups["freeze"].Value),
                    match.Groups["background"].Value,
                    match.Groups["levelType"].Value));
            }

            return result;
        }

        private static List<WarpPipeRecord> ParseAreaWarpPipes(string value)
        {
            List<WarpPipeRecord> result = new List<WarpPipeRecord>();
            foreach (Match match in AreaWarpPipePattern.Matches(value))
            {
                result.Add(new WarpPipeRecord(
                    ParseGroup(match, "x"),
                    ParseGroup(match, "y"),
                    0,
                    0,
                    0,
                    0,
                    match.Groups["entry"].Value,
                    match.Groups["exit"].Value,
                    0,
                    0,
                    bool.Parse(match.Groups["freeze"].Value),
                    match.Groups["background"].Value,
                    match.Groups["levelType"].Value,
                    match.Groups["area"].Value,
                    match.Groups["spawn"].Value));
            }

            return result;
        }

        private sealed class WarpPipeRecord
        {
            public int X { get; }
            public int Y { get; }
            public int TargetX { get; }
            public int TargetY { get; }
            public int LevelX { get; }
            public int LevelY { get; }
            public string EntryDirection { get; }
            public string ExitDirection { get; }
            public int CameraX { get; }
            public int CameraY { get; }
            public bool FreezeCamera { get; }
            public string BackgroundColor { get; }
            public string LevelType { get; }
            public string DestinationArea { get; }
            public string DestinationSpawn { get; }

            public WarpPipeRecord(
                int x,
                int y,
                int targetX,
                int targetY,
                int levelX,
                int levelY,
                string entryDirection,
                string exitDirection,
                int cameraX,
                int cameraY,
                bool freezeCamera,
                string backgroundColor,
                string levelType,
                string destinationArea = null,
                string destinationSpawn = null)
            {
                X = x;
                Y = y;
                TargetX = targetX;
                TargetY = targetY;
                LevelX = levelX;
                LevelY = levelY;
                EntryDirection = entryDirection;
                ExitDirection = exitDirection;
                CameraX = cameraX;
                CameraY = cameraY;
                FreezeCamera = freezeCamera;
                BackgroundColor = backgroundColor;
                LevelType = levelType;
                DestinationArea = destinationArea;
                DestinationSpawn = destinationSpawn;
            }
        }
    }
}
