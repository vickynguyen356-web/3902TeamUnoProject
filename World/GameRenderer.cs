using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class GameRenderer
    {
        private readonly Texture2D _backgroundTexture;
        private readonly Texture2D _blockTexture;

        public GameRenderer(Texture2D backgroundTexture, Texture2D blockTexture)
        {
            _backgroundTexture = backgroundTexture;
            _blockTexture = blockTexture;
        }

        public void DrawBackground(SpriteBatch spriteBatch, Rectangle viewport)
        {
            spriteBatch.Draw(_backgroundTexture, viewport, Color.White * 0.45f);
        }

        public void DrawWorld(SpriteBatch spriteBatch, Level level)
        {
            foreach (Block block in level.Blocks)
            {
                block.Draw(spriteBatch, _blockTexture);
            }

            foreach (IEnemy enemy in level.Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            level.Player.Draw(spriteBatch);

            foreach (IItem item in level.Items)
            {
                item.Draw(spriteBatch);
            }

            foreach (IProjectile projectile in level.Projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
