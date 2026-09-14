using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Interfaces;
using System;

namespace MonoGameLibrary.Entities
{
    public class KeyboardPlayer : ISprite
    {
        private Texture2D _spriteSheet;
        private Vector2 _pos;
        private Vector2 _velocity;
        private bool _isGrounded;

        /* physics */ 
        private const float Acceleration = 1200f;
        private const float Friction = 8.0f;
        private const float Gravity = 1200f;
        private const float JumpVelocity = -300f;
        private float FloorY; // floor pixel coordinate

        // sprite
        public int _frameWidth;
        public int _frameHeight;
        private int _currentCol = 1;
        private int _currentRow = 0;
        private int[] _runAnimationColSeq = new int[] { 0, 3, 3, 3 };
        private int[] _runAnimationRowSeq = new int[] { 0, 1, 2, 1 };
        private int _seqIndex = 0;
        SpriteEffects _facingDirection = SpriteEffects.None;
        private double _animationTimer;
        private const float spriteScale = 5.0f;

        // grid configuartion 
        private const int TotalCols = 4;
        private const int TotalRows = 5;

        public KeyboardPlayer(Texture2D texture, Vector2 startingPos, float groundY)
        {
            _spriteSheet = texture;
            _pos = startingPos;
            _velocity = Vector2.Zero;
            FloorY = groundY;

            // divide texture width evenly by # of horizontal frames
            _frameWidth = _spriteSheet.Width / TotalCols;
            _frameHeight = _spriteSheet.Height / TotalRows;
        }

        /// <summary>
        /// updates keyboardPlayer state
        /// </summary>
        /// <param name="gameTime"> to capture snapshot of GameTime state </param>
        /// <param name="controller"></param>
        public void Update(GameTime gameTime, IController controller)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyState = Keyboard.GetState();
            Vector2 movement = controller.GetMovementDirection();

            // logic for sprite facing direction based on horizontal movement
            if (keyState.IsKeyDown(Keys.D))
            {
                _facingDirection = SpriteEffects.FlipHorizontally;
            }
            else if (keyState.IsKeyDown(Keys.A)) 
            {
                _facingDirection = SpriteEffects.None;
            }
            
            // calculations for horizontal sprite movement
            _velocity.X += movement.X * Acceleration * t;
            _velocity.X -= _velocity.X * Friction * t;
            _pos += _velocity * t;

            if (controller.IsJumpRequested() && _isGrounded)
            {
                _velocity.Y = JumpVelocity;
                _isGrounded = false;
            }

            // if char in mid-air, apply gravity 
            if (!_isGrounded)
            {
                _velocity.Y += Gravity * t;
            }
            else
            {
                _velocity.Y = 0; 
            }

            // apply physics updates to actual screen position of char
            _pos += _velocity * t;

            // stop player when they hit the floor
            if (_pos.Y >= FloorY)
            {
                _pos.Y = FloorY;
                _velocity.Y = 0;
                _isGrounded = true;
            }

            // screen bounds
            float scaledWidth = _frameWidth * spriteScale;
            float scaledHeight = _frameHeight * spriteScale;

            _pos.X = MathHelper.Clamp(_pos.X, 0, 1280 - scaledWidth);
            _pos.Y = MathHelper.Clamp(_pos.Y, 0, 720 - scaledHeight);

            // update animation state
            DetermineAnimationFrame(gameTime);
        }

        /// <summary>
        /// helper that handles visual states
        /// </summary>
        private void DetermineAnimationFrame(GameTime gameTime)
        {
            float horizontalSpeed = Math.Abs(_velocity.X);

            /* char is airborne */
            if (!_isGrounded)
            {
                _animationTimer = 0;
                _seqIndex = 0;

                if (_velocity.Y < -200f) {
                    // jumping up
                    _currentCol = 0;
                    _currentRow = 3;
                } else if (_velocity.Y <= 200f)
                {
                    // near or at peak
                    _currentCol = 0;
                    _currentRow = 4;
                } else
                {
                    // falling
                    _currentCol = 3;
                    _currentRow = 2;
                }

                return;
            }

            /* running */
            // if char is moving at speed threshold, cycle frames
            if (horizontalSpeed > 15f)
            {
                // clamp speed to avoid too fast animation
                float speedMultiplier = MathHelper.Clamp(horizontalSpeed * 0.05f, 0.05f, 3.0f);
                double dynamicFrameDuration = 0.25 / speedMultiplier;

                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

                while (_animationTimer >= dynamicFrameDuration)
                {
                    _animationTimer -= dynamicFrameDuration;
                    _seqIndex++;

                    // wrap around array length 
                    if (_seqIndex >= _runAnimationColSeq.Length)
                    {
                        _seqIndex = 0;
                    }
                }

                // assign sprite sheet col/row based on current sequence index
                _currentCol = _runAnimationColSeq[_seqIndex];
                _currentRow = _runAnimationRowSeq[_seqIndex];

                return;
            }

            /* char is idle */
            _currentCol = 1;
            _currentRow = 0;

            _seqIndex = 0;
            _animationTimer = 0;

        }

        /// <summary>
        /// fulfills ISprite interface contract
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // point to target col/row index in spritesheet 
            int sourceX = _currentCol * _frameWidth;
            int sourceY = _currentRow * _frameHeight;

            Rectangle sourceRect = new Rectangle(
                sourceX,
                sourceY,
                _frameWidth,
                _frameHeight
                );
                
             spriteBatch.Draw(
                _spriteSheet,
                _pos,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                spriteScale,
                _facingDirection,
                0f
                );
        } 
    }
}
