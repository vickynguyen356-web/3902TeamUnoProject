using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;

namespace TeamUno.Mario.World
{
    internal class CollisionSystem
    {
        public void Update(Level level, Rectangle previousPlayerBounds, bool isJumpHeld = false)
        {
            ResolveMarioFloorCollision(level.Player, level.Blocks, previousPlayerBounds);
        }

        private static void ResolveMarioFloorCollision(
            MarioPlayer player, IReadOnlyList<Block> blocks, Rectangle previousPlayerBounds)
        {
            if (player.IsDead || player.Velocity.Y < 0f)
            {
                return;
            }

            Rectangle playerBounds = player.Bounds;

            // Basic example for the floor
            // A full version needs to handle fast movement across multiple blocks,
            // corner and side hits, and rounding around tile eges
            foreach (Block block in blocks)
            {
                if (block.Type != BlockType.Ground)
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                bool overlapsHorizontally = playerBounds.Right > blockBounds.Left
                    && playerBounds.Left < blockBounds.Right;

                // His feet were above the tile and have now reached its top
                bool landed = previousPlayerBounds.Bottom <= blockBounds.Top
                    && playerBounds.Bottom >= blockBounds.Top;

                if (overlapsHorizontally && landed)
                {
                    // Put his feet on top, stop the fall, and let him jump again
                    Vector2 position = player.Position;
                    position.Y = blockBounds.Top - playerBounds.Height;
                    Vector2 velocity = player.Velocity;
                    velocity.Y = 0f;
                    player.ApplyMotion(position, velocity, isGrounded: true);
                    return;
                }
            }
        }
    }
}
