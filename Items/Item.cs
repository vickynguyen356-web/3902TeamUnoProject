// abstract base class that provides shared functionality for items
// stores position, size, calculates bounds, and provides methods for update, draw, and reset
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
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
        // saves orignal position so item can be reset
        private readonly Vector2 _startingPosition;
        //stores item's scaled dimensions
        private readonly int _width;
        private readonly int _height;
        // stores item's current position
        // not readonly because moving items need to change it
        private Vector2 _position;

        public abstract ItemType Type
        {
            get;
        }

        // Item positions are at the bottom center of the sprite
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
        // converts bottom center position into a rectangle
        // used for drawing and collisions
        public Rectangle Bounds
        {
            get
            {
                // turning item's position into a rectangle
                return new Rectangle(
                // turns the item’s bottom-center position into the top-left position needed to create its rectangle
                    (int)Position.X - Width / 2, (int)Position.Y - Height, Width, Height);
                    // Position is the item's bottom center so subtract half the width and full height to get top-left corner
            }
        }

        protected Item(Vector2 position, ISprite sprite, SpriteAnimation animation)
        {
            // sprite is required to display the item
            ArgumentNullException.ThrowIfNull(sprite);
            // animation is required to update the sprite
            ArgumentNullException.ThrowIfNull(animation);

            _sprite = sprite;
            _animation = animation;
            _startingPosition = position;
            // place the item at its starting position
            Position = position;

            // use first animation frame to calculate item's dimensions after applying the sprite scale
            // used displayed size = original size × scale
            Rectangle frame = animation.Frames[0].LeftSource;
            _width = (int)(frame.Width * ItemSpriteFactory.SpriteScale);
            _height = (int)(frame.Height * ItemSpriteFactory.SpriteScale);
            // set up sprite with its animation
            _sprite.Update(new GameTime(), _animation);
        }
        
        // update the sprite using the item's animation
        public virtual void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime, _animation);
        }
        // draws the sprite inside the item's bounds
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Bounds, SpriteEffects.None);
        }
        // returns item to its original position and resets its sprite.
        public virtual void Reset()
        {
            Position = _startingPosition;
            _sprite.Reset();
            // apply the item's animation again after resetting.
            _sprite.Update(new GameTime(), _animation);
        }
    }
}
