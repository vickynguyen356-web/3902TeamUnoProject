using System;
using Microsoft.Xna.Framework;
using Sprint0.Entities;
using Sprint0.Interfaces;

namespace Sprint0.World
{
    public class CollisionSystem : ICollisionSystem
    {
        private readonly Level _level;

        public CollisionSystem(Level level)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            _level = level;
        }

        public void Resolve(MarioPlayer player, Vector2 previousPosition)
        {
            // Check collisions against objects in _level here.
            // Use player.ApplyMotion to apply the result.
            Vector2 position = player.Position;
            Vector2 velocity = player.Velocity;
            Rectangle bounds = player.Bounds;
            Rectangle previousBounds = new Rectangle(
                (int)previousPosition.X,
                (int)previousPosition.Y,
                bounds.Width,
                bounds.Height);
            bool isGrounded = player.IsGrounded;

            foreach (Block block in _level.Blocks)
            {
                if (block.Type != BlockType.Question && block.Type != BlockType.Solid)
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                Rectangle currentBounds = new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    bounds.Width,
                    bounds.Height);
                bool overlapsHorizontally = currentBounds.Right > blockBounds.Left
                    && currentBounds.Left < blockBounds.Right;

                if (overlapsHorizontally
                    && velocity.Y < 0
                    && previousBounds.Top >= blockBounds.Bottom
                    && currentBounds.Top < blockBounds.Bottom)
                {
                    if (block.Type == BlockType.Question)
                    {
                        block.HitFromBelow();
                    }

                    position.Y = blockBounds.Bottom;
                    velocity.Y = 0;
                    isGrounded = false;
                    continue;
                }

                if (overlapsHorizontally
                    && velocity.Y > 0
                    && previousBounds.Bottom <= blockBounds.Top
                    && currentBounds.Bottom > blockBounds.Top)
                {
                    position.Y = blockBounds.Top - bounds.Height;
                    velocity.Y = 0;
                    isGrounded = true;
                    continue;
                }

                currentBounds = new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    bounds.Width,
                    bounds.Height);
                bool overlapsVertically = currentBounds.Bottom > blockBounds.Top
                    && currentBounds.Top < blockBounds.Bottom;

                if (overlapsVertically
                    && velocity.X > 0
                    && previousBounds.Right <= blockBounds.Left
                    && currentBounds.Right > blockBounds.Left)
                {
                    position.X = blockBounds.Left - bounds.Width;
                    velocity.X = 0;
                }
                else if (overlapsVertically
                    && velocity.X < 0
                    && previousBounds.Left >= blockBounds.Right
                    && currentBounds.Left < blockBounds.Right)
                {
                    position.X = blockBounds.Right;
                    velocity.X = 0;
                }
            }

            player.ApplyMotion(position, velocity, isGrounded);
        }
    }
}
