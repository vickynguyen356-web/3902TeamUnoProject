using Microsoft.Xna.Framework;
using Sprint0.Entities;

namespace Sprint0.Interfaces
{
    public interface ICollisionSystem
    {
        void Resolve(MarioPlayer player, Vector2 previousPosition);
    }
}
