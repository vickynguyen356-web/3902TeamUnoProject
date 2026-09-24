using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Entities
{
    public class Goomba
    {
        public const int Size = 40;
        public Vector2 Position { get; private set; }

        public Goomba(Vector2 position)
        {
            Position = position;
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        // Goomba doesn't have a small state.
        public bool IsSmall
        {
            get
            {
                return false;
            }
        }

        // Goomba doesn't have different forms.
        public PlayerForm Form
        {
            get
            {
                return PlayerForm.Small;
            }
        }

        public EntityAnimationState AnimationState
        {
            get
            {
                // TODO
                return ;
            }
        }

        public void Update(GameTime gameTime)
        {
            // Enemy movement and state changes go here.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Enemy drawing goes here.
        }
    }
}
