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
        private const float UnderwaterOpacity = 0.82f;

        private readonly Texture2D _titleBackgroundTexture;
        private readonly Texture2D _blockTexture;
        private readonly Texture2D _tileSheetTexture;
        private readonly Texture2D _pixel;

        public GameRenderer(
            GraphicsDevice graphicsDevice,
            Texture2D titleBackgroundTexture,
            Texture2D blockTexture,
            Texture2D tileSheetTexture)
        {
            _titleBackgroundTexture = titleBackgroundTexture;
            _blockTexture = blockTexture;
            _tileSheetTexture = tileSheetTexture;
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }

        public void DrawBackground(
            SpriteBatch spriteBatch,
            Rectangle viewport,
            bool isTitleScreen,
            Color backgroundColor)
        {
            if (isTitleScreen)
            {
                spriteBatch.Draw(_titleBackgroundTexture, viewport, Color.White * 0.45f);
                return;
            }

            spriteBatch.Draw(_pixel, viewport, backgroundColor);
        }

        public void DrawWorld(SpriteBatch spriteBatch, GameSession session, Rectangle viewport)
        {
            float cameraLeft = session.Camera.Position.X;
            DrawTiles(
                spriteBatch,
                session.Level.Definition.BackgroundTiles,
                cameraLeft,
                viewport.Width);

            if (session.IsUnderwater)
            {
                Rectangle water = new Rectangle(0, 0, session.Level.Definition.Width, session.Level.Definition.Height);
                spriteBatch.Draw(
                    _pixel,
                    water,
                    new Color(16, 64, 144) * UnderwaterOpacity);
            }

            Rectangle pulleySource = LevelTile.GetSourceRectangle(391);
            foreach (Rectangle rope in session.Level.PulleyRopes)
            {
                spriteBatch.Draw(_tileSheetTexture, rope, pulleySource, Color.White);
            }

            DrawTiles(
                spriteBatch,
                session.Level.Definition.ForegroundTiles,
                cameraLeft,
                viewport.Width);

            foreach (Block block in session.Level.Blocks)
            {
                if (block.Type == BlockType.Flag)
                {
                    spriteBatch.Draw(
                        _tileSheetTexture,
                        block.Bounds,
                        LevelTile.GetSourceRectangle(152),
                        Color.White);
                }
                else
                {
                    block.Draw(spriteBatch, _blockTexture, _tileSheetTexture);
                }
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

            DrawTiles(
                spriteBatch,
                session.Level.Definition.AboveForegroundTiles,
                cameraLeft,
                viewport.Width);
        }

        private void DrawTiles(
            SpriteBatch spriteBatch,
            IReadOnlyList<LevelTile> tiles,
            float cameraLeft,
            int viewportWidth)
        {
            float cameraRight = cameraLeft + viewportWidth;
            foreach (LevelTile tile in tiles)
            {
                if (tile.Bounds.Right <= cameraLeft || tile.Bounds.Left >= cameraRight)
                {
                    continue;
                }

                spriteBatch.Draw(_tileSheetTexture, tile.Bounds, tile.SourceRectangle, Color.White);
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
            string controls = "Move: A/D or arrows   Sprint: Shift/RT   Jump: W/Space or Pad A   Fire: Z/N or Pad X   Pipe: Down/B   Pause: Esc/Start";
            int panelHeight = 66;
            if (session.Level.Definition.Id == "test")
            {
                panelHeight = 94;
            }
            Rectangle hudPanel = new Rectangle(12, 10, viewport.Width - 24, panelHeight);
            spriteBatch.Draw(_pixel, hudPanel, Color.Black * 0.62f);
            DrawText(spriteBatch, font, progress, new Vector2(24, 14));
            DrawText(spriteBatch, font, controls, new Vector2(24, 42));

            if (session.IsUnderwater)
            {
                DrawText(spriteBatch, font, "Underwater: press jump repeatedly to swim upward", new Vector2(24, 80));
            }
            if (session.Level.Definition.Id == "test")
            {
                string spots = "F3: blocks   F4: items   F5: shells   F6: Bowser   F7: pipes   F9: gap   |   Repeat a key to reload";
                DrawText(spriteBatch, font, spots, new Vector2(24, 70));
            }

            DrawFloatingTexts(spriteBatch, font, session, viewport);

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

        private static void DrawFloatingTexts(
            SpriteBatch spriteBatch,
            SpriteFont font,
            GameSession session,
            Rectangle viewport)
        {
            const float FloatingTextScale = 2f;
            if (session.Level.HasBowser && !session.Level.IsBowserDefeated)
            {
                return;
            }

            Vector2 cameraPosition = session.Camera.Position;
            foreach (FloatingTextSpawnDefinition floatingText in session.Level.Definition.FloatingTexts)
            {
                Vector2 screenPosition = floatingText.Position - cameraPosition;
                Vector2 textSize = font.MeasureString(floatingText.Text) * FloatingTextScale;
                if (screenPosition.X + textSize.X < 0f || screenPosition.X > viewport.Width
                    || screenPosition.Y + textSize.Y < 0f || screenPosition.Y > viewport.Height)
                {
                    continue;
                }

                spriteBatch.DrawString(
                    font,
                    floatingText.Text,
                    screenPosition + new Vector2(2f, 2f),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    FloatingTextScale,
                    SpriteEffects.None,
                    0f);
                spriteBatch.DrawString(
                    font,
                    floatingText.Text,
                    screenPosition,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    FloatingTextScale,
                    SpriteEffects.None,
                    0f);
            }
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
            if (menu.IsLevelSelection && menu.LevelColumnCount > 0)
            {
                DrawLevelMenu(spriteBatch, font, menu, viewport);
                return;
            }

            List<string> lines = menu.GetLines();
            float widest = Math.Max(
                font.MeasureString(menu.Title).X,
                font.MeasureString(menu.Instructions).X);
            foreach (string line in lines)
            {
                widest = Math.Max(widest, font.MeasureString("> " + line).X);
            }

            const int padding = 24;
            int rowHeight = font.LineSpacing + 6;
            int height = (lines.Count + 2) * rowHeight + padding * 2;
            Rectangle panel = new Rectangle(
                viewport.Center.X - (int)widest / 2 - padding,
                viewport.Center.Y - height / 2,
                (int)widest + padding * 2,
                height);

            spriteBatch.Draw(_pixel, viewport, Color.Black * 0.4f);
            spriteBatch.Draw(_pixel, panel, Color.Black * 0.85f);
            DrawCenteredText(spriteBatch, font, menu.Title, viewport.Center.X, panel.Y + padding, Color.White);

            int y = panel.Y + padding + rowHeight;
            for (int index = 0; index < lines.Count; index = index + 1)
            {
                string text = lines[index];
                Color color = Color.White;
                if (index == menu.SelectedIndex)
                {
                    text = "> " + text;
                    color = Color.Yellow;
                }

                spriteBatch.DrawString(font, text, new Vector2(panel.X + padding, y), color);
                y = y + rowHeight;
            }

            DrawCenteredText(
                spriteBatch, font, menu.Instructions, viewport.Center.X, y + 6, Color.LightGray);
        }

        private void DrawLevelMenu(SpriteBatch spriteBatch, SpriteFont font, MenuScreen menu, Rectangle viewport)
        {
            const int padding = 20;
            int columnCount = menu.LevelColumnCount;
            int availableWidth = viewport.Width - 48 - padding * 2;
            int columnWidth = Math.Max(72, availableWidth / columnCount);
            int panelWidth = columnWidth * columnCount + padding * 2;
            int rowHeight = font.LineSpacing + 6;
            int maximumRows = 0;
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                maximumRows = Math.Max(
                    maximumRows,
                    menu.GetLevelColumnIndices(columnIndex).Count);
            }

            int panelHeight = padding * 2 + rowHeight * (maximumRows + 4);
            Rectangle panel = new Rectangle(
                viewport.Center.X - panelWidth / 2,
                viewport.Center.Y - panelHeight / 2,
                panelWidth,
                panelHeight);

            spriteBatch.Draw(_pixel, viewport, Color.Black * 0.4f);
            spriteBatch.Draw(_pixel, panel, Color.Black * 0.85f);
            DrawCenteredText(spriteBatch, font, menu.Title, viewport.Center.X, panel.Y + padding, Color.White);

            int headerY = panel.Y + padding + rowHeight;
            int entriesY = headerY + rowHeight;
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                int columnX = panel.X + padding + columnIndex * columnWidth;
                DrawCenteredText(
                    spriteBatch,
                    font,
                    menu.GetLevelColumnTitle(columnIndex),
                    columnX + columnWidth / 2,
                    headerY,
                    Color.LightGray);

                IReadOnlyList<int> areaIndices = menu.GetLevelColumnIndices(columnIndex);
                for (int row = 0; row < areaIndices.Count; row++)
                {
                    int areaIndex = areaIndices[row];
                    string label = menu.GetLevelLabel(areaIndex);
                    Color color = areaIndex == menu.SelectedIndex ? Color.Yellow : Color.White;
                    if (areaIndex == menu.SelectedIndex)
                    {
                        label = "> " + label;
                    }

                    spriteBatch.DrawString(
                        font,
                        label,
                        new Vector2(columnX + 6, entriesY + row * rowHeight),
                        color);
                }
            }

            int backY = entriesY + maximumRows * rowHeight;
            string backLabel = menu.SelectedIndex == menu.GetLines().Count - 1 ? "> Back" : "Back";
            spriteBatch.DrawString(
                font,
                backLabel,
                new Vector2(panel.X + padding + (columnCount - 1) * columnWidth + 6, backY),
                menu.SelectedIndex == menu.GetLines().Count - 1 ? Color.Yellow : Color.White);
            DrawCenteredText(
                spriteBatch,
                font,
                menu.Instructions,
                viewport.Center.X,
                backY + rowHeight,
                Color.LightGray);
        }

        // clean up _pixel
        public void Dispose()
        {
            // makes sure pixel isn't null before disposing if never initialized
            _pixel?.Dispose();
        }
    }
}
