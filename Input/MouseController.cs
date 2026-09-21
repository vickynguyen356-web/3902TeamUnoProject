using Microsoft.Xna.Framework.Input;
using Sprint0.Commands;
using Sprint0.Interfaces;

namespace Sprint0.Input
{
    // Optional adapter; the current demo wires only the keyboard controller.
    public class MouseController : IController
    {
        private readonly ICommand _jumpCommand;
        private MouseState _previousMouseState;
        private MouseState _currentMouseState;

        public MouseController(IPlayer player)
        {
            _jumpCommand = new JumpCommand(player);
        }

        public void Update()
        {
            Update(Mouse.GetState());
        }

        public void Update(MouseState mouseState)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = mouseState;

            // Trigger once per right-click, not every frame the button is held.
            if (_currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                _jumpCommand.Execute();
            }
        }
    }
}
