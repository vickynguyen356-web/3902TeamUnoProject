using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;
using System.Collections.Generic;

namespace Sprint0.Entities
{
    public class Goomba : Enemy
    {
        /* goomba fields */
        public const int GoombaWidth = 23;
        public const int GoombaHeight = 24;

        // references available after creating Goomba
        public override int Width => GoombaWidth;
        public override int Height => GoombaHeight;
        private const float RunSpeed = 13f;
        /* animation related fields */
        private readonly Texture2D _texture;
        private readonly ISprite _sprite;
        private readonly EnemyStateMachine _stateMachine;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _goombaAnimations;
        private readonly Vector2 _startingPos;

        public Goomba(ISprite sprite, Vector2 position) 
            : base(sprite, position)
        {
            _goombaAnimations = GoombaSpriteFactory.CreateGoombaAnimations();
            _startingPos = position;
            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                Velocity.X = RunSpeed;
                Position += Velocity * elapsedSeconds;
            }

            UpdateAnimation(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            StateMachine.Update(Velocity);

            Sprite.Update(gameTime, GetCurrentAnimation());
        }

        private SpriteAnimation GetCurrentAnimation()
        {
            if (_goombaAnimations.TryGetValue(StateMachine.AnimationState, out SpriteAnimation animation))
            {
                return animation;
            }

            return _goombaAnimations[EntityAnimationState.Idle];
        }

        public void Reset()
        {
            Position = _startingPos;
            Velocity = Vector2.Zero;

            StateMachine.Reset();
            Sprite.Reset();

            UpdateAnimation(new GameTime());
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    SpriteEffects spriteEffects = _stateMachine.IsFlipped
        //        ? SpriteEffects.FlipHorizontally
        //        : SpriteEffects.None;

        //    spriteBatch.Draw(
        //        _texture,
        //        Position,
        //        null,
        //        Color.White,
        //        0f,
        //        Vector2.Zero,
        //        1f,
        //        spriteEffects,
        //        0f);
        //}
    }
}
