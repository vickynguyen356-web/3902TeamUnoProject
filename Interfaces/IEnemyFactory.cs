using Microsoft.Xna.Framework;
using Sprint0.Entities;

namespace Sprint0.Interfaces
{
    public interface IEnemyFactory
    {
        Enemy Create(EnemyType type, Vector2 position);
    }
}
