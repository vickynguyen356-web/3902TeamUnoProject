using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal static class ProjectileFactory
    {
        private static Texture2D _enemyTexture;
        private static Texture2D _itemTexture;

        public static void Initialize(Texture2D enemyTexture, Texture2D itemTexture)
        {
            ArgumentNullException.ThrowIfNull(enemyTexture);
            ArgumentNullException.ThrowIfNull(itemTexture);

            _enemyTexture = enemyTexture;
            _itemTexture = itemTexture;
        }

        public static IProjectile Create(ProjectileType type, Vector2 position, Vector2 velocity, bool isEnemyProjectile)
        {
            ISprite sprite = ProjectileSpriteFactory.Create(type, _enemyTexture, _itemTexture);

            switch (type)
            {
                case ProjectileType.Fireball:
                    return new Fireball(sprite, position, velocity, isEnemyProjectile);
                case ProjectileType.Hammer:
                    return new Hammer(sprite, position, velocity, isEnemyProjectile);
                case ProjectileType.BowserFire:
                    return new BowserFire(sprite, position, velocity, isEnemyProjectile);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
