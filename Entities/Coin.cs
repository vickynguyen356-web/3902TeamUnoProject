using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Entities
{
    public class Coin
    {
        public Vector2 Position { get; private set; }

        public Coin(Vector2 position)
        {
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            // Item animation and state changes go here.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Item drawing goes here.
        }
    }
}
