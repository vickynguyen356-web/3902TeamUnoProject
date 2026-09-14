using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Interfaces;

namespace MonoGameLibrary.Entities
{
    public class Player : IPlayer
    {
        private Texture2D _spriteSheet;
        private Vector2 _pos;
        private Vector2 _velocity;
        private bool _isGrounded;

        // physics 
        private const float Gravity = 1200f;
        private const float JumpVelocity = -800f;
        private const float FloorY = 400f; // floor pixel coordinate

        // sprite grid tracking
        private int _currentCol = 0;
        private int _currentRow = 0;

        // grid configuartion 
        private const int TotalCols = 4;
        private const int TotalRows = 5;
        private int _frameWidth;
        private int _frameHeight;

        public Player(Texture2D texture, Vector2 startingPos)
        {
            _spriteSheet = texture;
            _pos = startingPos;

            // divide texture width evenly by # of horizontal frames
            _frameWidth = _spriteSheet.Width / TotalCols;
            _frameHeight = _spriteSheet.Height / TotalRows;
        }

        public void Update(GameTime gameTime, IController controller)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // if char in mid-air, apply gravity 
            if (!_isGrounded)
            {
                _velocity.Y += Gravity * t;
            } else
            {
                _velocity.Y = 0; // no velocity when touching ground
            }

            // wait for jump request
            if (controller.IsJumpRequested() && _isGrounded)
            {
                _velocity.Y = JumpVelocity;
                _isGrounded = false;
            }

            // apply physics updates to actual screen position of char
            _pos += _velocity * t;

            // stop player when they hit the floor
            if (_pos.Y >= FloorY)
            {
                _pos.Y = FloorY;
                _isGrounded = true;
            }

            // update jump animation state
            DetermineAnimationFrame();
        }


        /// <summary>
        /// selects rows & columns from sprite sheet based on Y-velocity
        /// </summary>
        private void DetermineAnimationFrame()
        {
            if (_isGrounded)
            {
                // idle 
                _currentRow = 0;
                _currentCol = 1;
            } else
            {
                if (_velocity.Y < -200f)
                {
                    // jumping up
                    _currentRow = 4;
                    _currentCol = 0;
                }
                else if (_velocity.Y >= -200f && _velocity.Y <= 200f)
                {
                    // at peak 
                    _currentRow = 5;
                    _currentCol = 0;
                }
                else if (_velocity.Y > 200f)
                {
                    // falling down
                    _currentRow = 3;
                    _currentCol = 4;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // calculate where source pixel starts on sheet
            int sourceX = _currentCol * _frameWidth;
            int sourceY = _currentRow * _frameHeight;

            // slice out 1 sprite from grid
            Rectangle sourceRect = new Rectangle(sourceX, sourceY, _frameWidth, _frameHeight);

            // render frame to game window
            spriteBatch.Draw(_spriteSheet, _pos, sourceRect, Color.White);
        }
    }
}
