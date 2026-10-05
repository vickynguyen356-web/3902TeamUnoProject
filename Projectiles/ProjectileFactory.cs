using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Projectiles
{
    internal class ProjectileFactory : IProjectileFactory
    {
        private readonly Func<ISprite> _sprite;

        public ProjectileFactory(Func<ISprite> sprite)
        {
            ArgumentNullException.ThrowIfNull(sprite);

            _sprite = sprite;
        }

        public IProjectile Create(ProjectileType type, Vector2 position, Vector2 velocity)
        {
            switch (type)
            {
                case ProjectileType.Fireball:
                    return new Fireball(_sprite(), position, velocity);
                case ProjectileType.Hammer:
                    return new Hammer(_sprite(), position, velocity);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
