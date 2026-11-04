using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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

        public static void DrawHud(SpriteBatch spriteBatch, GameSession session, MenuScreen menu, SpriteFont font, Rectangle viewport)
        {
            string progress = "Area: " + session.Level.Definition.Id
                + "     Coins: " + session.Progress.Coins
                + "     Lives: " + session.Progress.Lives;
            string controls = "Esc: menu   Q: quit   F1-F7: areas and tests";
            DrawText(spriteBatch, font, progress, new Vector2(20, 16));
            DrawText(spriteBatch, font, controls, new Vector2(20, 44));

            if (session.State == GameSessionState.Menu)
            {
                DrawMenu(spriteBatch, font, menu, viewport);
                return;
            }

            string message;
            switch (session.State)
            {
                case GameSessionState.Paused:
                    message = "Paused\nP to resume";
                    break;
                case GameSessionState.Dying:
                    message = "You died";
                    break;
                case GameSessionState.FinishingLevel:
                    message = "Level clear!";
                    break;
                case GameSessionState.GameOver:
                    message = "Game over\nR to try again";
                    break;
                case GameSessionState.Won:
                    message = "You win!\nR to play again";
                    break;
                default:
                    return;
            }

            Vector2 messageSize = font.MeasureString(message);
            Vector2 messagePosition = new Vector2(
                viewport.Center.X - messageSize.X / 2f,
                viewport.Center.Y - messageSize.Y / 2f);
            DrawText(spriteBatch, font, message, messagePosition);
        }

        private static void DrawText(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position)
        {
            spriteBatch.DrawString(font, text, position + new Vector2(2, 2), Color.Black);
            spriteBatch.DrawString(font, text, position, Color.White);
        }

        private static void DrawMenu(SpriteBatch spriteBatch, SpriteFont font, MenuScreen menu, Rectangle viewport)
        {
            List<string> lines = menu.GetLines();
            float y = viewport.Center.Y - lines.Count * font.LineSpacing / 2f;
            for (int index = 0; index < lines.Count; index = index + 1)
            {
                string text = lines[index];
                if (index == menu.SelectedIndex)
                {
                    text = "> " + text;
                }

                Vector2 size = font.MeasureString(text);
                DrawText(spriteBatch, font, text, new Vector2(viewport.Center.X - size.X / 2f, y));
                y = y + font.LineSpacing;
            }
        }
    }
}
