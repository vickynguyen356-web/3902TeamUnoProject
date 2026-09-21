using System.Collections.Generic;

namespace Sprint0.World
{
    public class LevelDefinition
    {
        public IReadOnlyList<PlatformDefinition> Platforms { get; }
        public IReadOnlyList<CoinLineDefinition> CoinLines { get; }
        public IReadOnlyList<EnemySpawnDefinition> Enemies { get; }
        public GoalDefinition Goal { get; }

        public LevelDefinition(
            IReadOnlyList<PlatformDefinition> platforms,
            IReadOnlyList<CoinLineDefinition> coinLines,
            IReadOnlyList<EnemySpawnDefinition> enemies,
            GoalDefinition goal)
        {
            Platforms = new List<PlatformDefinition>(platforms).AsReadOnly();
            CoinLines = new List<CoinLineDefinition>(coinLines).AsReadOnly();
            Enemies = new List<EnemySpawnDefinition>(enemies).AsReadOnly();
            Goal = goal;
        }

        public static LevelDefinition CreateDefault()
        {
            // Add the level layout here.
            return new LevelDefinition(
                new List<PlatformDefinition>(),
                new List<CoinLineDefinition>(),
                new List<EnemySpawnDefinition>(),
                new GoalDefinition(0, 0));
        }
    }

    public struct PlatformDefinition
    {
        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public int Length { get; private set; }

        public PlatformDefinition(int tileX, int tileY, int length)
        {
            TileX = tileX;
            TileY = tileY;
            Length = length;
        }
    }

    public struct CoinLineDefinition
    {
        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public int Count { get; private set; }

        public CoinLineDefinition(int tileX, int tileY, int count)
        {
            TileX = tileX;
            TileY = tileY;
            Count = count;
        }
    }

    public struct EnemySpawnDefinition
    {
        public int TileX { get; private set; }

        public EnemySpawnDefinition(int tileX)
        {
            TileX = tileX;
        }
    }

    public struct GoalDefinition
    {
        public int TileX { get; private set; }
        public int HeightInTiles { get; private set; }

        public GoalDefinition(int tileX, int heightInTiles)
        {
            TileX = tileX;
            HeightInTiles = heightInTiles;
        }
    }
}
