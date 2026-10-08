using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Enemies;

namespace TeamUno.Mario.World
{
    internal class EnemySpawnDefinition
    {
        private readonly EnemyType _type;
        private readonly Vector2 _position;

        public EnemyType Type
        {
            get
            {
                return _type;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public EnemySpawnDefinition(EnemyType type, Vector2 position)
        {
            _type = type;
            _position = position;
        }
    }
}
