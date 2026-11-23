using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using static TeamUno.Mario.World.LevelLayerReader;
using static TeamUno.Mario.World.LevelProperties;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Parses the property sections that describe moving scenery and hazards: MOVING_PLATFORM, FIRE_BAR, VINE, PLATFORM_LEVEL and FLOATING_TEXT.
    /// </summary>
    internal static class PropertySpawnParser
    {
        private static readonly Regex MovingPlatformPattern = new Regex(
            @"\((?<x>\d+)\s*,\s*(?<y>\d+)\)\s+(?<motion>\w+)\s+(?<direction>\w+)\s+" +
            @"\((?<min>\d+)\s*,\s*(?<max>\d+)\)\s+(?<shift>\w+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex FireBarPattern = new Regex(
            @"\((?<x>\d+)\s*,\s*(?<y>\d+)\)\s+(?<angle>\d+)\s+" +
            @"(?<direction>\w+)\s+(?<length>\d+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PlatformLevelPattern = new Regex(
            @"\((?<leftX>\d+)\s*,\s*(?<leftY>\d+)\)\s*" +
            @"\((?<rightX>\d+)\s*,\s*(?<rightY>\d+)\)\s+(?<pulleyY>\d+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex FloatingTextPattern = new Regex(
            @"\((?<x>\d+)\s*,\s*(?<y>\d+)\)\s*\((?<text>[^)]*)\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex VinePattern = new Regex(
            @"\((?<x>\d+)\s*,\s*(?<y>\d+)\)\s*" +
            @"\((?<targetX>\d+)\s*,\s*(?<targetY>\d+)\)\s*" +
            @"\((?<cameraX>\d+)\s*,\s*(?<cameraY>\d+)\)\s+" +
            @"(?<resetY>\d+)\s*\((?<resetX>\d+)\s*,\s*(?<resetPositionY>\d+)\)\s+" +
            @"(?<cameraMax>\d+)\s+(?<background>\w+)\s+(?<levelType>\w+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static List<MovingPlatformSpawnDefinition> ParseMovingPlatforms(
            string value,
            int[][] foreground,
            int[][] underground,
            bool isUnderground)
        {
            List<MovingPlatformSpawnDefinition> result = new List<MovingPlatformSpawnDefinition>();
            foreach (Match match in MovingPlatformPattern.Matches(value))
            {
                int column = ParseGroup(match, "x");
                int row = ParseGroup(match, "y");
                int foregroundTile = GetLayerTile(foreground, column, row);
                int undergroundTile = GetLayerTile(underground, column, row);
                bool foregroundHasPlatform = IsMovingPlatformStartTile(foregroundTile);
                bool undergroundHasPlatform = IsMovingPlatformStartTile(undergroundTile);
                if (!foregroundHasPlatform && !undergroundHasPlatform)
                {
                    continue;
                }

                int platformTile = foregroundHasPlatform
                    ? foregroundTile
                    : undergroundHasPlatform ? undergroundTile : foregroundTile;
                int platformWidth = GetMovingPlatformWidth(platformTile);
                bool platformIsUnderground = isUnderground
                    || (!foregroundHasPlatform && undergroundHasPlatform);
                result.Add(new MovingPlatformSpawnDefinition(
                    new Vector2(column * TileSize, row * TileSize),
                    match.Groups["motion"].Value,
                    match.Groups["direction"].Value,
                    ParseGroup(match, "min"),
                    ParseGroup(match, "max"),
                    platformWidth,
                    bool.Parse(match.Groups["shift"].Value),
                    platformIsUnderground,
                    platformTile));
            }

            return result;
        }

        private static int GetMovingPlatformWidth(
            int tileId)
        {
            if (tileId == 761)
            {
                return 2;
            }

            if (tileId == 609)
            {
                return 1;
            }

            if (tileId == 809)
            {
                return 3;
            }

            return 3;
        }

        private static bool IsMovingPlatformStartTile(int tileId)
        {
            return tileId == 609 || tileId == 761 || tileId == 809;
        }

        internal static List<FireBarSpawnDefinition> ParseFireBars(string value)
        {
            List<FireBarSpawnDefinition> result = new List<FireBarSpawnDefinition>();
            foreach (Match match in FireBarPattern.Matches(value))
            {
                result.Add(new FireBarSpawnDefinition(
                    new Vector2(ParseGroup(match, "x") * TileSize, ParseGroup(match, "y") * TileSize),
                    ParseGroup(match, "angle"),
                    match.Groups["direction"].Value,
                    ParseGroup(match, "length")));
            }

            return result;
        }

        internal static List<VineSpawnDefinition> ParseVines(string value)
        {
            List<VineSpawnDefinition> result = new List<VineSpawnDefinition>();
            foreach (Match match in VinePattern.Matches(value))
            {
                string levelType = match.Groups["levelType"].Value;
                result.Add(new VineSpawnDefinition(
                    new Vector2(ParseGroup(match, "x") * TileSize, ParseGroup(match, "y") * TileSize),
                    new Vector2(
                        ParseGroup(match, "targetX") * TileSize,
                        ParseGroup(match, "targetY") * TileSize),
                    new Vector2(
                        ParseGroup(match, "cameraX") * TileSize,
                        ParseGroup(match, "cameraY") * TileSize),
                    ParseGroup(match, "resetY") * TileSize,
                    new Vector2(
                        ParseGroup(match, "resetX") * TileSize,
                        ParseGroup(match, "resetPositionY") * TileSize),
                    ParseGroup(match, "cameraMax") * TileSize,
                    ParseBackgroundColor(match.Groups["background"].Value),
                    levelType,
                    GetMusicTrack(levelType)));
            }

            return result;
        }

        internal static List<PlatformLevelSpawnDefinition> ParsePlatformLevels(string value)
        {
            List<PlatformLevelSpawnDefinition> result = new List<PlatformLevelSpawnDefinition>();
            foreach (Match match in PlatformLevelPattern.Matches(value))
            {
                result.Add(new PlatformLevelSpawnDefinition(
                    new Vector2(ParseGroup(match, "leftX") * TileSize, ParseGroup(match, "leftY") * TileSize),
                    new Vector2(ParseGroup(match, "rightX") * TileSize, ParseGroup(match, "rightY") * TileSize),
                    (ParseGroup(match, "pulleyY") + 1) * TileSize));
            }

            return result;
        }

        internal static List<FloatingTextSpawnDefinition> ParseFloatingTexts(string value)
        {
            List<FloatingTextSpawnDefinition> result = new List<FloatingTextSpawnDefinition>();
            foreach (Match match in FloatingTextPattern.Matches(value))
            {
                result.Add(new FloatingTextSpawnDefinition(
                    new Vector2(ParseGroup(match, "x") * TileSize, ParseGroup(match, "y") * TileSize),
                    match.Groups["text"].Value.Trim()));
            }

            return result;
        }
    }
}
