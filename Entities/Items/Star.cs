using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class Star : Item
    {
        // horizontal speed, bounce height, and vertical speed
        private const float MoveSpeed = 90f;
        private const float BounceHeight = 64f;
        private const float BounceSpeed = 160f;
        // remember starting height as the bottom of the bounce
        private readonly float _startingY;
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
            _startingY = position.Y;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // keep moving right throughout the bounce
            float nextX = Position.X + MoveSpeed * elapsedSeconds;
            float nextY = Position.Y;

            if (_movingUp)
            {
                // screen Y decreases when moving up
                nextY = nextY - BounceSpeed * elapsedSeconds;
                // stop at the top of the bounce and switch to moving down
                if (nextY <= _startingY - BounceHeight)
                {
                    nextY = _startingY - BounceHeight;
                    _movingUp = false;
                }
            }
            else
            {
                // screen Y increases when moving down
                nextY = nextY + BounceSpeed * elapsedSeconds;
                if (nextY >= _startingY)
                {
                    // stop at starting height and begin another bounce
                    nextY = _startingY;
                    _movingUp = true;
                }
            }

            // save updated position and advance sprite animation
            Position = new Vector2(nextX, nextY);
            base.Update(gameTime);
        }
    }
}
