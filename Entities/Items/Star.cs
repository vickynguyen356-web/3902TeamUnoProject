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

        public override ItemType Type
        {
            get
            {
                return ItemType.Star;
            }
        }

        public Star(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateStarAnimation(), 32, 32)
        {
            _startingY = position.Y;
            Velocity = new Vector2(MoveSpeed, -BounceSpeed);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float nextX = Position.X + Velocity.X * elapsedSeconds;
            float nextY = Position.Y;

            if (Velocity.Y < 0f)
            {
                // screen Y decreases when moving up
                nextY = nextY + Velocity.Y * elapsedSeconds;
                // stop at the top of the bounce and switch to moving down
                if (nextY <= _startingY - BounceHeight)
                {
                    nextY = _startingY - BounceHeight;
                    Velocity.Y = -Velocity.Y;
                }
            }
            else if (Velocity.Y > 0f)
            {
                // screen Y increases when moving down
                nextY = nextY + Velocity.Y * elapsedSeconds;
                if (nextY >= _startingY)
                {
                    // stop at starting height and begin another bounce
                    nextY = _startingY;
                    Velocity.Y = -Velocity.Y;
                }
            }

            // save updated position and advance sprite animation
            Position = new Vector2(nextX, nextY);
            base.Update(gameTime);
        }
    }
}
