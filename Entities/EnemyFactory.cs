using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly Func<ISprite> _createGoombaSprite;
        private readonly Func<ISprite> _createKoopaSprite;
        private readonly Func<ISprite> _createPiranhaSprite;
        private readonly Func<ISprite> _createHammerBroSprite;
        private readonly Func<ISprite> _createBowserSprite;
        private readonly IProjectileFactory _projectileFactory;

        public EnemyFactory(Func<ISprite> createGoombaSprite,
            Func<ISprite> createKoopaSprite,
            Func<ISprite> createPiranhaSprite,
            Func<ISprite> createHammerBroSprite,
            Func<ISprite> createBowserSprite,
            IProjectileFactory projectileFactory)
        {
            _createGoombaSprite = createGoombaSprite;
            _createKoopaSprite = createKoopaSprite;
            _createPiranhaSprite = createPiranhaSprite;
            _createHammerBroSprite = createHammerBroSprite;
            _createBowserSprite = createBowserSprite;

            _projectileFactory = projectileFactory;
        }

        public IEnemy Create(EnemyType type, Vector2 position)
        {
            switch (type)
            {
                case EnemyType.Goomba:
                    return new Goomba(_createGoombaSprite(), position, _projectileFactory);
                case EnemyType.Koopa:
                    return new Koopa(_createKoopaSprite(), position, _projectileFactory);
                case EnemyType.PiranhaPlant:
                    return new PiranhaPlant(_createPiranhaSprite(), position, _projectileFactory);
                case EnemyType.HammerBro:
                    return new HammerBro(_createHammerBroSprite(), position, _projectileFactory);
                case EnemyType.Bowser:
                    return new Bowser(_createBowserSprite(), position, _projectileFactory);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
