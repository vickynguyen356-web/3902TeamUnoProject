using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class Goomba : Enemy
    {
        private readonly Texture2D _texture;
        public const int Size = 40;
        public override int Width => 23;
        public override int Height => 24;
        private const float MovementSpeed = 13f;
        private Vector2 _velocity;
        private readonly EnemyStateMachine _stateMachine;
        private readonly SpriteAnimation _idleAnimation;
        private readonly SpriteAnimation _runAnimation;
        private readonly SpriteAnimation _deadAnimation;



        public Goomba(Vector2 position, Texture2D texture) : base(position, texture, 1.0f)
        {
            Velocity = new Vector2(-MovementSpeed, 0); // Goomba moves left by default

            _idleAnimation = new SpriteAnimation(0.2f, )
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                _stateMachine.Update(_velocity);
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += _velocity * deltaTime;

            _stateMachine.Update(_velocity);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = _stateMachine.IsFlipped
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            spriteBatch.Draw(
                _texture,
                Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                spriteEffects,
                0f);
        }
    }
}
