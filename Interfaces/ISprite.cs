using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Interfaces
{
    public interface ISprite
    {
        void Reset();

        void Update(GameTime gameTime, ISpriteState spriteState);

        void Draw(SpriteBatch spriteBatch, ISpriteState spriteState);
    }
}
