using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Entities;

namespace Sprint0.Interfaces
{
    public interface ISprite
    {
        void Reset();

        void Update(GameTime gameTime, SpriteAnimation animation);

        void Draw(SpriteBatch spriteBatch, Rectangle bounds, SpriteEffects facingDirection);
    }
}
