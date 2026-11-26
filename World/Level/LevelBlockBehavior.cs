using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal static class LevelBlockBehavior
    {
        private const int TileSize = 48;

        public static int CollectItems(
            LevelDefinition definition,
            IList<IBlock> blocks,
            IList<IItem> items)
        {
            int coinsCollected = 0;
            List<IBlock> newVines = new List<IBlock>();

            foreach (IBlock block in blocks)
            {
                ItemType? contents = block.CollectItem();
                if (!contents.HasValue)
                {
                    continue;
                }

                if (contents.Value == ItemType.Vine)
                {
                    VineSpawnDefinition vine = FindVineSpawn(
                        definition,
                        block.Position);
                    if (vine == null)
                    {
                        throw new InvalidOperationException(
                            "A vine block was hit without matching VINE metadata in level '"
                            + definition.Id + "'.");
                    }

                    newVines.Add(new Block(
                        block.Position,
                        BlockType.Vine,
                        width: TileSize,
                        height: TileSize,
                        isUnderground: block.IsUnderground,
                        isUnderwater: definition.IsUnderwater,
                        vineSpawn: vine));
                    continue;
                }

                IItem item = ItemFactory.Create(contents.Value, block.Position);
                Vector2 position = new Vector2(
                    block.Bounds.Center.X - item.Bounds.Width / 2f,
                    block.Bounds.Top - item.Bounds.Height);
                item.ApplyMotion(position, item.CurrentVelocity);
                items.Add(item);

                if (contents.Value == ItemType.BlockCoin)
                {
                    coinsCollected++;
                }
            }

            foreach (IBlock vine in newVines)
            {
                blocks.Add(vine);
            }

            return coinsCollected;
        }

        public static VineSpawnDefinition GetVineTravelRequest(
            IEnumerable<IBlock> blocks)
        {
            foreach (IBlock block in blocks)
            {
                VineSpawnDefinition vine = block.ConsumeVineTravelRequest();
                if (vine != null)
                {
                    return vine;
                }
            }

            return null;
        }

        public static void CarryPlayerWithMovingPlatforms(
            IReadOnlyList<IBlock> blocks,
            MarioPlayer player,
            Rectangle previousPlayerBounds)
        {
            if (player.IsDead || player.Velocity.Y < 0f)
            {
                return;
            }

            foreach (IBlock block in blocks)
            {
                Vector2 movement = block.MovementDelta;
                if (block.Type != BlockType.Platform
                    || (movement.X == 0f && movement.Y == 0f)
                    || Math.Abs(movement.X) > TileSize
                    || Math.Abs(movement.Y) > TileSize)
                {
                    continue;
                }

                Rectangle previousPlatformBounds = block.PreviousBounds;
                bool stoodOnPlatform =
                    previousPlayerBounds.Bottom >= previousPlatformBounds.Top - 1
                    && previousPlayerBounds.Bottom <= previousPlatformBounds.Top + 2
                    && previousPlayerBounds.Right > previousPlatformBounds.Left
                    && previousPlayerBounds.Left < previousPlatformBounds.Right;
                if (!stoodOnPlatform)
                {
                    continue;
                }

                Vector2 position = player.Position + movement;
                position.Y = block.Bounds.Top - player.Bounds.Height;
                Vector2 velocity = player.Velocity;
                velocity.Y = 0f;
                player.ApplyMotion(position, velocity, isGrounded: true);
                return;
            }
        }

        public static void BeginFlagpoleSequence(
            IReadOnlyList<IBlock> blocks)
        {
            foreach (IBlock flag in blocks)
            {
                if (flag.Type != BlockType.Flag)
                {
                    continue;
                }

                IBlock closestPole = null;
                int closestDistance = int.MaxValue;
                foreach (IBlock pole in blocks)
                {
                    if (pole.Type != BlockType.FlagPole)
                    {
                        continue;
                    }

                    int distance = Math.Abs(
                        flag.Bounds.Center.X - pole.Bounds.Center.X);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPole = pole;
                    }
                }

                if (closestPole != null)
                {
                    flag.BeginFlagpoleDescent(closestPole.Bounds.Bottom);
                }
            }
        }

        public static void UpdateFlagpoleFlags(
            IReadOnlyList<IBlock> blocks,
            GameTime gameTime)
        {
            float elapsedSeconds = Math.Min(
                1f / 30f,
                Math.Max(0f, (float)gameTime.ElapsedGameTime.TotalSeconds));
            foreach (IBlock block in blocks)
            {
                if (block.Type == BlockType.Flag)
                {
                    block.UpdateFlagpoleDescent(elapsedSeconds);
                }
            }
        }

        public static bool ResolveLevelFinishWalk(
            LevelDefinition definition,
            IReadOnlyList<IBlock> blocks,
            MarioPlayer player,
            Rectangle previousBounds)
        {
            Rectangle currentBounds = player.Bounds;
            foreach (IBlock block in blocks)
            {
                if (!IsLevelFinishSolid(block))
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                bool crossedWall =
                    previousBounds.Right <= blockBounds.Left
                    && currentBounds.Right >= blockBounds.Left
                    && currentBounds.Bottom > blockBounds.Top
                    && currentBounds.Top < blockBounds.Bottom;
                if (!crossedWall)
                {
                    continue;
                }

                Vector2 position = player.Position;
                position.X = blockBounds.Left - player.Bounds.Width;
                Vector2 velocity = player.Velocity;
                velocity.X = 0f;
                player.ApplyMotion(position, velocity, player.IsGrounded);
                return true;
            }

            IBlock support = null;
            int smallestGap = TileSize + 1;
            foreach (IBlock block in blocks)
            {
                if (!IsLevelFinishSolid(block)
                    && block.Type != BlockType.Platform)
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                int gap = blockBounds.Top - currentBounds.Bottom;
                bool horizontalSupport =
                    currentBounds.Right > blockBounds.Left
                    && currentBounds.Left < blockBounds.Right;
                if (horizontalSupport
                    && gap >= -1
                    && gap <= TileSize
                    && gap < smallestGap)
                {
                    smallestGap = Math.Max(0, gap);
                    support = block;
                }
            }

            if (support != null)
            {
                Vector2 position = player.Position;
                position.Y = support.Bounds.Top - player.Bounds.Height;
                Vector2 velocity = player.Velocity;
                velocity.Y = 0f;
                player.ApplyMotion(position, velocity, isGrounded: true);
            }

            if (player.Bounds.Right >= definition.Width)
            {
                Vector2 position = player.Position;
                position.X = definition.Width - player.Bounds.Width;
                player.ApplyMotion(position, Vector2.Zero, isGrounded: true);
                return true;
            }

            return false;
        }

        public static IBlock GetPipeTravelRequest(IList<IBlock> blocks)
        {
            foreach (IBlock block in blocks)
            {
                if (block.IsTravelRequested)
                {
                    block.ClearTravelRequest();
                    return block;
                }
            }

            return null;
        }

        public static bool TryActivateCastleAxe(
            bool hasBowser,
            bool isBowserDefeated,
            Rectangle playerBounds,
            IReadOnlyList<IBlock> blocks,
            IReadOnlyList<IEnemy> enemies)
        {
            if (!hasBowser || isBowserDefeated)
            {
                return false;
            }

            IBlock axe = null;
            foreach (IBlock block in blocks)
            {
                if (!block.IsDeleted
                    && block.Type == BlockType.Axe
                    && block.Bounds.Intersects(playerBounds))
                {
                    axe = block;
                    break;
                }
            }

            if (axe == null)
            {
                return false;
            }

            axe.MarkDeleted();
            foreach (IBlock block in blocks)
            {
                if (block.Type == BlockType.Bridge)
                {
                    block.MarkDeleted();
                }
            }

            bool defeatedBowser = false;
            foreach (IEnemy enemy in enemies)
            {
                if (enemy is Bowser bowser && !bowser.IsDead)
                {
                    while (!bowser.IsDead)
                    {
                        bowser.TakeDamage();
                    }

                    defeatedBowser = true;
                }
            }

            return defeatedBowser;
        }

        private static VineSpawnDefinition FindVineSpawn(
            LevelDefinition definition,
            Vector2 position)
        {
            foreach (VineSpawnDefinition vine in definition.Vines)
            {
                if (vine.Position == position)
                {
                    return vine;
                }
            }

            return null;
        }

        private static bool IsLevelFinishSolid(IBlock block)
        {
            return !block.IsDeleted
                && block.Type != BlockType.Empty
                && block.Type != BlockType.Invisible
                && block.Type != BlockType.Vine
                && block.Type != BlockType.FlagPole
                && block.Type != BlockType.Flag
                && block.Type != BlockType.Platform;
        }
    }
}
