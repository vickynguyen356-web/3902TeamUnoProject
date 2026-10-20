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

        public CollisionSystem(
            ICollisionHandler<MarioPlayer, Block> marioBlockHandler,
            ICollisionHandler<MarioPlayer, IEnemy> marioEnemyHandler,
            ICollisionHandler<MarioPlayer, IItem> marioItemHandler,
            ICollisionHandler<MarioPlayer, IProjectile> marioProjectileHandler,
            ICollisionHandler<IEnemy, Block> enemyBlockHandler,
            ICollisionHandler<IItem, Block> itemBlockHandler,
            ICollisionHandler<IEnemy, IProjectile> enemyProjectileHandler,
            ICollisionHandler<IProjectile, Block> projectileBlockHandler)
        {
            ArgumentNullException.ThrowIfNull(marioBlockHandler);
            ArgumentNullException.ThrowIfNull(marioEnemyHandler);
            ArgumentNullException.ThrowIfNull(marioItemHandler);
            ArgumentNullException.ThrowIfNull(marioProjectileHandler);
            ArgumentNullException.ThrowIfNull(enemyBlockHandler);
            ArgumentNullException.ThrowIfNull(itemBlockHandler);
            ArgumentNullException.ThrowIfNull(enemyProjectileHandler);
            ArgumentNullException.ThrowIfNull(projectileBlockHandler);

            _marioBlockHandler = marioBlockHandler;
            _marioEnemyHandler = marioEnemyHandler;
            _marioItemHandler = marioItemHandler;
            _marioProjectileHandler = marioProjectileHandler;
            _enemyBlockHandler = enemyBlockHandler;
            _itemBlockHandler = itemBlockHandler;
            _enemyProjectileHandler = enemyProjectileHandler;
            _projectileBlockHandler = projectileBlockHandler;
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
                new ProjectileBlockCollisionHandler());
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

            // Collisions involving Mario (previous bounds are only tracked for him)
            Process(players, level.Blocks, previousPlayerBounds, _marioBlockHandler);
            Process(players, level.Enemies, previousPlayerBounds, _marioEnemyHandler);
            Process(players, level.Items, previousPlayerBounds, _marioItemHandler);
            Process(players, level.Projectiles, previousPlayerBounds, _marioProjectileHandler);

            // Everything else
            Process(level.Enemies, level.Blocks, Rectangle.Empty, _enemyBlockHandler);
            Process(level.Items, level.Blocks, Rectangle.Empty, _itemBlockHandler);
            Process(level.Enemies, level.Projectiles, Rectangle.Empty, _enemyProjectileHandler);
            Process(level.Projectiles, level.Blocks, Rectangle.Empty, _projectileBlockHandler);
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
            List<(TFirst First, TSecond Second, CollisionInfo Info)> pairs =
                new List<(TFirst, TSecond, CollisionInfo)>();

            foreach (TFirst first in firsts)
            {
                foreach (TSecond second in seconds)
                {
                    CollisionInfo info;
                    if (TryDetect(first.Bounds, second.Bounds, firstPreviousBounds, out info))
                    {
                        pairs.Add((first, second, info));
                    }
                }
            }

            foreach ((TFirst First, TSecond Second, CollisionInfo Info) pair in pairs)
            {
                handler.Handle(pair.First, pair.Second, pair.Info);
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
