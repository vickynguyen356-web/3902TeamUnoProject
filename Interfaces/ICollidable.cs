using Microsoft.Xna.Framework;

namespace TeamUno.Mario.Interfaces
{
    /// <summary>
    /// Anything CollisionSystem can test. Make IPlayer, IEnemy, IItem, IProjectile, and Block
    /// implement/inherit this. If "Rectangle Bounds" is already exposed, nothing else changes.
    /// </summary>
    internal interface ICollidable
    {
        Rectangle Bounds { get; }
    }
}
