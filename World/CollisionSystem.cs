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
        }
    }
}
