using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.World;

namespace TeamUno.Mario.Input
{
    internal class GamepadController : IController
    {
        private const float StickDeadZone = 0.25f;
        private readonly IPlayer _player;
        private readonly GameSession _session;
        private GamePadState _previousState;
        private bool _isJumpHeld;
<<<<<<< Updated upstream
=======
        private bool _isSprintHeld;
>>>>>>> Stashed changes

        public bool IsJumpHeld
        {
            get
            {
                return _isJumpHeld;
            }
        }

<<<<<<< Updated upstream
=======
        public bool IsSprintHeld
        {
            get
            {
                return _isSprintHeld;
            }
        }

>>>>>>> Stashed changes
        public GamepadController(IPlayer player, GameSession session)
        {
            System.ArgumentNullException.ThrowIfNull(player);
            System.ArgumentNullException.ThrowIfNull(session);
            _player = player;
            _session = session;
        }

        public void Update()
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
            bool connected = state.IsConnected;

            if (WasPressed(state, Buttons.Back))
            {
                if (_session.State == GameSessionState.Won || _session.State == GameSessionState.GameOver)
                {
                    _session.Quit();
                }
                else
                {
                    _session.ToggleMute();
                }
            }

            bool resetRequested = false;
            if (WasPressed(state, Buttons.Y))
            {
                _session.Reset();
                resetRequested = true;
            }
            else if ((_session.State == GameSessionState.Won || _session.State == GameSessionState.GameOver)
                && (WasPressed(state, Buttons.A) || WasPressed(state, Buttons.Start)))
            {
                _session.Reset();
                resetRequested = true;
            }

            _isJumpHeld = connected && state.IsButtonDown(Buttons.A);
<<<<<<< Updated upstream
=======
            _isSprintHeld = _session.CanControlPlayer && connected
                && state.Triggers.Right > StickDeadZone;
>>>>>>> Stashed changes
            if (_session.CanControlPlayer && !resetRequested)
            {
                float movement = 0f;
                bool moveLeft = state.IsButtonDown(Buttons.DPadLeft)
                    || state.ThumbSticks.Left.X < -StickDeadZone;
                bool moveRight = state.IsButtonDown(Buttons.DPadRight)
                    || state.ThumbSticks.Left.X > StickDeadZone;
                if (moveLeft != moveRight)
                {
                    movement = moveLeft ? -1f : 1f;
                }

<<<<<<< Updated upstream
                if (moveLeft || moveRight)
=======
                if (movement != 0f)
>>>>>>> Stashed changes
                {
                    _player.Move(movement);
                }

<<<<<<< Updated upstream
                if (connected && state.IsButtonDown(Buttons.B))
                {
                    _player.SetCrouching(true);
                }
=======
                _player.SetCrouching(connected && state.IsButtonDown(Buttons.B));
>>>>>>> Stashed changes

                float verticalStick = connected ? -state.ThumbSticks.Left.Y : 0f;
                if (System.Math.Abs(verticalStick) > StickDeadZone)
                {
                    _player.Climb(verticalStick);
                }

                if (WasPressed(state, Buttons.A))
                {
                    _player.Jump();
                }

                if (WasPressed(state, Buttons.X))
                {
                    _player.ThrowFireball();
                }

                if (WasPressed(state, Buttons.B))
                {
                    _player.RequestPipeEntry();
                }

                if (WasPressed(state, Buttons.Start))
                {
                    _session.TogglePause();
                }
            }

            _previousState = state;
        }

        public void ResetInput()
        {
            _previousState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
            _isJumpHeld = false;
<<<<<<< Updated upstream
=======
            _isSprintHeld = false;
>>>>>>> Stashed changes
        }

        private bool WasPressed(GamePadState state, Buttons button)
        {
            return state.IsConnected
                && state.IsButtonDown(button)
                && (!_previousState.IsConnected || _previousState.IsButtonUp(button));
        }
    }
}
