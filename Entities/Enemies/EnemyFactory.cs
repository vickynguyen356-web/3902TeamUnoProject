using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal static class EnemyFactory
    {
        private static Texture2D _enemyTexture;

        public static void Initialize(Texture2D enemyTexture)
        {
            ArgumentNullException.ThrowIfNull(enemyTexture);

            _enemyTexture = enemyTexture;
        }

        public static IEnemy Create(EnemyType type, Vector2 position)
        {
            ISprite sprite = new SpriteSheetSprite(_enemyTexture, 2f, true);

            switch (type)
            {
                case EnemyType.Goomba:
                    return new Goomba(sprite, position);
                case EnemyType.Koopa:
                    return new Koopa(sprite, position);
                case EnemyType.PiranhaPlant:
                    return new PiranhaPlant(sprite, position);
                case EnemyType.HammerBro:
                    return new HammerBro(sprite, position);
                case EnemyType.Bowser:
                    return new Bowser(sprite, position);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
