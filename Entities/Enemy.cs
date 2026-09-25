using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Entities
{
    public abstract class Enemy
    {
        protected readonly EnemyStateMachine StateMachine;
        protected readonly SpriteSheetSprite Sprite;
        protected Vector2 Velocity;
        protected Vector2 Position { get; protected set; }
        public abstract int Width { get; }
        public abstract int Height { get; }

        protected Enemy(Vector2 position, Texture2D texture, float scale)
        {
            Position = position;

            StateMachine = new EnemyStateMachine();

            Sprite = new SpriteSheetSprite(texture, scale);

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
