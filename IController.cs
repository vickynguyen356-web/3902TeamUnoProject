using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Interfaces
{
    public interface IController
    {
        /// <summary>
        /// update internal hardware input states (keyboardstate & mousestate) every frame
        /// </summary>
        void Update();

        /// <summary>
        /// returns true if esc was input to quit
        /// </summary>
        bool EscQuit();

        /// <summary>
        /// returns direction vector (X: -1 to 1, Y: -1 to 1) based on user input
        /// </summary>
        Vector2 GetMovementDirection();

        /// <summary>
        /// true if right click on mouse was presseed; otherwise false
        /// </summary>
        bool IsJumpRequested();
    }

}