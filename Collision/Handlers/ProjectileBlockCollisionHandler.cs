using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>IProjectile vs Block. 
    /// CHECKLIST:
    /// info.Side is relative to the IProjectile.
    /// Specifically for Mario projectile's, including Shell. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class ProjectileBlockCollisionHandler : ICollisionHandler<IProjectile, Block>
    {
        public void Handle(IProjectile first, Block second, CollisionInfo info)
        {
            if (first.IsDead
                || second.IsDeleted
                || second.Type == BlockType.Empty
                || second.Type == BlockType.Invisible
                || second.Type == BlockType.Vine
                || second.Type == BlockType.FlagPole)
            {
                return;
            }

            if (second.Type == BlockType.Platform && info.Side != CollisionSide.Bottom)
            {
                return;
            }

            Projectile projectile = first as Projectile;
            if (projectile == null)
            {
                return;
            }

            Rectangle projectileBounds = projectile.Bounds;
            Rectangle blockBounds = second.Bounds;
            Vector2 position = projectile.Position;
            Vector2 velocity = projectile.CurrentVelocity;

            if (projectile is Fireball fireball)
            {
                ResolveFireballCollision(fireball, second, info.Side, projectileBounds, blockBounds, position, velocity);
            }
            else if (projectile is KoopaShell shell)
            {
                ResolveShellCollision(shell, second, info.Side, projectileBounds, blockBounds, position, velocity);
            }
            else
            {
                projectile.Kill();
            }
        }

        private static void ResolveFireballCollision(
            Fireball fireball,
            Block block,
            CollisionSide side,
            Rectangle projectileBounds,
            Rectangle blockBounds,
            Vector2 position,
            Vector2 velocity)
        {
            switch (side)
            {
                case CollisionSide.Bottom:
                    if (velocity.Y <= 0f
                        || projectileBounds.Right <= blockBounds.Left
                        || projectileBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }

                    position.Y = blockBounds.Top - projectileBounds.Height;
                    fireball.Bounce();
                    fireball.ApplyMotion(position, fireball.CurrentVelocity);
                    break;
                case CollisionSide.Top:
                    if (velocity.Y >= 0f
                        || block.Type == BlockType.Platform
                        || projectileBounds.Right <= blockBounds.Left
                        || projectileBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }

                    fireball.Kill();
                    break;
                case CollisionSide.Left:
                    if (velocity.X >= 0f
                        || projectileBounds.Bottom <= blockBounds.Top
                        || projectileBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }
                    fireball.Kill();
                    break;
                case CollisionSide.Right:
                    if (velocity.X <= 0f
                        || projectileBounds.Bottom <= blockBounds.Top
                        || projectileBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }
                    fireball.Kill();
                    break;
            }
        }

        private static void ResolveShellCollision(
            KoopaShell shell,
            Block block,
            CollisionSide side,
            Rectangle projectileBounds,
            Rectangle blockBounds,
            Vector2 position,
            Vector2 velocity)
        {
            switch (side)
            {
                case CollisionSide.Bottom:
                    if (velocity.Y < 0f
                        || projectileBounds.Right <= blockBounds.Left
                        || projectileBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }

                    position.Y = blockBounds.Top - projectileBounds.Height;
                    velocity.Y = 0f;
                    shell.ApplyMotion(position, velocity);
                    return;
                case CollisionSide.Top:
                    if (velocity.Y >= 0f
                        || projectileBounds.Right <= blockBounds.Left
                        || projectileBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }

                    position.Y = blockBounds.Bottom;
                    velocity.Y = 0f;
                    shell.ApplyMotion(position, velocity);
                    return;
                case CollisionSide.Right:
                    if (velocity.X <= 0f
                        || projectileBounds.Bottom <= blockBounds.Top
                        || projectileBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }

                    position.X = blockBounds.Left - projectileBounds.Width;
                    break;
                case CollisionSide.Left:
                    if (velocity.X >= 0f
                        || projectileBounds.Bottom <= blockBounds.Top
                        || projectileBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }

                    position.X = blockBounds.Right;
                    break;
                default:
                    return;
            }

            if (block.Type == BlockType.Brick || block.Type == BlockType.Question)
            {
                block.HitFromBelow(canBreak: true);
            }

            shell.Bounce();
            shell.ApplyMotion(position, shell.CurrentVelocity);
        }
    }
}
