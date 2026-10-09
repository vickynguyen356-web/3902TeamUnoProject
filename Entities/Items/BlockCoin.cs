// provides position and timing types: Vector2 and GameTime
using Microsoft.Xna.Framework;
// provides the ISprite interface used by the constructor
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    // inherits shared item functionality from Item class
    internal class BlockCoin : Item
    {
        private const float MoveSpeed = 180f;
        // half is used for rising and half for falling.
        private const float DisplayTime = 0.6f;

        // stores remaining lifetime of this coin
        private float _timer = DisplayTime;

        // overrides inherited Type to identify the item --> block coin
        public override ItemType Type
        {
            get
            {
                return ItemType.BlockCoin;
            }
        }

        // runs when a BlockCoin is created. passes its position, sprite, and block coin animation to Item constructor
        public BlockCoin(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateBlockCoinAnimation(), 16, 32)
        {
            Velocity = new Vector2(0f, -MoveSpeed);
        }

        // updates the coin's remaining lifetime and vertical position
        public override void Update(GameTime gameTime)
        {
            if (IsExpired)
            {
                return;
            }

            // read elapsed time and convert it to float.
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // coin's remaining time
            float previousTime = _timer;
            _timer = _timer - elapsedSeconds;

            if (_timer <= 0)
            {
                IsExpired = true;
                return;
            }

            if (previousTime >= DisplayTime / 2 && _timer < DisplayTime / 2)
            {
                Velocity.Y = -Velocity.Y;
            }

            Position = Position + Velocity * elapsedSeconds;
            base.Update(gameTime);
        }
    }
}
