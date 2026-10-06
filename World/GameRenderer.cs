using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    public class GameRenderer
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

        public void DrawWorld(SpriteBatch spriteBatch, GameSession session)
        {
            foreach (Block block in session.Level.Blocks)
            {
                block.Draw(spriteBatch, _blockTexture);
            }

            foreach (IEnemy enemy in session.Level.Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            session.Player.Draw(spriteBatch);

            foreach (IItem item in session.Level.Items)
            {
                item.Draw(spriteBatch);
            }

            foreach (IProjectile projectile in session.Level.Projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
