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
            var platforms = new List<PlatformDefinition>
            {
                new PlatformDefinition(0, 11, 18),
                new PlatformDefinition(20, 10, 5),
                new PlatformDefinition(27, 9, 4),
                new PlatformDefinition(34, 8, 3),
                new PlatformDefinition(40, 9, 5),
                new PlatformDefinition(48, 10, 6),
                new PlatformDefinition(58, 11, 25)
            };

            var coinLines = new List<CoinLineDefinition>
            {
                new CoinLineDefinition(21, 8, 3),
                new CoinLineDefinition(35, 6, 3),
                new CoinLineDefinition(49, 8, 3)
            };

            var enemies = new List<EnemySpawnDefinition>
            {
                new EnemySpawnDefinition(30),
                new EnemySpawnDefinition(52)
            };

            return new LevelDefinition(
                platforms,
                coinLines,
                enemies,
                new GoalDefinition(70, 2));
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
