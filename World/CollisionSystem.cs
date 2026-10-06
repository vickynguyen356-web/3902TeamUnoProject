using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal class CollisionSystem
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Collision detection will maintain state between updates")]
        public void Update(GameSession session, Rectangle previousPlayerBounds)
        {
        }
    }
}
