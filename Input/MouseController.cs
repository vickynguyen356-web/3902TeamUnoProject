using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
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

            if (_currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                _jumpCommand.Execute();
            }
        }
    }
}
