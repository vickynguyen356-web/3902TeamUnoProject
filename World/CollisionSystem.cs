using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Collision.Handlers;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Collision;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// Handles collision detection and resolution for the game world.
    /// </summary>
    internal class CollisionSystem
    {
        private readonly ICollisionHandler<MarioPlayer, Block> _marioBlockHandler;
        private readonly ICollisionHandler<MarioPlayer, IEnemy> _marioEnemyHandler;
        private readonly ICollisionHandler<MarioPlayer, IItem> _marioItemHandler;
        private readonly ICollisionHandler<MarioPlayer, IProjectile> _marioProjectileHandler;
        private readonly ICollisionHandler<IEnemy, Block> _enemyBlockHandler;
        private readonly ICollisionHandler<IItem, Block> _itemBlockHandler;
        private readonly ICollisionHandler<IEnemy, IProjectile> _enemyProjectileHandler;
        private readonly ICollisionHandler<IProjectile, Block> _projectileBlockHandler;
        private readonly ICollisionHandler<IEnemy, IEnemy> _enemyEnemyHandler;
        private readonly ICollisionHandler<IProjectile, IProjectile> _projectileProjectileHandler;

        public CollisionSystem(
            ICollisionHandler<MarioPlayer, Block> marioBlockHandler,
            ICollisionHandler<MarioPlayer, IEnemy> marioEnemyHandler,
            ICollisionHandler<MarioPlayer, IItem> marioItemHandler,
            ICollisionHandler<MarioPlayer, IProjectile> marioProjectileHandler,
            ICollisionHandler<IEnemy, Block> enemyBlockHandler,
            ICollisionHandler<IItem, Block> itemBlockHandler,
            ICollisionHandler<IEnemy, IProjectile> enemyProjectileHandler,
            ICollisionHandler<IProjectile, Block> projectileBlockHandler,
            ICollisionHandler<IEnemy, IEnemy> enemyEnemyHandler,
            ICollisionHandler<IProjectile, IProjectile> projectileProjectileHandler)
        {
            ArgumentNullException.ThrowIfNull(marioBlockHandler);
            ArgumentNullException.ThrowIfNull(marioEnemyHandler);
            ArgumentNullException.ThrowIfNull(marioItemHandler);
            ArgumentNullException.ThrowIfNull(marioProjectileHandler);
            ArgumentNullException.ThrowIfNull(enemyBlockHandler);
            ArgumentNullException.ThrowIfNull(itemBlockHandler);
            ArgumentNullException.ThrowIfNull(enemyProjectileHandler);
            ArgumentNullException.ThrowIfNull(projectileBlockHandler);
            ArgumentNullException.ThrowIfNull(enemyEnemyHandler);
            ArgumentNullException.ThrowIfNull(projectileProjectileHandler);

            _marioBlockHandler = marioBlockHandler;
            _marioEnemyHandler = marioEnemyHandler;
            _marioItemHandler = marioItemHandler;
            _marioProjectileHandler = marioProjectileHandler;
            _enemyBlockHandler = enemyBlockHandler;
            _itemBlockHandler = itemBlockHandler;
            _enemyProjectileHandler = enemyProjectileHandler;
            _projectileBlockHandler = projectileBlockHandler;
            _enemyEnemyHandler = enemyEnemyHandler;
            _projectileProjectileHandler = projectileProjectileHandler;
        }

        // Default, add or swap handlers here.
        public static CollisionSystem CreateDefault(GameProgress progress)
        {
            return new CollisionSystem(
                new MarioBlockCollisionHandler(),
                new MarioEnemyCollisionHandler(),
                new MarioItemCollisionHandler(progress),
                new MarioProjectileCollisionHandler(),
                new EnemyBlockCollisionHandler(),
                new ItemBlockCollisionHandler(),
                new EnemyProjectileCollisionHandler(),
                new ProjectileBlockCollisionHandler(),
                new EnemyEnemyCollisionHandler(),
                new ProjectileProjectileCollisionHandler());
        }

        /* Lists which entity types can collide. Each pairing calls Process to resolve the collision. 
         Only the player needs to know where it was last frame, thus the other collision pairings have Rectangle.Empty passed.*/

        /// <summary>
        /// Runs every collision check for this frame.
        /// previousPlayerBounds must get received before player moves this frame.
        /// isJumpHeld isn't used yet (TODO: jump/bounce handling).
        /// </summary>
        public void Update(GameSession session, Rectangle previousPlayerBounds, bool isJumpHeld = false)
        {
            ArgumentNullException.ThrowIfNull(session);

            Level level = session.Level;
            MarioPlayer[] players = new MarioPlayer[] { session.Player };

            ProcessDistinctPairs(level.Enemies, _enemyEnemyHandler);
            Process(level.Enemies, level.Blocks, Rectangle.Empty, _enemyBlockHandler);
            Process(level.Items, level.Blocks, Rectangle.Empty, _itemBlockHandler);
            Process(level.Projectiles, level.Blocks, Rectangle.Empty, _projectileBlockHandler);
            ProcessDistinctPairs(level.Projectiles, _projectileProjectileHandler);

            Process(players, level.Blocks, previousPlayerBounds, _marioBlockHandler);
            Process(level.Enemies, level.Projectiles, Rectangle.Empty, _enemyProjectileHandler);
            Process(players, level.Enemies, previousPlayerBounds, _marioEnemyHandler);
            Process(players, level.Items, previousPlayerBounds, _marioItemHandler);
            Process(players, level.Projectiles, previousPlayerBounds, _marioProjectileHandler);
        }

        /// <summary>
        /// Detects overlapping pair, then calls handler for each detection.
        /// Pairs are collected first, then handlers can add projectiles to level without
        /// breaking loops.
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="firsts"></param>
        /// <param name="seconds"></param>
        /// <param name="firstPreviousBounds"></param>
        /// <param name="handler"></param>
        private static void Process<TFirst, TSecond>(
            IEnumerable<TFirst> firsts,
            IEnumerable<TSecond> seconds,
            Rectangle firstPreviousBounds,
            ICollisionHandler<TFirst, TSecond> handler)
            where TFirst : ICollidable
            where TSecond : ICollidable
        {
            List<(TFirst First, TSecond Second)> pairs =
                new List<(TFirst, TSecond)>();

            foreach (TFirst first in firsts)
            {
                foreach (TSecond second in seconds)
                {
                    CollisionInfo info;
                    if (TryDetect(first.Bounds, second.Bounds, firstPreviousBounds, out info))
                    {
                        pairs.Add((first, second));
                    }
                }
            }

            foreach ((TFirst First, TSecond Second) pair in pairs)
            {
                CollisionInfo currentInfo;
                if (TryDetect(pair.First.Bounds, pair.Second.Bounds, firstPreviousBounds, out currentInfo))
                {
                    handler.Handle(pair.First, pair.Second, currentInfo);
                }
            }
        }

        private static void ProcessDistinctPairs<T>(
            IReadOnlyList<T> objects, ICollisionHandler<T, T> handler)
            where T : ICollidable
        {
            for (int firstIndex = 0; firstIndex < objects.Count; firstIndex = firstIndex + 1)
            {
                for (int secondIndex = firstIndex + 1; secondIndex < objects.Count; secondIndex = secondIndex + 1)
                {
                    T first = objects[firstIndex];
                    T second = objects[secondIndex];
                    CollisionInfo info;
                    if (TryDetect(first.Bounds, second.Bounds, Rectangle.Empty, out info))
                    {
                        handler.Handle(first, second, info);
                    }
                }
            }
        }

        /// <summary>
        /// Axis-aligned rectangle overlap testing. If the rectangles overlap, 
        /// which side of the 'first' object was touched is also taken into account.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="firstPreviousBounds"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static bool TryDetect(
            Rectangle first,
            Rectangle second,
            Rectangle firstPreviousBounds,
            out CollisionInfo info)
        {
            int left = Math.Max(first.Left, second.Left);
            int right = Math.Min(first.Right, second.Right);
            int top = Math.Max(first.Top, second.Top);
            int bottom = Math.Min(first.Bottom, second.Bottom);

            int overlapWidth = right - left;
            int overlapHeight = bottom - top;

            // Negative means there is a gap. Zero on one axis means the edges are touching,
            // which counts (standing on a block must keep registering). Corner-only contact does not.
            if (overlapWidth < 0 || overlapHeight < 0 || (overlapWidth == 0 && overlapHeight == 0))
            {
                info = default(CollisionInfo);
                return false;
            }

            CollisionSide side;
            if (overlapWidth < overlapHeight)
            {
                // Shallower overlap is the axis object came in on: left/right contact.
                side = first.Center.X < second.Center.X ? CollisionSide.Right : CollisionSide.Left;
            }
            else
            {
                // Screen Y grows downward, so smaller Y means 'first' is above 'second'.
                side = first.Center.Y < second.Center.Y ? CollisionSide.Bottom : CollisionSide.Top;
            }

            info = new CollisionInfo(side, overlapWidth, overlapHeight, firstPreviousBounds);
            return true;
        }
    }
}
