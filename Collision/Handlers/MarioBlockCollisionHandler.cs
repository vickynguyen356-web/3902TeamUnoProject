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
            if (player.IsDead || player.Velocity.Y < 0f)
            {
                return;
            }

            // TODO: other block types (brick, question, pipe, flagpole, vine...)
            // and head/side hits go here. For now only the floor is handled.
            if (block.Type != BlockType.Ground)
            {
                return;
            }

            Rectangle playerBounds = player.Bounds;
            Rectangle blockBounds = block.Bounds;
            Rectangle previousPlayerBounds = info.FirstPreviousBounds;

            // His feet were above the tile and have now reached its top.
            bool landed = previousPlayerBounds.Bottom <= blockBounds.Top
                && playerBounds.Bottom >= blockBounds.Top;

            if (!landed)
            {
                return;
            }

            // Put his feet on top, stop the fall, and let him jump again
            Vector2 position = player.Position;
            position.Y = blockBounds.Top - playerBounds.Height;
            Vector2 velocity = player.Velocity;
            velocity.Y = 0f;
            player.ApplyMotion(position, velocity, isGrounded: true);
        }
    }
}
