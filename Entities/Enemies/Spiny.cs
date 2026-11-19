using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class Spiny : Enemy
    {
        public const float ThrowSpeed = 150f;
        public const int SpinyWidth = 48;
        public const int SpinyHeight = 48;

        private readonly SpriteAnimation _eggAnimation;
        private readonly SpriteAnimation _walkingAnimation;
        private bool _hasFallen;
        private bool _isWalking;

        public override int Width
        {
            get { return SpinyWidth; }
        }

        public override int Height
        {
            get { return SpinyHeight; }
        }

        public Spiny(ISprite sprite, Vector2 position, float horizontalVelocity)
            : base(sprite, position)
        {
            _eggAnimation = SpinySpriteFactory.CreateEggAnimation();
            _walkingAnimation = SpinySpriteFactory.CreateWalkingAnimation();
            Velocity = new Vector2(horizontalVelocity, 0f);
            Sprite.Update(new GameTime(), _eggAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                Velocity = new Vector2(0f, Velocity.Y);
                Sprite.Update(gameTime, _walkingAnimation);
                return;
            }

            float elapsedSeconds = Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            if (Velocity.Y > 0f)
            {
                _hasFallen = true;
            }
            else if (_hasFallen && Velocity.Y == 0f)
            {
                _isWalking = true;
            }

            if (!TryWalk(elapsedSeconds))
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            }

            StateMachine.Update(Velocity);
            Sprite.Update(gameTime, _isWalking ? _walkingAnimation : _eggAnimation);
        }
    }
}
