using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Projectiles
{
    public class ProjectileSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }

        internal static IReadOnlyDictionary<ProjectileType, SpriteAnimation> CreateProjectileAnimations()
        {
            SpriteAnimation fireball = new SpriteAnimation(0.28f, CreateFrame(89, 559, 14, 16));

            SpriteAnimation hammer = new SpriteAnimation(0.28f, CreateFrame(412, 1327, 8, 16));

            return new Dictionary<ProjectileType, SpriteAnimation>
            {
                { ProjectileType.Fireball, fireball },
                { ProjectileType.Hammer, hammer }
            };
        }

        private static SpriteFrame CreateFrame(int x, int y, int width, int height)
        {
            Rectangle sourceRectangle = new Rectangle(x, y, width, height);
            return new SpriteFrame(sourceRectangle, sourceRectangle);
        }
    }
}
