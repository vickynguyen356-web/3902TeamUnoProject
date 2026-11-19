using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using static TeamUno.Mario.World.LevelLayerReader;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Reads the .levelproperties file: scalar keys (music track, background colour, next area, spawn points, coordinate tuples).
    /// </summary>
    internal static class LevelProperties
    {
        private static readonly Regex TuplePattern = new Regex(@"\((\d+)\s*,\s*(\d+)\)", RegexOptions.Compiled);

        private static readonly Regex SpawnPointPattern = new Regex(
            @"(?<name>[A-Za-z][A-Za-z0-9_-]*)\s*\((?<x>\d+)\s*,\s*(?<y>\d+)\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static void AddSpawnPoints(
            string properties,
            string levelId,
            Dictionary<string, Vector2> spawnPoints)
        {
            foreach (Match match in SpawnPointPattern.Matches(GetProperty(properties, "SPAWN_POINT")))
            {
                string spawnName = match.Groups["name"].Value;
                Vector2 spawnPosition = ToSpawnPoint(
                    ParseGroup(match, "x"),
                    ParseGroup(match, "y"));
                if (!spawnPoints.TryAdd(spawnName, spawnPosition))
                {
                    throw new InvalidDataException(
                        "Level '" + levelId + "' defines the spawn point '" + spawnName + "' more than once.");
                }
            }
        }

        internal static Color ParseBackgroundColor(string value)
        {
            if (value.Equals("BLACK", StringComparison.OrdinalIgnoreCase))
            {
                return Color.Black;
            }

            if (value.Equals("BLUE", StringComparison.OrdinalIgnoreCase))
            {
                return new Color(92, 148, 252);
            }

            throw new InvalidDataException("Unsupported background color: " + value);
        }

        internal static string GetMusicTrack(string levelType)
        {
            if (levelType.Contains("UNDERWATER", StringComparison.OrdinalIgnoreCase))
            {
                return "Audio/Tracks/underwaterTheme";
            }

            if (levelType.Equals("UNDERGROUND", StringComparison.OrdinalIgnoreCase))
            {
                return "Audio/Tracks/undergroundTheme";
            }

            if (levelType.Equals("CASTLE", StringComparison.OrdinalIgnoreCase))
            {
                return "Audio/Tracks/castleTheme";
            }

            return "Audio/Tracks/overworldMusic";
        }

        internal static Vector2 ToSpawnPoint(int column, int row)
        {
            return new Vector2(column * TileSize, (row + 1) * TileSize);
        }

        internal static Tuple<int, int> ReadRequiredTuple(string properties, string key, string path)
        {
            string value = GetSingleProperty(properties, key);
            Match match = TuplePattern.Match(value);
            if (!match.Success)
            {
                throw new InvalidDataException("Property " + key + " must contain a coordinate pair in " + path);
            }

            return Tuple.Create(
                int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
        }

        internal static string ReadNextArea(string properties)
        {
            string namedArea = GetOptionalSingleProperty(properties, "NEXT_AREA_ID");
            if (!string.IsNullOrWhiteSpace(namedArea))
            {
                return namedArea;
            }

            string value = GetSingleProperty(properties, "NEXT_LEVEL");
            Match match = TuplePattern.Match(value);
            if (!match.Success)
            {
                throw new InvalidDataException("NEXT_LEVEL must contain a coordinate pair.");
            }

            int world = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            int level = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            return world == 0 || level == 0
                ? null
                : "World" + world.ToString(CultureInfo.InvariantCulture)
                    + "-" + level.ToString(CultureInfo.InvariantCulture);
        }

        internal static string GetOptionalSingleProperty(string properties, string key)
        {
            Match match = Regex.Match(
                properties,
                @"(?m)^\s*" + Regex.Escape(key) + @"\s*=\s*(?<value>[^\r\n\\]*)");
            return match.Success ? match.Groups["value"].Value.Trim() : null;
        }

        internal static string GetSingleProperty(string properties, string key)
        {
            Match match = Regex.Match(properties, @"(?m)^\s*" + Regex.Escape(key) + @"\s*=\s*(?<value>[^\r\n\\]*)");
            if (!match.Success)
            {
                throw new InvalidDataException("Required level property is missing: " + key);
            }

            return match.Groups["value"].Value.Trim();
        }

        internal static string GetProperty(string properties, string key)
        {
            Match match = Regex.Match(properties, @"(?m)^\s*" + Regex.Escape(key) + @"\s*=\s*(?<value>[^\r\n]*)");
            if (!match.Success)
            {
                return string.Empty;
            }

            string value = match.Groups["value"].Value;
            int nextLineStart = match.Index + match.Length;
            while (nextLineStart < properties.Length)
            {
                int lineEnd = properties.IndexOf('\n', nextLineStart);
                if (lineEnd < 0)
                {
                    lineEnd = properties.Length;
                }

                string line = properties.Substring(nextLineStart, lineEnd - nextLineStart).Trim();
                if (Regex.IsMatch(line, @"^[A-Z_]+\s*="))
                {
                    break;
                }

                value += " " + line.TrimEnd('\\').Trim();
                nextLineStart = lineEnd + 1;
            }

            return value.Replace("\\", string.Empty, StringComparison.Ordinal);
        }

        internal static int ParseGroup(Match match, string groupName)
        {
            return int.Parse(match.Groups[groupName].Value, CultureInfo.InvariantCulture);
        }
    }
}
