using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.World
{
    internal static class LevelLayouts
    {
        private const int TileSize = 48;
        private const int FloorY = 528;
        private const int AreaHeight = 720;
        private const string OverworldMusic = "Audio/Tracks/overworldMusic";
        private const string UndergroundMusic = "Audio/Tracks/undergroundTheme";
        private const string BowserMusic = "Audio/Tracks/castleTheme";

        public static IReadOnlyDictionary<string, LevelDefinition> CreateAreas()
        {
            Dictionary<string, LevelDefinition> areas = new Dictionary<string, LevelDefinition>();
            areas.Add("overworld", CreateOverworld());
            areas.Add("underground", CreateUnderground());
            areas.Add("test", CreateTestArea());
            areas.Add("bowser-party", CreateBowserParty());
            return areas;
        }

        private static LevelDefinition CreateOverworld()
        {
            Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>
            {
                { "start", new Vector2(96, FloorY) },
                { "first-pipe", new Vector2(928, 398) },
                { "second-pipe", new Vector2(1984, 398) },
                { "finish", new Vector2(3120, FloorY) }
            };

            List<BlockSpawnDefinition> blocks = new List<BlockSpawnDefinition>();
            AddBlockRows(blocks, BlockType.Ground, 0, FloorY, columns: 63, rows: 4);
            AddBlockRows(blocks, BlockType.Ground, 3072, FloorY, columns: 16, rows: 4);
            blocks.Add(new BlockSpawnDefinition(new Vector2(384, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(432, 336), BlockType.Question, contents: ItemType.BlockCoin));
            blocks.Add(new BlockSpawnDefinition(new Vector2(480, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(528, 336), BlockType.Invisible, contents: ItemType.OneUpMushroom));
            blocks.Add(new BlockSpawnDefinition(new Vector2(720, 240), BlockType.Vine, height: 288));
            blocks.Add(new BlockSpawnDefinition(new Vector2(336, 432), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(384, 432), BlockType.Question, contents: ItemType.Mushroom));
            blocks.Add(new BlockSpawnDefinition(new Vector2(432, 432), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(912, 398), BlockType.Pipe, width: 64, height: 130,
                destinationArea: "underground", destinationSpawn: "entrance"));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1392, 336), BlockType.Question, contents: ItemType.FireFlower));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1440, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1488, 336), BlockType.Question, contents: ItemType.BlockCoin));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1968, 398), BlockType.Pipe, width: 64, height: 130,
                destinationArea: "underground", destinationSpawn: "exit"));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2496, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2544, 336), BlockType.Question, contents: ItemType.Star));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2592, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(3504, 30), BlockType.FlagPole, height: 498));

            ItemSpawnDefinition[] items = new ItemSpawnDefinition[]
            {
                new ItemSpawnDefinition(ItemType.Mushroom, new Vector2(224, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.OneUpMushroom, new Vector2(304, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.Star, new Vector2(560, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(400, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(448, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(496, 256)),
                new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(1424, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(2512, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(2560, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(2608, 256))
            };

            EnemySpawnDefinition[] enemies = new EnemySpawnDefinition[]
            {
                // this patrol reaches the first pipe before its normal turn point
                new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(840, FloorY - Goomba.GoombaHeight)),
                new EnemySpawnDefinition(EnemyType.Koopa, new Vector2(1680, FloorY - Koopa.KoopaHeight)),
                new EnemySpawnDefinition(EnemyType.HammerBro, new Vector2(2208, FloorY - HammerBro.HammerBroHeight)),
                // this patrol reaches the gap before its normal turn point
                new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(2944, FloorY - Goomba.GoombaHeight)),
                new EnemySpawnDefinition(EnemyType.Bowser, new Vector2(3264, FloorY - Bowser.BowserHeight))
            };

            return new LevelDefinition("overworld", 3840, AreaHeight, spawnPoints, blocks, items, enemies,
                nextAreaId: "bowser-party", 
                musicTrack: OverworldMusic);
        }

        private static LevelDefinition CreateUnderground()
        {
            Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>
            {
                { "start", new Vector2(240, FloorY) },
                { "entrance", new Vector2(112, 398) },
                { "exit", new Vector2(1744, 398) }
            };

            List<BlockSpawnDefinition> blocks = new List<BlockSpawnDefinition>();
            AddBlockRows(blocks, BlockType.Ground, 0, FloorY, columns: 40, rows: 4);
            AddBlockRows(blocks, BlockType.Solid, 0, 0, columns: 40, rows: 2);
            blocks.Add(new BlockSpawnDefinition(new Vector2(96, 398), BlockType.Pipe, width: 64, height: 130,
                destinationArea: "overworld", destinationSpawn: "first-pipe"));
            AddBlockRows(blocks, BlockType.Brick, 432, 336, columns: 3, rows: 1);
            blocks.Add(new BlockSpawnDefinition(new Vector2(576, 336), BlockType.Question, contents: ItemType.BlockCoin));
            blocks.Add(new BlockSpawnDefinition(new Vector2(864, 384), BlockType.Platform, width: 192, height: 24));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1200, 336), BlockType.Question, contents: ItemType.FireFlower));
            AddBlockRows(blocks, BlockType.Brick, 1248, 336, columns: 3, rows: 1);
            blocks.Add(new BlockSpawnDefinition(new Vector2(1728, 398), BlockType.Pipe, width: 64, height: 130,
                destinationArea: "overworld", destinationSpawn: "second-pipe"));

            ItemSpawnDefinition[] items = new ItemSpawnDefinition[]
            {
                new ItemSpawnDefinition(ItemType.Mushroom, new Vector2(304, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(448, 288)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(496, 288)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(544, 288)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(896, 336)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(944, 336)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(992, 336)),
                new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(1440, FloorY - 32))
            };

            EnemySpawnDefinition[] enemies = new EnemySpawnDefinition[]
            {
                new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(736, FloorY - Goomba.GoombaHeight)),
                new EnemySpawnDefinition(EnemyType.Koopa, new Vector2(1536, FloorY - Koopa.KoopaHeight))
            };

            return new LevelDefinition("underground", 1920, AreaHeight, spawnPoints, blocks, items, enemies,
                levelStartAreaId: "overworld", 
                musicTrack: UndergroundMusic);
        }

        private static LevelDefinition CreateTestArea()
        {
            Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>
            {
                { "start", new Vector2(96, FloorY) },
                { "items", new Vector2(832, FloorY) },
                { "combat", new Vector2(1680, 432) },
                { "ledge", new Vector2(2656, FloorY) },
                { "bowser", new Vector2(3120, FloorY) },
                { "pipes", new Vector2(3872, 398) }
            };

            List<BlockSpawnDefinition> blocks = new List<BlockSpawnDefinition>();
            List<ItemSpawnDefinition> items = new List<ItemSpawnDefinition>();
            List<EnemySpawnDefinition> enemies = new List<EnemySpawnDefinition>();

            AddBlockRows(blocks, BlockType.Ground, 0, FloorY, columns: 58, rows: 4);
            AddBlockRows(blocks, BlockType.Ground, 2880, FloorY, columns: 36, rows: 4);
            AddBlockBumpSpot(blocks, items, enemies);
            AddItemSpot(blocks, items);
            AddShellSpot(blocks, items, enemies);
            enemies.Add(new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(2896, FloorY - Goomba.GoombaHeight)));
            AddBowserSpot(blocks, items, enemies);
            AddPipeSpot(blocks);

            return new LevelDefinition("test", 4608, AreaHeight, spawnPoints, blocks, items, enemies,
                musicTrack: OverworldMusic);
        }

        private static void AddBlockBumpSpot(
            List<BlockSpawnDefinition> blocks, List<ItemSpawnDefinition> items, List<EnemySpawnDefinition> enemies)
        {
            blocks.Add(new BlockSpawnDefinition(new Vector2(96, 384), BlockType.Question, contents: ItemType.BlockCoin));
            blocks.Add(new BlockSpawnDefinition(new Vector2(192, 384), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(288, 384), BlockType.Used));
            blocks.Add(new BlockSpawnDefinition(new Vector2(384, 384), BlockType.Solid));
            blocks.Add(new BlockSpawnDefinition(new Vector2(576, 384), BlockType.Empty));
            blocks.Add(new BlockSpawnDefinition(new Vector2(576, 432), BlockType.Solid, width: 96));
            items.Add(new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(608, FloorY - 32)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(104, 384 - Goomba.GoombaHeight)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(200, 384 - Goomba.GoombaHeight)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(296, 384 - Goomba.GoombaHeight)));
            blocks.Add(new BlockSpawnDefinition(new Vector2(448, 496), BlockType.Pipe, width: 64, height: 32));
            enemies.Add(new EnemySpawnDefinition(EnemyType.PiranhaPlant, new Vector2(464, 496 - PiranhaPlant.PiranhaHeight)));
        }

        private static void AddItemSpot(List<BlockSpawnDefinition> blocks, List<ItemSpawnDefinition> items)
        {
            blocks.Add(new BlockSpawnDefinition(new Vector2(864, 384), BlockType.Invisible, contents: ItemType.OneUpMushroom));
            items.Add(new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(872, 448)));
            AddBlockRows(blocks, BlockType.Solid, 1008, 432, columns: 1, rows: 2);
            AddBlockRows(blocks, BlockType.Solid, 1344, 432, columns: 1, rows: 2);
            items.Add(new ItemSpawnDefinition(ItemType.Mushroom, new Vector2(1216, FloorY - 32)));
            items.Add(new ItemSpawnDefinition(ItemType.Star, new Vector2(1120, FloorY - 32)));
            items.Add(new ItemSpawnDefinition(ItemType.OneUpMushroom, new Vector2(1072, FloorY - 32)));
            items.Add(new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(936, FloorY - 32)));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1056, 336), BlockType.Question, contents: ItemType.Mushroom));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1200, 336), BlockType.Question, contents: ItemType.FireFlower));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1296, 336), BlockType.Question, contents: ItemType.Star));
            blocks.Add(new BlockSpawnDefinition(new Vector2(672, 384), BlockType.Platform, width: 144, height: 24));
        }

        private static void AddShellSpot(
            List<BlockSpawnDefinition> blocks, List<ItemSpawnDefinition> items, List<EnemySpawnDefinition> enemies)
        {
            blocks.Add(new BlockSpawnDefinition(new Vector2(1632, 432), BlockType.Platform, width: 112, height: 24));
            items.Add(new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(1584, FloorY - 32)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Koopa, new Vector2(1840, FloorY - Koopa.KoopaHeight)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(2048, FloorY - Goomba.GoombaHeight)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Koopa, new Vector2(2160, FloorY - Koopa.KoopaHeight)));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2304, FloorY - TileSize), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2400, FloorY - TileSize), BlockType.Question, contents: ItemType.BlockCoin));
            enemies.Add(new EnemySpawnDefinition(EnemyType.HammerBro, new Vector2(2480, FloorY - HammerBro.HammerBroHeight)));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2448, 456), BlockType.Platform, width: 96, height: 24));
            AddBlockRows(blocks, BlockType.Solid, 2544, 432, columns: 1, rows: 2);
        }

        private static void AddBowserSpot(
            List<BlockSpawnDefinition> blocks, List<ItemSpawnDefinition> items, List<EnemySpawnDefinition> enemies)
        {
            AddBlockRows(blocks, BlockType.Solid, 3024, 432, columns: 1, rows: 2);
            items.Add(new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(3200, FloorY - 32)));
            enemies.Add(new EnemySpawnDefinition(EnemyType.Bowser, new Vector2(3504, FloorY - Bowser.BowserHeight)));
            AddBlockRows(blocks, BlockType.Solid, 3712, 432, columns: 1, rows: 2);
        }

        private static void AddPipeSpot(List<BlockSpawnDefinition> blocks)
        {
            blocks.Add(new BlockSpawnDefinition(new Vector2(3856, 398), BlockType.Pipe, width: 64, height: 130,
                destinationArea: "overworld", destinationSpawn: "start"));
            blocks.Add(new BlockSpawnDefinition(new Vector2(4000, 432), BlockType.Pipe, width: 64, height: 96));
            blocks.Add(new BlockSpawnDefinition(new Vector2(4152, 240), BlockType.Vine, height: 288));
            blocks.Add(new BlockSpawnDefinition(new Vector2(4200, 240), BlockType.Platform, width: 144, height: 24));
            blocks.Add(new BlockSpawnDefinition(new Vector2(4464, 30), BlockType.FlagPole, height: 498));
        }

        private static LevelDefinition CreateBowserParty()
        {
            Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>
            {
                { "start", new Vector2(96, FloorY) },
                { "finish", new Vector2(2880, FloorY) }
            };

            List<BlockSpawnDefinition> blocks = new List<BlockSpawnDefinition>();
            List<EnemySpawnDefinition> enemies = new List<EnemySpawnDefinition>();
            AddBlockRows(blocks, BlockType.Ground, 0, FloorY, columns: 66, rows: 4);

            for (int row = 0; row < 5; row = row + 1)
            {
                for (int column = 0; column < 10; column = column + 1)
                {
                    int platformX = 384 + column * 240;
                    int platformY = FloorY - row * 96;
                    blocks.Add(new BlockSpawnDefinition(
                        new Vector2(platformX, platformY), BlockType.Platform, width: 192, height: 24));
                    enemies.Add(new EnemySpawnDefinition(
                        EnemyType.Bowser, new Vector2(platformX + 40, platformY - Bowser.BowserHeight)));
                }
            }

            blocks.Add(new BlockSpawnDefinition(new Vector2(3072, 30), BlockType.FlagPole, height: 498));

            ItemSpawnDefinition[] items = new ItemSpawnDefinition[]
            {
                new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(192, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.Star, new Vector2(240, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(1056, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.Star, new Vector2(1200, FloorY - 32)),
                new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(2016, FloorY - 32))
            };

            return new LevelDefinition("bowser-party", 3168, AreaHeight, spawnPoints, blocks, items, enemies, 
                musicTrack: BowserMusic);
        }

        private static void AddBlockRows(
            List<BlockSpawnDefinition> blocks,
            BlockType type,
            int startX,
            int startY,
            int columns,
            int rows)
        {
            for (int row = 0; row < rows; row = row + 1)
            {
                for (int column = 0; column < columns; column = column + 1)
                {
                    Vector2 position = new Vector2(startX + column * TileSize, startY + row * TileSize);
                    blocks.Add(new BlockSpawnDefinition(position, type));
                }
            }
        }
    }
}
