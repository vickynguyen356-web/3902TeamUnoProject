using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    internal class Star : Item
    {
        // horizontal speed, bounce height, and vertical speed
        private const float MoveSpeed = 90f;
        private const float BounceHeight = 64f;
        private const float BounceSpeed = 160f;
        // remember starting height as the bottom of the bounce
        private readonly float _groundY;
        private bool _movingUp = true;
        
        
        public override ItemType Type
        {
            get
            {
                return ItemType.Star;
            }
        }

        public Star(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateStarAnimation())
        {
            _groundY = position.Y;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // keep moving right throughout the bounce
            float x = Position.X + MoveSpeed * seconds;
            float y = Position.Y;
            if (_movingUp)
            {
                // screen Y decreases when moving up
                y = y - BounceSpeed * seconds;
                // stop at the top of the bounce and switch to moving down
                if (y <= _groundY - BounceHeight)
                {
                    y = _groundY - BounceHeight;
                    _movingUp = false;
                }
            }
            else
            {
                // screen Y increases when moving down
                y = y + BounceSpeed * seconds;
                if (y >= _groundY)
                {
                    // stop at starting height and begin another bounce
                    y = _groundY;
                    _movingUp = true;
                }
            }
            // save updated position and advance sprite animation
            Position = new Vector2(x, y);
            base.Update(gameTime);
        }

        public override void Reset()
        {
            // restore starting position and animation, then bounce up again
            base.Reset();
            _movingUp = true;
        }
    }
}
