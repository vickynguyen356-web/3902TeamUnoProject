// provides Math.Min
using System;
// provides position and timing types: Vector2 and GameTime
using Microsoft.Xna.Framework;
// provides SpriteBatch for drawing sprites
using Microsoft.Xna.Framework.Graphics;
// provides the ISprite interface used by the constructor
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    // inherits shared item functionality from Item class
    internal class BlockCoin : Item
    {
        private const float MoveSpeed = 180f;
        // half is used for rising and half for falling.
        private const float DisplayTime = 0.6f;

        // stores remaining lifetime of this coin
        private float _timer = DisplayTime;
        // the coin starts visible.
        private bool _isVisible = true;
        // overrides inherited Type to identify the item --> block coin
        public override ItemType Type
        {
            get
            {
                return ItemType.BlockCoin;
            }
        }
        // other code can read this property, but only this class can use its setter to change visibility
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            private set
            {
                _isVisible = value;
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
            // stop immediately if coin is already hidden
            if (!IsVisible)
            {
                return;
            }
            // read elapsed time and convert it to float.
            // 1/30 of a second per update
            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            // coin's remaining time
            _timer = _timer - seconds;
            // hide coin when its time ends
            if (_timer <= 0)
            {
                IsVisible = false;
                return;
            }
            // copy current vertical position
            float y = Position.Y;
            // if at least half the time remains, move up.
            if (_timer >= DisplayTime / 2)
            {
                // subtracting moves the coin up
                y = y - MoveSpeed * seconds;
            }
            else
            {
                // second half, move down by adding to the Y
                y = y + MoveSpeed * seconds;
            }
            // apply the new Y
            Position = new Vector2(Position.X, y);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // use Item's drawing behavior only while coin is visible
            if (IsVisible)
            {
                base.Draw(spriteBatch);
            }
        }
        // restarts coin's timer and makes it visible again
        public override void Reset()
        {
            base.Reset();
            // restore the full time
            _timer = DisplayTime;
            // allow coin to update and draw again
            IsVisible = true;
        }
    }
}
