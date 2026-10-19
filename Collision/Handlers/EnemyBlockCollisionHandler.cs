using Microsoft.Xna.Framework;
using TeamUno.Mario.Collision;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>IEnemy vs Block. 
    /// CHECKLIST:
    /// info.Side is relative to the IEnemy. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up).
    /// TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class EnemyBlockCollisionHandler : ICollisionHandler<IEnemy, Block>
    {
        public void Handle(IEnemy first, Block second, CollisionInfo info)
        {
            // TODO: Enemy update (turn around on side hit, deleted upon falling thru floor).
            // TODO: Block update.
            if (first.IsDead
                || second.IsDeleted
                || second.Type == BlockType.Empty
                || second.Type == BlockType.Vine)
            {
                return;
            }

            if (info.Side == CollisionSide.Top && second.Type == BlockType.Platform)
            {
                return;
            }

            Rectangle blockBounds = second.Bounds;
            Rectangle enemyBounds = first.Bounds;
            Vector2 position = first.Position;
            Vector2 velocity = first.Velocity;

            switch (info.Side)
            {
                case CollisionSide.Right:
                    position.X = blockBounds.Left - enemyBounds.Width;
                    velocity.X = -velocity.X;
                    break;
                case CollisionSide.Left:
                    position.X = blockBounds.Right;
                    velocity.X = -velocity.X;
                    break;
                case CollisionSide.Bottom:
                    position.Y = blockBounds.Top - enemyBounds.Height;
                    break;
                case CollisionSide.Top:
                    position.Y = blockBounds.Bottom;
                    break;
                default:
                    return;
            }

            first.Position = position;
            first.Velocity = velocity;

            if (info.Side == CollisionSide.Top)
            {
                second.HitFromBelow(canBreak: false);
            }
        }
    }
}