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
            : base(position, sprite, ItemSpriteFactory.CreateBlockCoinAnimation())
        {
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
            _timer = _timer - elapsedSeconds;

            if (_timer <= 0)
            {
                IsExpired = true;
                return;
            }

            // copy current vertical position
            float nextY = Position.Y;
            // if at least half the time remains, move up.
            if (_timer >= DisplayTime / 2)
            {
                // subtracting moves the coin up
                nextY = nextY - MoveSpeed * elapsedSeconds;
            }
            else
            {
                // second half, move down by adding to the Y
                nextY = nextY + MoveSpeed * elapsedSeconds;
            }

            // apply the new Y
            Position = new Vector2(Position.X, nextY);
            base.Update(gameTime);
        }
    }
}
