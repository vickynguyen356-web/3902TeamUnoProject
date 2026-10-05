using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Entities;
namespace TeamUno.Mario.Projectiles
{
    internal class ProjectileSpriteFactory
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 2f);
        }
        internal static IReadOnlyDictionary<ProjectileType, SpriteAnimation> CreateProjectileAnimations()
        {
            SpriteAnimation fireball = new SpriteAnimation(0.28f, CreateFrame(1, 8, 1, 8));

            SpriteAnimation hammer = new SpriteAnimation(0.28f, CreateFrame(6, 20, 6, 20));

            return new Dictionary<ProjectileType, SpriteAnimation>
            {
                { ProjectileType.Fireball, fireball },
                { ProjectileType.Hammer, hammer }
            };
        }
        private static SpriteFrame CreateFrame(int leftCol, int leftRow, int rightCol, int rightRow)
        {
            Rectangle leftFrame = new Rectangle(
                leftCol * FrameWidth,
                leftRow * FrameHeight,
                FrameWidth,
                FrameHeight);
            Rectangle rightFrame = new Rectangle(
                rightCol * FrameWidth,
                rightRow * FrameHeight,
                FrameWidth,
                FrameHeight);
            return new SpriteFrame(leftFrame, rightFrame);
        }
    }
}
