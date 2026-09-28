using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public class BlockCoin : Item
    {
        private const float MoveSpeed = 180f;
        private const float DisplayTime = 0.6f;

        private float _timer = DisplayTime;
        private bool _isVisible = true;

        public override ItemType Type
        {
            get
            {
                return ItemType.BlockCoin;
            }
        }

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

        public BlockCoin(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateBlockCoinAnimation())
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
            {
                return;
            }

            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            _timer = _timer - seconds;
            if (_timer <= 0)
            {
                IsVisible = false;
                return;
            }

            float y = Position.Y;
            if (_timer >= DisplayTime / 2)
            {
                y = y - MoveSpeed * seconds;
            }
            else
            {
                y = y + MoveSpeed * seconds;
            }
            Position = new Vector2(Position.X, y);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                base.Draw(spriteBatch);
            }
        }

        public override void Reset()
        {
            base.Reset();
            _timer = DisplayTime;
            IsVisible = true;
        }
    }
}
