using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Entities;

namespace Sprint0.World
{
    public class GameRenderer
    {
        private readonly Texture2D _whitePixelTexture;
        private readonly Texture2D _blockTexture;
        private readonly SpriteFont _controlsFont;

        public GameRenderer(Texture2D whitePixelTexture, Texture2D blockTexture, SpriteFont controlsFont)
        {
            _whitePixelTexture = whitePixelTexture;
            _blockTexture = blockTexture;
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
            foreach (Block block in level.Blocks)
            {
                block.Draw(spriteBatch, _blockTexture);
            }

            foreach (Coin coin in level.Coins)
            {
                coin.Draw(spriteBatch);
            }

            foreach (Enemy enemy in level.Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        public void DrawControls(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_controlsFont, "A/D or Left/Right: move   W/Up/Space: jump", new Vector2(32, 24), Color.White);
            spriteBatch.DrawString(_controlsFont, "S/Down: crouch   Z/N: throw fireball   E: damage", new Vector2(32, 56), Color.White);
            spriteBatch.DrawString(_controlsFont, "R: reset   Q/Escape: quit", new Vector2(32, 88), Color.White);
            spriteBatch.DrawString(_controlsFont, "O/P: cycle to previous and next enemy", new Vector2(32, 120), Color.White);
        }
    }
}
