// abstract base class that provides shared functionality for items
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal abstract class Item : IItem
    //class that implements interface
    // follows contract defined by IItem interface
    // items inherit from this class and provide their own item type
    {
        // stores sprite and animation used to display the item
        // readonly = field can be assigned during construction but cannot be reassigned after
        private readonly ISprite _sprite;
        private readonly SpriteAnimation _animation;
        //stores item's collision dimensions
        private readonly int _width;
        private readonly int _height;
        // stores item's current position
        // not readonly because moving items need to change it
        private Vector2 _position;
        private bool _isExpired;
        protected Vector2 Velocity;

        public abstract ItemType Type
        {
            get;
        }

        // this class can change the position.
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

        public Vector2 CurrentVelocity
        {
            get
            {
                return Velocity;
            }
        }

        // provides item width
        public int Width
        {
            get
            {
                return _width;
            }
        }

        // provides item height
        public int Height
        {
            get
            {
                return _height;
            }
        }

        public bool IsExpired
        {
            get
            {
                return _isExpired;
            }
            protected set
            {
                _isExpired = value;
            }
        }

        // used for drawing and collisions
        public Rectangle Bounds
        {
            get
            {
                // turning item's position into a rectangle
                return new Rectangle(
                    (int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        protected Item(Vector2 position, ISprite sprite, SpriteAnimation animation, int width, int height)
        {
            // sprite is required to display the item
            ArgumentNullException.ThrowIfNull(sprite);
            // animation is required to update the sprite
            ArgumentNullException.ThrowIfNull(animation);

            _sprite = sprite;
            _animation = animation;
            // place the item at its starting position
            Position = position;

            _width = width;
            _height = height;
            // set up sprite with its animation
            _sprite.Update(new GameTime(), _animation);
        }

        // update the sprite using the item's animation
        public virtual void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime, _animation);
        }

        public void ApplyMotion(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public void Expire()
        {
            IsExpired = true;
        }

        // draws the sprite inside the item's bounds
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsExpired)
            {
                return;
            }

            _sprite.Draw(spriteBatch, Bounds, FacingDirection.Left);
        }
    }
}
