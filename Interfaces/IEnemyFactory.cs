using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Interfaces
{
    public interface IEnemyFactory
    {
        IEnemy Create(EnemyType type, Vector2 position);
    }
}
