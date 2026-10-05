using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Interfaces
{
    internal interface IEnemyFactory
    {
        IEnemy Create(EnemyType type, Vector2 position);
    }
}
