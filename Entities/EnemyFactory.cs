using System;
using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly Func<ISprite> _createGoombaSprite;
        private readonly Func<ISprite> _createKoopaSprite;
        private readonly Func<ISprite> _createPiranhaSprite;

        public EnemyFactory(Func<ISprite> createGoombaSprite,
            Func<ISprite> createKoopaSprite,
            Func<ISprite> createPiranhaSprite)
        {
            _createGoombaSprite = createGoombaSprite;
            _createKoopaSprite = createKoopaSprite;
            _createPiranhaSprite = createPiranhaSprite;
        }

        public Enemy Create(EnemyType type, Vector2 position)
        {
            switch (type)
            {
                case EnemyType.Goomba:
                    return new Goomba(_createGoombaSprite(), position);
                case EnemyType.Koopa:
                    return new Koopa(_createKoopaSprite(), position);
                case EnemyType.PiranhaPlant:
                    return new PiranhaPlant(_createPiranhaSprite(), position);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
