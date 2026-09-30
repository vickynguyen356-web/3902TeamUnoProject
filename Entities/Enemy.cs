using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public abstract class Enemy : IEnemy
    {
        protected readonly EnemyStateMachine StateMachine;
        protected readonly ISprite Sprite;
        protected Vector2 Velocity;
        private Vector2 _position;

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

        public abstract int Width
        {
            get;
        }

        public abstract int Height
        {
            get;
        }

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

        public bool IsDead
        {
            get
            {
                return StateMachine.IsDead;
            }
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects facingDirection;
            if (StateMachine.IsFlipped)
            {
                facingDirection = SpriteEffects.FlipHorizontally;
            }
            else
            {
                facingDirection = SpriteEffects.None;
            }

            Sprite.Draw(
                spriteBatch,
                Bounds,
                facingDirection);
        }

        public virtual void TakeDamage()
        {
            StateMachine.TakeDamage();
        }

        public virtual void Kill()
        {
            StateMachine.Kill();
        }
    }
}
