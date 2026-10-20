using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    internal class ItemBlockCollisionHandler : ICollisionHandler<IItem, Block>
    {
        public void Handle(IItem item, Block block, CollisionInfo info)
        {
            if (item.IsExpired || block.IsDeleted || (item.Type != ItemType.Mushroom
                && item.Type != ItemType.OneUpMushroom && item.Type != ItemType.Star))
            {
                return;
            }

            // items can move through these blocks
            if (block.Type == BlockType.Empty || block.Type == BlockType.Vine
                || block.Type == BlockType.FlagPole)
            {
                return;
            }

            Rectangle itemBounds = item.Bounds;
            Rectangle blockBounds = block.Bounds;
            // another collision might have already moved the item away
            if (itemBounds.Right < blockBounds.Left || itemBounds.Left > blockBounds.Right
                || itemBounds.Bottom < blockBounds.Top || itemBounds.Top > blockBounds.Bottom)
            {
                return;
            }

            Vector2 position = item.Position;
            Vector2 velocity = item.CurrentVelocity;
            switch (info.Side)
            {
                case CollisionSide.Bottom:
                    if (velocity.Y < 0f || itemBounds.Right <= blockBounds.Left
                        || itemBounds.Left >= blockBounds.Right)
                    {
                        return;
                    }
                    position.Y = blockBounds.Top - itemBounds.Height;
                    // stars bounce while mushrooms stay on the surface
                    velocity.Y = item.Type == ItemType.Star ? -Star.BounceSpeed : 0f;
                    break;
                case CollisionSide.Top:
                    if (velocity.Y >= 0f || block.Type == BlockType.Platform)
                    {
                        return;
                    }
                    position.Y = blockBounds.Bottom;
                    velocity.Y = 0f;
                    break;
                case CollisionSide.Right:
                    if (velocity.X <= 0f || block.Type == BlockType.Platform
                        || itemBounds.Bottom <= blockBounds.Top || itemBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }
                    position.X = blockBounds.Left - itemBounds.Width;
                    velocity.X = -velocity.X;
                    break;
                case CollisionSide.Left:
                    if (velocity.X >= 0f || block.Type == BlockType.Platform
                        || itemBounds.Bottom <= blockBounds.Top || itemBounds.Top >= blockBounds.Bottom)
                    {
                        return;
                    }
                    position.X = blockBounds.Right;
                    velocity.X = -velocity.X;
                    break;
                default:
                    return;
            }

            item.ApplyMotion(position, velocity);
        }
    }
}
