using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Entities;

namespace Sprint0.World
{
    public class GameRenderer
    {
        private readonly Texture2D _whitePixelTexture;
        private readonly SpriteFont _controlsFont;

        public GameRenderer(Texture2D whitePixelTexture, SpriteFont controlsFont)
        {
            _whitePixelTexture = whitePixelTexture;
            _controlsFont = controlsFont;
        }

        public void DrawDemoFloor(SpriteBatch spriteBatch, Rectangle viewport, int floorY)
        {
            Rectangle floorBounds = new Rectangle(viewport.Left, floorY, viewport.Width, viewport.Bottom - floorY);
            spriteBatch.Draw(_whitePixelTexture, floorBounds, Color.SaddleBrown);
        }

        public void DrawWorld(SpriteBatch spriteBatch, Level level, MarioPlayer player)
        {
            DrawLevel(spriteBatch, level);
            player.Draw(spriteBatch);
        }

        private void DrawLevel(SpriteBatch spriteBatch, Level level)
        {
            // Block drawing goes here.

            foreach (Coin coin in level.Coins)
            {
                coin.Draw(spriteBatch);
            }

            foreach (Goomba enemy in level.Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        public void DrawControls(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_controlsFont, "A/D or Left/Right: move   W/Up/Space: jump", new Vector2(32, 24), Color.White);
            spriteBatch.DrawString(_controlsFont, "S/Down: crouch   Z/N: throw fireball", new Vector2(32, 56), Color.White);
            spriteBatch.DrawString(_controlsFont, "R: reset   Q/Escape: quit", new Vector2(32, 88), Color.White);
        }
    }
}
