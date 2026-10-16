using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Commands;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Input;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal enum GameSessionState
    {
        Playing,
        Paused,
        Menu
    }

    internal class GameSession : IGameActions
    {
        private readonly Texture2D _marioTexture;
        private readonly int _viewportWidth;
        private readonly IController _controller;
        private readonly ICommand _quitCommand;
        private readonly ICommand _resetCommand;
        private readonly List<ICommand> _pendingPlayerCommands = new List<ICommand>();
        private Dictionary<InputAction, ICommand> _playerCommands;
        private ICommand _standCommand;
        private Level _level;
        private GameSessionState _state;
        private bool _shouldExit;

        public Level Level
        {
            get
            {
                return _level;
            }
        }

        public GameSessionState State
        {
            get
            {
                return _state;
            }
        }

        public bool ShouldExit
        {
            get
            {
                return _shouldExit;
            }
            private set
            {
                _shouldExit = value;
            }
        }

        public GameSession(Texture2D marioTexture, int viewportWidth, IController controller)
        {
            ArgumentNullException.ThrowIfNull(marioTexture);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(viewportWidth);
            ArgumentNullException.ThrowIfNull(controller);

            _marioTexture = marioTexture;
            _viewportWidth = viewportWidth;
            _controller = controller;
            _quitCommand = new QuitCommand(this);
            _resetCommand = new ResetCommand(this);
            StartNewGame();
        }

        public void Update(GameTime gameTime)
        {
            _controller.Update();
            if (TryExecuteSessionCommand())
            {
                return;
            }

            if (ShouldExit || State != GameSessionState.Playing)
            {
                return;
            }

            PreparePlayerCommands();
            Level.Update(gameTime, _pendingPlayerCommands, _controller.IsJumpHeld);
        }

        public void StartNewGame()
        {
            CreateLevel(LevelLayouts.CreateFirstLevel(), PlayerForm.Fire);
        }

        public void LoadLevel(LevelDefinition definition)
        {
            CreateLevel(definition, Level.Player.Form);
        }

        private void CreateLevel(LevelDefinition definition, PlayerForm startingForm)
        {
            ArgumentNullException.ThrowIfNull(definition);

            ISprite playerSprite = MarioSpriteFactory.Create(_marioTexture);
            _level = new Level(definition, playerSprite, _viewportWidth, startingForm);
            BindPlayerCommands();
            ShouldExit = false;
            _state = GameSessionState.Playing;
        }

        private void BindPlayerCommands()
        {
            _playerCommands = new Dictionary<InputAction, ICommand>
            {
                { InputAction.MoveLeft, new MoveCommand(Level.Player, -1) },
                { InputAction.MoveRight, new MoveCommand(Level.Player, 1) },
                { InputAction.Crouch, new CrouchCommand(Level.Player, true) },
                { InputAction.Jump, new JumpCommand(Level.Player) },
                { InputAction.ThrowFireball, new ThrowFireballCommand(Level.Player) }
            };
            _standCommand = new CrouchCommand(Level.Player, false);
            _pendingPlayerCommands.Clear();
        }

        public void Pause()
        {
            _state = GameSessionState.Paused;
        }

        public void OpenMenu()
        {
            _state = GameSessionState.Menu;
        }

        public void Resume()
        {
            _state = GameSessionState.Playing;
        }

        public void Quit()
        {
            ShouldExit = true;
        }

        public void Reset()
        {
            ShouldExit = false;
            Level.Reset();
            _state = GameSessionState.Playing;
        }

        private bool TryExecuteSessionCommand()
        {
            bool resetRequested = false;
            foreach (InputAction action in _controller.Actions)
            {
                if (action == InputAction.Quit)
                {
                    _quitCommand.Execute();
                    return true;
                }

                if (action == InputAction.Reset)
                {
                    resetRequested = true;
                }
            }

            if (resetRequested)
            {
                _resetCommand.Execute();
                return true;
            }

            return false;
        }

        private void PreparePlayerCommands()
        {
            _pendingPlayerCommands.Clear();
            bool movingLeft = false;
            bool movingRight = false;
            bool crouching = false;

            foreach (InputAction action in _controller.Actions)
            {
                if (action == InputAction.MoveLeft)
                {
                    movingLeft = true;
                }
                else if (action == InputAction.MoveRight)
                {
                    movingRight = true;
                }
                else if (action == InputAction.Crouch)
                {
                    crouching = true;
                }
            }

            if (!crouching)
            {
                _pendingPlayerCommands.Add(_standCommand);
            }

            foreach (InputAction action in _controller.Actions)
            {
                if (movingLeft && movingRight
                    && (action == InputAction.MoveLeft || action == InputAction.MoveRight))
                {
                    continue;
                }

                ICommand command;
                if (_playerCommands.TryGetValue(action, out command))
                {
                    _pendingPlayerCommands.Add(command);
                }
            }
        }
    }
}
