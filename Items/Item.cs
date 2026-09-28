using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public abstract class Item : IItem
    {
        private readonly ISprite _sprite;
        private readonly SpriteAnimation _animation;
        private readonly Vector2 _startingPosition;
        private readonly int _width;
        private readonly int _height;
        private Vector2 _position;

        public abstract ItemType Type
        {
            get;
        }

        // Item positions are at the bottom center of the sprite
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            protected set
            {
                _position = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X - Width / 2, (int)Position.Y - Height, Width, Height);
            }
        }

        protected Item(Vector2 position, ISprite sprite, SpriteAnimation animation)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            if (animation == null)
            {
                throw new ArgumentNullException(nameof(animation));
            }

            _sprite = sprite;
            _animation = animation;
            _startingPosition = position;
            Position = position;

            Rectangle frame = animation.Frames[0].LeftSource;
            _width = (int)(frame.Width * ItemSpriteFactory.SpriteScale);
            _height = (int)(frame.Height * ItemSpriteFactory.SpriteScale);
            _sprite.Update(new GameTime(), _animation);
        }

        public virtual void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime, _animation);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Bounds, SpriteEffects.None);
        }

        public virtual void Reset()
        {
            Position = _startingPosition;
            _sprite.Reset();
            _sprite.Update(new GameTime(), _animation);
        }
    }
}
