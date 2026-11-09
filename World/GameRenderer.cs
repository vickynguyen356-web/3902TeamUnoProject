using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using System.Collections.Generic;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Interfaces;
using System;

namespace TeamUno.Mario.World
{
    internal class GameRenderer : IDisposable
    {
        private const float UnderwaterOpacity = 0.85f;

        private readonly Texture2D _titleBackgroundTexture;
        private readonly Texture2D _blockTexture;
        private readonly Texture2D _pixel;
        private const float CloudParallax = 0.35f;
        private const int CloudSpacing = 560;
        private static readonly int[] CloudHeights = { 112, 208, 144, 256 };

        public GameRenderer(
            GraphicsDevice graphicsDevice,
            Texture2D titleBackgroundTexture,
            Texture2D blockTexture)
        {
            _titleBackgroundTexture = titleBackgroundTexture;
            _blockTexture = blockTexture;
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }

        public void DrawBackground(
            SpriteBatch spriteBatch,
            Rectangle viewport,
            bool isTitleScreen,
            float cameraX)
        {
            if (isTitleScreen)
            {
                spriteBatch.Draw(_titleBackgroundTexture, viewport, Color.White * 0.45f);
                return;
            }

            spriteBatch.Draw(_pixel, viewport, new Color(92, 148, 252));
            DrawClouds(spriteBatch, viewport, cameraX);
        }

        private void DrawClouds(SpriteBatch spriteBatch, Rectangle viewport, float cameraX)
        {
            float horizontalOffset = cameraX * CloudParallax % CloudSpacing;
            float x = viewport.Left - horizontalOffset - CloudSpacing;
            int cloudIndex = -1;

            while (x < viewport.Right)
            {
                cloudIndex = cloudIndex + 1;
                int heightIndex = cloudIndex % CloudHeights.Length;
                DrawCloud(spriteBatch, (int)x, CloudHeights[heightIndex]);
                x = x + CloudSpacing;
            }
        }

        private void DrawCloud(SpriteBatch spriteBatch, int x, int y)
        {
            Color cloudColor = new Color(250, 250, 255);
            spriteBatch.Draw(_pixel, new Rectangle(x + 12, y + 24, 84, 24), cloudColor);
            spriteBatch.Draw(_pixel, new Rectangle(x + 24, y + 12, 36, 36), cloudColor);
            spriteBatch.Draw(_pixel, new Rectangle(x + 48, y, 36, 48), cloudColor);
            spriteBatch.Draw(_pixel, new Rectangle(x + 72, y + 12, 36, 36), cloudColor);
        }

        public void DrawWorld(SpriteBatch spriteBatch, GameSession session)
        {
            if (session.Level.Definition.IsUnderwater)
            {
                Rectangle water = new Rectangle(0, 0, session.Level.Definition.Width, session.Level.Definition.Height);
                spriteBatch.Draw(_pixel, water, new Color(16, 64, 144) * UnderwaterOpacity);
            }

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
            if (session.State == GameSessionState.Title)
            {
                DrawTitleScreen(spriteBatch, font, viewport);
                return;
            }

            string progress = "Area: " + session.Level.Definition.Id
                + "     Score: " + session.Progress.Score.ToString("D6", CultureInfo.InvariantCulture)
                + "     Coins: " + session.Progress.Coins + " / 100"
                + "     Lives: " + session.Progress.Lives
                + "     Time: " + session.TimeRemaining.ToString("D3", CultureInfo.InvariantCulture);
            string controls = "Move: A/D or arrows   Jump: W/Space or Pad A   Fire: Z/N or Pad X   Pipe: Down/B   Menu: Esc/Start";
            Rectangle hudPanel = new Rectangle(12, 10, viewport.Width - 24, 66);
            spriteBatch.Draw(_pixel, hudPanel, Color.Black * 0.62f);
            DrawText(spriteBatch, font, progress, new Vector2(24, 14));
            DrawText(spriteBatch, font, controls, new Vector2(24, 42));

            if (session.Level.Definition.IsUnderwater)
            {
                DrawText(spriteBatch, font, "Underwater: press jump repeatedly to swim upward", new Vector2(24, 80));
            }

            if (session.State == GameSessionState.Menu)
            {
                DrawMenu(spriteBatch, font, menu, viewport);
                return;
            }

            if (session.State == GameSessionState.Won)
            {
                spriteBatch.Draw(_pixel, viewport, Color.Black * 0.62f);
                DrawEndScreen(spriteBatch, font, viewport,
                    "COURSE CLEAR!", "You completed every area.",
                    "Score: " + session.Progress.Score + "     Time: " + session.TimeRemaining,
                    "A / Enter / R: replay     Back / Q: quit");
                return;
            }

            if (session.State == GameSessionState.GameOver)
            {
                spriteBatch.Draw(_pixel, viewport, Color.Black * 0.62f);
                DrawEndScreen(spriteBatch, font, viewport,
                    "GAME OVER", "Your adventure has ended.",
                    "A / Enter / R: try again", "Back / Q: quit");
                return;
            }

            string message;
            switch (session.State)
            {
                case GameSessionState.Paused:
                    message = "Paused\nP or Start to resume";
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

        public void DrawTransitionOverlay(SpriteBatch spriteBatch, GameSession session, Rectangle viewport)
        {
            if (session.State == GameSessionState.PipeTransition && session.PipeTransitionOpacity > 0f)
            {
                spriteBatch.Draw(_pixel, viewport, Color.Black * session.PipeTransitionOpacity);
            }
        }

        private static void DrawTitleScreen(SpriteBatch spriteBatch, SpriteFont font, Rectangle viewport)
        {
            const float titleScale = 4f;
            const float promptScale = 1.5f;
            const float titleOpacity = 0.82f;
            const string title = "TEAM UNO";
            const string startPrompt = "PRESS ENTER TO START";

            Vector2 titleSize = font.MeasureString(title) * titleScale;
            Vector2 titlePosition = new Vector2(
                viewport.Center.X - titleSize.X / 2f,
                viewport.Center.Y - 100f);
            spriteBatch.DrawString(
                font, title, titlePosition + new Vector2(4, 4), Color.Black * titleOpacity,
                0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(
                font, title, titlePosition, Color.White * titleOpacity,
                0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

            Vector2 promptSize = font.MeasureString(startPrompt) * promptScale;
            Vector2 promptPosition = new Vector2(
                viewport.Center.X - promptSize.X / 2f,
                viewport.Center.Y + 100f);
            spriteBatch.DrawString(
                font, startPrompt, promptPosition, Color.White * titleOpacity,
                0f, Vector2.Zero, promptScale, SpriteEffects.None, 0f);
        }

        private void DrawEndScreen(
            SpriteBatch spriteBatch,
            SpriteFont font,
            Rectangle viewport,
            string title,
            string subtitle,
            string primaryAction,
            string secondaryAction)
        {
            const int panelWidth = 640;
            const int panelHeight = 250;
            Rectangle panel = new Rectangle(
                viewport.Center.X - panelWidth / 2,
                viewport.Center.Y - panelHeight / 2,
                panelWidth,
                panelHeight);
            spriteBatch.Draw(_pixel, panel, Color.Black * 0.88f);

            DrawCenteredText(spriteBatch, font, title, viewport.Center.X, panel.Y + 32, Color.White);
            DrawCenteredText(spriteBatch, font, subtitle, viewport.Center.X, panel.Y + 90, Color.White);
            DrawCenteredText(spriteBatch, font, primaryAction, viewport.Center.X, panel.Y + 150, Color.White);
            DrawCenteredText(spriteBatch, font, secondaryAction, viewport.Center.X, panel.Y + 190, Color.White);
        }

        private static void DrawCenteredText(
            SpriteBatch spriteBatch,
            SpriteFont font,
            string text,
            int centerX,
            int y,
            Color color)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 position = new Vector2(centerX - size.X / 2f, y);
            spriteBatch.DrawString(font, text, position + new Vector2(2, 2), Color.Black);
            spriteBatch.DrawString(font, text, position, color);
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
