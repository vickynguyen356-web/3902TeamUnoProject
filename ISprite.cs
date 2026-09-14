using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Interfaces
{
    public interface ISprite
    {
        /// <summary>
        /// handles internal entity updates 
        /// </summary>
        /// <param name="gameTime"> provides snapshot of timing values </param>
        /// <param name="controller"> input mechanism that drives char's behavior </param>
        void Update(GameTime gameTime, IController controller);

        /// <summary>
        /// renders target sprite grid
        /// </summary>
        /// <param name="spriteBatch"></param>
        void Draw(SpriteBatch spriteBatch);
    }
}
