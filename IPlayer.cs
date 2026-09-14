using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace MonoGameLibrary.Interfaces
{
    public interface IPlayer
    {
        /// <summary>
        /// update player state
        /// </summary>
        public void Update(GameTime gameTime, IController controller);

        /// <summary>
        /// updates player animation state (idle, walking, jumping)
        /// </summary>
        public void Draw(SpriteBatch spriteBatch);

    }

}