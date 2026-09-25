using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Interfaces
{ 
    public interface IEnemy
    {
        Vector2 Position { get; }
        Rectangle Bounds { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void TakeDamage();
    }
    }
}
