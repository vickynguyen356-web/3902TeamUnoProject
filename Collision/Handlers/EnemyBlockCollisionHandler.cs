using Microsoft.Xna.Framework;
using TeamUno.Mario.Collision;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
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
            if (first.IsDead || first is PiranhaPlant || first is BulletBill
                || first is BulletBillCannon || first is FireBarSegment
                || first is CheepCheep
                || second.IsDeleted
                || second.Type == BlockType.Empty
                || second.Type == BlockType.Invisible
                || second.Type == BlockType.Vine
                || second.Type == BlockType.FlagPole
                || second.Type == BlockType.Flag)
            {
                return;
            }

            if (info.Side != CollisionSide.Bottom && second.Type == BlockType.Platform)
            {
                return;
            }

            Rectangle blockBounds = second.Bounds;
            Rectangle enemyBounds = first.CollisionBounds;
            Rectangle visualBounds = first.Bounds;
            Vector2 position = first.Position;
            Vector2 velocity = first.Velocity;

            switch (info.Side)
            {
                case CollisionSide.Right:
                    if (velocity.X <= 0f || enemyBounds.Bottom <= blockBounds.Top || enemyBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }
                    position.X = blockBounds.Left - (enemyBounds.Right - visualBounds.Left);
                    velocity.X = -velocity.X;
                    break;
                case CollisionSide.Left:
                    if (velocity.X >= 0f || enemyBounds.Bottom <= blockBounds.Top || enemyBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }
                    position.X = blockBounds.Right - (enemyBounds.Left - visualBounds.Left);
                    velocity.X = -velocity.X;
                    break;
                case CollisionSide.Bottom:
                    if (velocity.Y < 0f || enemyBounds.Right <= blockBounds.Left || enemyBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }
                    position.Y = blockBounds.Top - (enemyBounds.Bottom - visualBounds.Top);
                    velocity.Y = 0f;
                    break;
                case CollisionSide.Top:
                    if (velocity.Y >= 0f || enemyBounds.Right <= blockBounds.Left || enemyBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }
                    position.Y = blockBounds.Bottom - (enemyBounds.Top - visualBounds.Top);
                    velocity.Y = 0f;
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
