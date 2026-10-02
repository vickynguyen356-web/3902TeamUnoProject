using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Items;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;

namespace TeamUno.Mario.World
{
    public class GameRenderer
    {
        private readonly Texture2D _backgroundTexture;
        private readonly Texture2D _whitePixelTexture;
        private readonly Texture2D _blockTexture;
        private readonly SpriteFont _controlsFont;

        public GameRenderer(Texture2D backgroundTexture, Texture2D whitePixelTexture,
            Texture2D blockTexture, SpriteFont controlsFont)
        {
            _backgroundTexture = backgroundTexture;
            _whitePixelTexture = whitePixelTexture;
            _blockTexture = blockTexture;
            _controlsFont = controlsFont;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle viewport, GameSession session)
        {
            // don't draw background for now
            // spriteBatch.Draw(_backgroundTexture, viewport, Color.White * 0.45f);
            DrawFloor(spriteBatch, viewport, session.Level.Definition.FloorY);
            DrawLevel(spriteBatch, session.Level);
            session.Player.Draw(spriteBatch);
            DrawControls(spriteBatch);
            foreach (IItem item in session.Level.Items)
            {
                item.Draw(spriteBatch);
            }

            IDemoControls demoControls = session.Level as IDemoControls;
            if (demoControls != null)
            {
                DrawDemoControls(spriteBatch, demoControls);
            }
        }

        private void DrawFloor(SpriteBatch spriteBatch, Rectangle viewport, int floorY)
        {
            Rectangle floorBounds = new Rectangle(viewport.Left, floorY, viewport.Width, viewport.Bottom - floorY);
            spriteBatch.Draw(_whitePixelTexture, floorBounds, Color.SaddleBrown);
        }

        private void DrawLevel(SpriteBatch spriteBatch, Level level)
        {
            foreach (Block block in level.Blocks)
            {
                block.Draw(spriteBatch, _blockTexture);
            }

            foreach (IEnemy enemy in level.Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach (BowserFireball fireball in level.Fireballs)
            {
                fireball.Draw(spriteBatch);
            }

            foreach (IProjectile projectile in level.Projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }

        private void DrawControls(SpriteBatch spriteBatch)
        {
            DrawControlText(spriteBatch, "A/D or Left/Right: move   W/Up/Space: jump", 24);
            DrawControlText(spriteBatch, "S/Down: crouch   Z/N: throw fireball   E: damage", 56);
            DrawControlText(spriteBatch, "R: reset   Q/Escape: quit", 88);
        }

        private void DrawDemoControls(SpriteBatch spriteBatch, IDemoControls demoControls)
        {
            DrawControlText(spriteBatch, "T/Y: previous/next block   O/P: previous/next enemy", 120);
            DrawControlText(spriteBatch, "U/I: previous/next demo item   Showing: " + GetItemName(demoControls.SelectedItem), 152);
            DrawControlText(spriteBatch, "Show: 1 Mushroom  2 Flower  3 Floating Coin  4 Star  5 1-Up  6 Block Coin", 184);
        }

        private void DrawControlText(SpriteBatch spriteBatch, string text, float y)
        {
            spriteBatch.DrawString(_controlsFont, text, new Vector2(32, y), Color.White);
        }

        private static string GetItemName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Mushroom:
                    return "Mushroom";
                case ItemType.FireFlower:
                    return "Fire Flower";
                case ItemType.FloatingCoin:
                    return "Floating Coin";
                case ItemType.Star:
                    return "Star";
                case ItemType.OneUpMushroom:
                    return "1-Up Mushroom";
                case ItemType.BlockCoin:
                    return "Block Coin";
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType));
            }
        }
    }
}
