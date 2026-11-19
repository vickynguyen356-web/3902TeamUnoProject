using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using static TeamUno.Mario.World.LevelLayerReader;
using static TeamUno.Mario.World.LevelProperties;
using static TeamUno.Mario.World.WarpPipeParser;
using static TeamUno.Mario.World.BlockSpawnParser;
using static TeamUno.Mario.World.ItemSpawnParser;
using static TeamUno.Mario.World.EnemySpawnParser;
using static TeamUno.Mario.World.PropertySpawnParser;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Entry point of level loading: discovers the level folders, orchestrates the specialised parsers and validates the result.
    /// </summary>
    internal static class LevelParser
    {
        public static IReadOnlyDictionary<string, LevelDefinition> LoadAreas()
        {
            string dataDirectory = Path.Combine(AppContext.BaseDirectory, "World", "LevelData");
            if (!Directory.Exists(dataDirectory))
            {
                dataDirectory = Path.Combine(Environment.CurrentDirectory, "World", "LevelData");
            }

            return LoadAreas(dataDirectory);
        }

        public static IReadOnlyDictionary<string, LevelDefinition> LoadAreas(string dataDirectory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
            if (!Directory.Exists(dataDirectory))
            {
                throw new DirectoryNotFoundException("Level data directory was not found: " + dataDirectory);
            }

            Dictionary<string, LevelDefinition> areas = new Dictionary<string, LevelDefinition>(
                StringComparer.OrdinalIgnoreCase);
            foreach (string levelDirectory in Directory.GetDirectories(dataDirectory))
            {
                string levelId = Path.GetFileName(levelDirectory);
                string propertiesPath = Path.Combine(levelDirectory, levelId + ".levelproperties");
                if (!File.Exists(propertiesPath))
                {
                    continue;
                }

                LevelDefinition definition = ParseLevel(levelDirectory, levelId);
                areas.Add(definition.Id, definition);
            }

            if (areas.Count == 0)
            {
                throw new InvalidDataException("No level folders with matching .levelproperties files were found in "
                    + dataDirectory);
            }

            foreach (KeyValuePair<string, LevelDefinition> area in LevelLayouts.CreateAreas())
            {
                if (!areas.ContainsKey(area.Key))
                {
                    areas.Add(area.Key, area.Value);
                }
            }

            foreach (LevelDefinition definition in areas.Values)
            {
                if (definition.NextAreaId != null && !areas.ContainsKey(definition.NextAreaId))
                {
                    throw new InvalidDataException(
                        "Level '" + definition.Id + "' points to missing next area '" + definition.NextAreaId + "'");
                }

                foreach (BlockSpawnDefinition block in definition.Blocks)
                {
                    if (block.DestinationArea != null && !areas.ContainsKey(block.DestinationArea))
                    {
                        throw new InvalidDataException(
                            "Level '" + definition.Id + "' contains a pipe to missing area '"
                            + block.DestinationArea + "'");
                    }
                }
            }

            return areas;
        }

        private static LevelDefinition ParseLevel(string levelDirectory, string levelId)
        {
            string propertiesPath = Path.Combine(levelDirectory, levelId + ".levelproperties");
            string properties = File.ReadAllText(propertiesPath);
            int[][] foreground = ReadGrid(Path.Combine(levelDirectory, levelId + "_Foreground.csv"));
            int[][] background = ReadOptionalGrid(Path.Combine(levelDirectory, levelId + "_Background.csv"));
            int[][] aboveForeground = ReadOptionalGrid(
                Path.Combine(levelDirectory, levelId + "_Above_Foreground.csv"));
            int[][] underground = ReadOptionalGrid(Path.Combine(levelDirectory, levelId + "_Underground.csv"));
            int[][] enemyGrid = ReadOptionalGrid(Path.Combine(levelDirectory, levelId + "_Enemies.csv"));
            int[][] collectibleGrid = ReadOptionalGrid(Path.Combine(levelDirectory, levelId + "_Collectibles.csv"));
            int[][] itemGrid = ReadOptionalGrid(Path.Combine(levelDirectory, levelId + "_Items.csv"));
            string levelType = GetSingleProperty(properties, "LEVEL_TYPE");
            bool isUnderground = levelType.Equals("UNDERGROUND", StringComparison.OrdinalIgnoreCase);
            bool isUnderwater = levelType.Contains("UNDERWATER", StringComparison.OrdinalIgnoreCase);
            List<MovingPlatformSpawnDefinition> movingPlatforms = ParseMovingPlatforms(
                GetProperty(properties, "MOVING_PLATFORM"),
                foreground,
                underground,
                isUnderground);
            List<PlatformLevelSpawnDefinition> platformLevels = ParsePlatformLevels(
                GetProperty(properties, "PLATFORM_LEVEL"));
            List<VineSpawnDefinition> vines = ParseVines(GetProperty(properties, "VINE"));

            List<BlockSpawnDefinition> blocks = new List<BlockSpawnDefinition>();
            List<ItemSpawnDefinition> items = new List<ItemSpawnDefinition>();
            List<EnemySpawnDefinition> enemies = new List<EnemySpawnDefinition>();
            Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>(StringComparer.OrdinalIgnoreCase);
            int widthInTiles = foreground[0].Length;
            int heightInTiles = foreground.Length;

            EnsureSameDimensions(foreground, background, levelId + "_Background.csv");
            EnsureSameDimensions(foreground, aboveForeground, levelId + "_Above_Foreground.csv");
            EnsureSameDimensions(foreground, underground, levelId + "_Underground.csv");
            EnsureSameDimensions(foreground, enemyGrid, levelId + "_Enemies.csv");
            EnsureSameDimensions(foreground, collectibleGrid, levelId + "_Collectibles.csv");
            EnsureSameDimensions(foreground, itemGrid, levelId + "_Items.csv");

            Tuple<int, int> start = ReadRequiredTuple(properties, "PLAYER_START", propertiesPath);
            Tuple<int, int> cameraStart = ReadRequiredTuple(properties, "CAMERA_START", propertiesPath);
            spawnPoints.Add("start", ToSpawnPoint(start.Item1, start.Item2));
            AddSpawnPoints(properties, levelId, spawnPoints);

            HashSet<long> foregroundPipeCells = new HashSet<long>();
            HashSet<long> undergroundPipeCells = new HashSet<long>();
            AddWarpPipeBlocks(
                properties,
                levelId,
                foreground,
                underground,
                isUnderground,
                blocks,
                foregroundPipeCells,
                undergroundPipeCells);

            HashSet<long> hiddenCollectibleCells = GetHiddenCollectibleCells(
                foreground, underground, collectibleGrid);
            HashSet<long> foregroundCoinCells = new HashSet<long>();
            HashSet<long> undergroundCoinCells = new HashSet<long>();
            AddMapBlocks(
                foreground,
                blocks,
                foregroundPipeCells,
                isUnderground,
                isUnderwater,
                items,
                foregroundCoinCells,
                hiddenCollectibleCells);
            if (underground != null)
            {
                AddMapBlocks(
                    underground,
                    blocks,
                    undergroundPipeCells,
                    isUnderground: true,
                    isUnderwater: false,
                    items,
                    undergroundCoinCells,
                    hiddenCollectibleCells);
            }

            AddFlagpoles(foreground, blocks, isUnderground);
            if (underground != null)
            {
                AddFlagpoles(underground, blocks, isUnderground: true);
            }
            AddCollectibles(foreground, underground, collectibleGrid, items, blocks, vines, isUnderground);
            AddPlacedItems(itemGrid, items);
            AddEnemies(enemyGrid, background, enemies);
            AddCannonSpawners(foreground, enemies);
            AddCannonSpawners(underground, enemies);

            HashSet<long> modeledForegroundCells = GetModeledBlockCells(
                blocks,
                movingPlatforms,
                platformLevels,
                isUnderground);
            HashSet<long> modeledUndergroundCells = GetModeledBlockCells(
                blocks,
                movingPlatforms,
                platformLevels,
                isUnderground: true);
            modeledForegroundCells.UnionWith(foregroundCoinCells);
            modeledUndergroundCells.UnionWith(undergroundCoinCells);
            List<LevelTile> backgroundTiles = GetLayerTiles(background, null);
            List<LevelTile> foregroundTiles = new List<LevelTile>();
            AddLayerTiles(underground, modeledUndergroundCells, foregroundTiles);
            AddLayerTiles(foreground, modeledForegroundCells, foregroundTiles);
            List<LevelTile> aboveForegroundTiles = GetLayerTiles(aboveForeground, null);

            string nextAreaId = ReadNextArea(properties);
            int width = checked(widthInTiles * TileSize);
            int height = checked(heightInTiles * TileSize);
            string musicTrack = GetOptionalSingleProperty(properties, "MUSIC_TRACK")
                ?? GetMusicTrack(levelType);

            return new LevelDefinition(
                levelId,
                width,
                height,
                spawnPoints,
                blocks,
                items,
                enemies,
                nextAreaId: nextAreaId,
                levelStartAreaId: GetOptionalSingleProperty(properties, "LEVEL_START_AREA_ID"),
                musicTrack: musicTrack,
                isUnderwater: isUnderwater,
                movingPlatforms: movingPlatforms,
                fireBars: ParseFireBars(GetProperty(properties, "FIRE_BAR")),
                platformLevels: platformLevels,
                floatingTexts: ParseFloatingTexts(GetProperty(properties, "FLOATING_TEXT")),
                vines: vines,
                isUnderground: isUnderground,
                backgroundColor: ParseBackgroundColor(GetSingleProperty(properties, "BACKGROUND_COLOR")),
                backgroundTiles: backgroundTiles,
                foregroundTiles: foregroundTiles,
                aboveForegroundTiles: aboveForegroundTiles,
                cameraStart: new Vector2(cameraStart.Item1 * TileSize, cameraStart.Item2 * TileSize));
        }
    }
}
