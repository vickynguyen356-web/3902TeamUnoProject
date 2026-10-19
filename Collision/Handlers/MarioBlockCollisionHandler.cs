using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>MarioPlayer vs Block
    /// CHECKLIST:
    /// info.Side is relative to the player. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class MarioBlockCollisionHandler : ICollisionHandler<MarioPlayer, Block>
    {
        public void Handle(MarioPlayer player, Block block, CollisionInfo info)
        {
            if (ShouldIgnoreCollision(player, block))
            {
                return;
            }

            // TODO: other block types (brick, question, pipe, flagpole, vine...)
            // and head/side hits go here. For now only the floor is handled.
            if (block.Type == BlockType.Vine || block.Type == BlockType.Empty)
            {
                return;
            }

            Rectangle blockBounds = block.Bounds;
            if (block.Type == BlockType.FlagPole)
            {
                TryStartFlagpoleSlide(player, blockBounds, info.Side);
                return;
            }

            if (ResolveLanding(player, blockBounds, info.FirstPreviousBounds))
            {
                return;
            }

            if (block.Type == BlockType.Platform)
            {
                return;
            }

            // Put his feet on top, stop the fall, and let him jump again
            if (ResolveHitFromBelow(player, block, blockBounds, info.FirstPreviousBounds))
            {
                return;
            }

            ResolveHorizontalCollision(player, blockBounds, info.FirstPreviousBounds);
        }

        private static bool ShouldIgnoreCollision(MarioPlayer player, Block block)
        {
            return player.IsDead
                || block.IsDeleted
                || player.IsChangingForm
                || (player.IsSlidingOnFlagpole && block.Type != BlockType.Ground);
        }

        private static void TryStartFlagpoleSlide(
            MarioPlayer player,
            Rectangle flagpoleBounds,
            CollisionSide collisionSide)
        {
            bool hitSide = collisionSide == CollisionSide.Left || collisionSide == CollisionSide.Right;
            if (hitSide && player.Velocity.X != 0)
            {
                player.StartFlagpoleSlide(
                    flagpoleBounds.Center.X,
                    flagpoleBounds.Top,
                    flagpoleBounds.Bottom);
            }
        }

        private static bool ResolveLanding(
            MarioPlayer player,
            Rectangle blockBounds,
            Rectangle previousPlayerBounds)
        {
            Rectangle playerBounds = player.Bounds;
            Vector2 velocity = player.Velocity;
            bool crossedBlockTop = previousPlayerBounds.Bottom <= blockBounds.Top
                && playerBounds.Bottom >= blockBounds.Top;

            if (velocity.Y < 0 || !crossedBlockTop)
            {
                return false;
            }

            Vector2 position = player.Position;
            position.Y = blockBounds.Top - playerBounds.Height;
            velocity.Y = 0;
            player.ApplyMotion(position, velocity, isGrounded: true);
            player.StopFlagpoleSlide();
            return true;
        }

        private static bool ResolveHitFromBelow(
            MarioPlayer player,
            Block block,
            Rectangle blockBounds,
            Rectangle previousPlayerBounds)
        {
            Rectangle playerBounds = player.Bounds;
            Vector2 velocity = player.Velocity;
            bool crossedBlockBottom = previousPlayerBounds.Top >= blockBounds.Bottom
                && playerBounds.Top <= blockBounds.Bottom;

            if (velocity.Y >= 0 || !crossedBlockBottom)
            {
                return false;
            }

            Vector2 position = player.Position;
            position.Y = blockBounds.Bottom;
            velocity.Y = 0;
            player.ApplyMotion(position, velocity, isGrounded: false);
            block.HitFromBelow(!player.IsSmall);
            return true;
        }

        private static void ResolveHorizontalCollision(
            MarioPlayer player,
            Rectangle blockBounds,
            Rectangle previousPlayerBounds)
        {
            Rectangle playerBounds = player.Bounds;
            Vector2 position = player.Position;
            Vector2 velocity = player.Velocity;

            if (velocity.X > 0
                && previousPlayerBounds.Right <= blockBounds.Left
                && playerBounds.Right >= blockBounds.Left)
            {
                position.X = blockBounds.Left - playerBounds.Width;
                velocity.X = 0;
            }
            else if (velocity.X < 0
                && previousPlayerBounds.Left >= blockBounds.Right
                && playerBounds.Left <= blockBounds.Right)
            {
                position.X = blockBounds.Right;
                velocity.X = 0;
            }
            else
            {
                return;
            }

            player.ApplyMotion(position, velocity, player.IsGrounded);
        }
    }
}
