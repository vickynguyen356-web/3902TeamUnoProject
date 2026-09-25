using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public abstract class Enemy
    {
        protected readonly EnemyStateMachine StateMachine;
        protected readonly ISprite Sprite;
        protected Vector2 Velocity;
        protected Vector2 Position { get; set; }
        public abstract int Width { get; }
        public abstract int Height { get; }

        protected Enemy(ISprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;

            StateMachine = new EnemyStateMachine();

        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Width,
                    Height);
            }
        }

        public bool IsDead => StateMachine.IsDead;

        public abstract void Update(GameTime gameTime);
        // any enemy classes can override this method to implement specific sprite drawing behavior
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects facingDirection =
                StateMachine.IsFlipped
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            Sprite.Draw(
                spriteBatch,
                Bounds,
                facingDirection);
        }
        // any enemy classes can override this method to implement damage taking behavior
        public virtual void TakeDamage()
        {
            StateMachine.TakeDamage();
        }
    }
}
