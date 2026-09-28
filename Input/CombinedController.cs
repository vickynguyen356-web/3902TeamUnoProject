using System;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Input
{
    public class CombinedController : IController
    {
        private readonly KeyboardInput _input;
        private readonly ICommand _quitCommand;
        private readonly ICommand _resetCommand;
        private readonly IController[] _controllers;
        private bool _resetThisFrame;

        public bool ResetThisFrame
        {
            get
            {
                return _resetThisFrame;
            }
            private set
            {
                _resetThisFrame = value;
            }
        }

        public CombinedController(KeyboardInput input, IGameActions gameActions, params IController[] controllers)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (gameActions == null)
            {
                throw new ArgumentNullException(nameof(gameActions));
            }

            _input = input;
            _quitCommand = new QuitCommand(gameActions);
            _resetCommand = new ResetCommand(gameActions);
            _controllers = (IController[])controllers.Clone();
        }

        public void Update()
        {
            Update(Keyboard.GetState());
        }

        public void Update(KeyboardState keyboardState)
        {
            _input.Update(keyboardState);
            ResetThisFrame = false;

            if (_input.WasPressed(Keys.Q) || _input.WasPressed(Keys.Escape))
            {
                _quitCommand.Execute();
                return;
            }

            if (_input.WasPressed(Keys.R))
            {
                _resetCommand.Execute();
                ResetThisFrame = true;
                return;
            }

            foreach (IController controller in _controllers)
            {
                controller.Update();
            }
        }
    }
}
