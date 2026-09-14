using Microsoft.Xna.Framework;
using MonoGameLibrary.Interfaces;

namespace MonoGameLibrary.Input
{
    public class CombinedController : IController
    {
        private KeyboardController _keyboard;
        private MouseController _mouse;

        /* implements IController interface by combining keyboard & mouse input handling */
        public CombinedController()
        {
            _keyboard = new KeyboardController();
            _mouse = new MouseController();
        }

        public void Update()
        {
            _keyboard.Update();
            _mouse.Update();
        }

        public bool EscQuit()
        {
            return _keyboard.EscQuit();
        }

        public Vector2 GetMovementDirection()
        {
            return _keyboard.GetMovementDirection();
        }

        public bool IsJumpRequested()
        {
            return _mouse.IsJumpRequested();
        }
    }
}