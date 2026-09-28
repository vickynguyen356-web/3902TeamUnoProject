using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public class Star : Item
    {
        private const float MoveSpeed = 90f;
        private const float BounceHeight = 64f;
        private const float BounceSpeed = 160f;

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
            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            float x = Position.X + MoveSpeed * seconds;
            float y = Position.Y;
            if (_movingUp)
            {
                y = y - BounceSpeed * seconds;
                if (y <= _groundY - BounceHeight)
                {
                    y = _groundY - BounceHeight;
                    _movingUp = false;
                }
            }
            else
            {
                y = y + BounceSpeed * seconds;
                if (y >= _groundY)
                {
                    y = _groundY;
                    _movingUp = true;
                }
            }
            Position = new Vector2(x, y);
            base.Update(gameTime);
        }

        public override void Reset()
        {
            base.Reset();
            _movingUp = true;
        }
    }
}
