using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;

namespace TeamUno.Mario.Interfaces
{
    public interface ISprite
    {
        void Reset();

        void Update(GameTime gameTime, SpriteAnimation animation);

        void Draw(SpriteBatch spriteBatch, Rectangle bounds, SpriteEffects facingDirection);
    }
}
