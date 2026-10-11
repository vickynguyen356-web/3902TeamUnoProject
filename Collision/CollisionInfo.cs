using Microsoft.Xna.Framework;

namespace TeamUno.Mario.Collision
{
    /// <summary>
    /// Which side of the first object the contact happened on
    /// </summary>
    internal enum CollisionSide
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }
    /// <summary>
    /// Details about one collision. Side is relevant to the first argument of the handler.
    /// Ex: in PlayerEnemyCollisionHandler, Side == Bottom means player's feet hit
    /// the enemy, which is a stomp.
    /// </summary>
    internal readonly struct CollisionInfo
    {
        // Which side of the first object was touched
        public CollisionSide Side { get; }
        public int OverlapWidth { get; }
        public int OverlapHeight { get; }

        /// <summary>
        /// Where the first object was before this frame's movement.
        /// Only filled in for the player. Rectangle.Empty for everything else.
        /// </summary>
        public Rectangle FirstPreviousBounds { get; }

        public CollisionInfo(CollisionSide side, int overlapWidth, int overlapHeight, Rectangle firstPreviousBounds)
        {
            Side = side;
            OverlapWidth = overlapWidth;
            OverlapHeight = overlapHeight;
            FirstPreviousBounds = firstPreviousBounds;
        }
    }
}
