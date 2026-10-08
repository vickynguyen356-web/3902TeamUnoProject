using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Player;

namespace TeamUno.Mario.World
{
    internal static class LevelLayouts
    {
        public static LevelDefinition CreateFirstLevel()
        {
            const int tileSize = 48;
            const int width = 3840;
            const int height = 720;
            const int floorY = 528;

            List<BlockSpawnDefinition> blocks = new List<BlockSpawnDefinition>();
            for (int y = floorY; y < height; y = y + tileSize)
            {
                for (int x = 0; x < width; x = x + tileSize)
                {
                    blocks.Add(new BlockSpawnDefinition(new Vector2(x, y), BlockType.Ground));
                }
            }

            blocks.Add(new BlockSpawnDefinition(new Vector2(384, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(432, 336), BlockType.Question));
            blocks.Add(new BlockSpawnDefinition(new Vector2(480, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(912, floorY - 130), BlockType.Pipe, 64, 130));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1392, 336), BlockType.Question));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1440, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1488, 336), BlockType.Question));
            blocks.Add(new BlockSpawnDefinition(new Vector2(1968, floorY - 130), BlockType.Pipe, 64, 130));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2496, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2544, 336), BlockType.Question));
            blocks.Add(new BlockSpawnDefinition(new Vector2(2592, 336), BlockType.Brick));
            blocks.Add(new BlockSpawnDefinition(new Vector2(3504, floorY - 498), BlockType.FlagPole, 48, 498));

            ItemSpawnDefinition[] items = new ItemSpawnDefinition[]
            {
                new ItemSpawnDefinition(ItemType.Mushroom, new Vector2(224, floorY - 32)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(400, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(448, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(496, 256)),
                new ItemSpawnDefinition(ItemType.FireFlower, new Vector2(1424, floorY - 32)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(2512, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(2560, 256)),
                new ItemSpawnDefinition(ItemType.FloatingCoin, new Vector2(2608, 256))
            };

            EnemySpawnDefinition[] enemies = new EnemySpawnDefinition[]
            {
                new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(720, floorY - Goomba.GoombaHeight)),
                new EnemySpawnDefinition(EnemyType.Koopa, new Vector2(1680, floorY - Koopa.KoopaHeight)),
                new EnemySpawnDefinition(EnemyType.HammerBro, new Vector2(2208, floorY - HammerBro.HammerBroHeight)),
                new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(2880, floorY - Goomba.GoombaHeight)),
                new EnemySpawnDefinition(EnemyType.Bowser, new Vector2(3264, floorY - Bowser.BowserHeight))
            };

            Vector2 playerSpawnPosition = new Vector2(96, floorY - MarioPlayer.StandingHeight);
            return new LevelDefinition(width, height, playerSpawnPosition, blocks, items, enemies);
        }
    }
}
