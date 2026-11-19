using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Enemies;

namespace TeamUno.Mario.World
{
    internal class EnemySpawnDefinition
    {
        private readonly EnemyType _type;
        private readonly Vector2 _position;
        private readonly int _sourceTileId;
        private readonly bool _isUnderwater;

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

        public int SourceTileId
        {
            get
            {
                return _sourceTileId;
            }
        }

        public bool IsUnderwater
        {
            get
            {
                return _isUnderwater;
            }
        }

        public EnemySpawnDefinition(
            EnemyType type,
            Vector2 position,
            int sourceTileId = -1,
            bool isUnderwater = false)
        {
            _type = type;
            _position = position;
            _sourceTileId = sourceTileId;
            _isUnderwater = isUnderwater;
        }
    }
}
