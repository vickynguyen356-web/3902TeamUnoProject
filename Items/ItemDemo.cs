using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Sprint0.Entities;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class ItemDemo
    {
        private const float StartingX = 32f;
        private IItem[] items;
        private float rightEdge;
        private int currentIndex;
        private SpriteFont controlsFont;
        private string[] itemNames =
        {
            "Mushroom", "Fire Flower", "Floating Coin", "Star", "1-Up Mushroom", "Block Coin"
        };

        private ItemSelection selection = new ItemSelection();

        public ItemSelection Selection
        {
            get { return selection; }
        }

        public IItem CurrentItem
        {
            get { return items[currentIndex]; }
        }

        public ItemDemo(ContentManager content, SpriteFont controlsFont, int floorY, int windowWidth)
        {
            items = CreateItems(content, new Vector2(StartingX, floorY));
            rightEdge = windowWidth + 16;
            this.controlsFont = controlsFont;
        }

        private static IItem[] CreateItems(ContentManager content, Vector2 position)
        {
            ItemSpriteFactory factory = ItemSpriteFactory.Instance;
            factory.LoadAllTextures(content);
            return new IItem[]
            {
                new Mushroom(position, factory.CreateMushroomSprite()),
                new FireFlower(position, factory.CreateFireFlowerSprite()),
                new Coin(position, factory.CreateCoinSprite()),
                new Star(position, factory.CreateStarSprite()),
                new OneUpMushroom(position, factory.CreateOneUpMushroomSprite()),
                new BlockCoin(position, factory.CreateBlockCoinSprite())
            };
        }

        public void Cycle(int direction)
        {
            currentIndex += direction;

            if (currentIndex < 0)
            {
                currentIndex = items.Length - 1;
            }
            else if (currentIndex >= items.Length)
            {
                currentIndex = 0;
            }

            CurrentItem.Reset();
        }

        public void Update(GameTime gameTime)
        {
            CurrentItem.Update(gameTime);
            // Restart moving items so they remain available in the Sprint 2 demo.
            if (CurrentItem.Position.X > rightEdge)
            {
                CurrentItem.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentItem.Draw(spriteBatch);
            if (controlsFont != null)
            {
                DrawControls(spriteBatch);
            }
        }

        private void DrawControls(SpriteBatch spriteBatch)
        {
            DrawControlText(spriteBatch, "U/I: previous/next demo item   Showing: " + itemNames[(int)CurrentItem.Type], 152);
            DrawControlText(spriteBatch, "Select: 1 Mushroom  2 Flower  3 Floating Coin  4 Star  5 1-Up  6 Block Coin", 184);
            DrawControlText(spriteBatch, "Selected item: " + itemNames[(int)selection.SelectedItem], 216);
        }

        private void DrawControlText(SpriteBatch spriteBatch, string text, float y)
        {
            spriteBatch.DrawString(controlsFont, text, new Vector2(32, y), Color.White);
        }

        public void Reset()
        {
            currentIndex = 0;
            selection.Reset();
            foreach (IItem item in items)
            {
                item.Reset();
            }
        }
    }
}
