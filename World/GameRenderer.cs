using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;
using System;

namespace TeamUno.Mario.World
{
    internal class GameRenderer : IDisposable
    {
        private readonly Texture2D _backgroundTexture;
        private readonly Texture2D _blockTexture;
        private readonly Texture2D _pixel;

        public GameRenderer(GraphicsDevice graphicsDevice, Texture2D backgroundTexture, Texture2D blockTexture)
        {
            _backgroundTexture = backgroundTexture;
            _blockTexture = blockTexture;
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }

        public void DrawBackground(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Rectangle viewport)
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

        public void DrawHud(SpriteBatch spriteBatch, GameSession session, MenuScreen menu, SpriteFont font, Rectangle viewport)
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

        private void DrawMenu(SpriteBatch spriteBatch, SpriteFont font, MenuScreen menu, Rectangle viewport)
        {
            List<string> lines = menu.GetLines();

            // measure the widest line, including "> " marker, so the panel fits every row
            float widest = 0f;
            for (int index = 0; index < lines.Count; index = index + 1)
            {
                float width = font.MeasureString("> " + lines[index]).X;
                widest = System.Math.Max(widest, width);
            }

            const int padding = 24;
            const int borderThickness = 4;
            float height = lines.Count * font.LineSpacing;

            Rectangle panel = new Rectangle(
                (int)(viewport.Center.X - widest / 2f) - padding,
                (int)(viewport.Center.Y - height / 2f) - padding,
                (int)widest + padding * 2,
                (int)height + padding * 2);

            // semi transparent black background
            spriteBatch.Draw(_pixel, panel, Color.Black * 0.7f);

            // solid black border: top, bottom, left, right
            spriteBatch.Draw(_pixel, new Rectangle(panel.X, panel.Y, panel.Width, borderThickness), Color.Black);
            spriteBatch.Draw(_pixel, new Rectangle(panel.X, panel.Bottom - borderThickness, panel.Width, borderThickness), Color.Black);
            spriteBatch.Draw(_pixel, new Rectangle(panel.X, panel.Y, borderThickness, panel.Height), Color.Black);
            spriteBatch.Draw(_pixel, new Rectangle(panel.Right - borderThickness, panel.Y, borderThickness, panel.Height), Color.Black);

            float y = panel.Y + padding;
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

        // clean up _pixel
        public void Dispose()
        {
            // makes sure pixel isn't null before disposing if never initialized
            _pixel?.Dispose();
        }
    }
}
