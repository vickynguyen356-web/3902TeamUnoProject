using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Projectiles
{
    internal static class ProjectileSpriteFactory
    {
        public static ISprite Create(ProjectileType type, Texture2D enemyTexture, Texture2D itemTexture)
        {
            switch (type)
            {
                case ProjectileType.Fireball:
                    return new SpriteSheetSprite(itemTexture, 2f);
                case ProjectileType.BowserFire:
                    return new SpriteSheetSprite(enemyTexture, 2f, useSpriteEffects: true);
                case ProjectileType.Hammer:
                    return new SpriteSheetSprite(enemyTexture, 2f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        internal static SpriteAnimation CreateFireballAnimation()
        {
            return new SpriteAnimation(0.1f,
                CreateFrame(180, 54, 8, 8),
                CreateFrame(190, 54, 8, 8),
                CreateFrame(200, 54, 8, 8),
                CreateFrame(210, 54, 8, 8));
        }

        internal static SpriteAnimation CreateBowserFireAnimation()
        {
            return new SpriteAnimation(0.14f,
                CreateFrame(276, 375, 24, 8),
                CreateFrame(340, 375, 24, 8));
        }

        internal static SpriteAnimation CreateHammerAnimation()
        {
            return new SpriteAnimation(0.28f, CreateFrame(412, 1327, 8, 16));
        }

        private static SpriteFrame CreateFrame(int x, int y, int width, int height)
        {
            Rectangle sourceRectangle = new Rectangle(x, y, width, height);
            return new SpriteFrame(sourceRectangle, sourceRectangle);
        }
    }
}
