using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class PiranhaPlant : Enemy
    {
        public const int PiranhaWidth = 32;
        public const int PiranhaHeight = 48;
        private const float StateDuration = 1.0f;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _piranhaAnimations;
        private float _stateTimer;
        private PiranhaState _state;

        public override int Width
        {
            get
            {
                return PiranhaWidth;
            }
        }

        public override int Height
        {
            get
            {
                return PiranhaHeight;
            }
        }

        internal enum PiranhaState
        {
            Closed,
            Open
        }

        public PiranhaPlant(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _piranhaAnimations = PiranhaSpriteFactory.CreatePiranhaAnimations();

            _state = PiranhaState.Closed;
            _stateTimer = 0f;

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                _stateTimer = _stateTimer + elapsedSeconds;

                if (_stateTimer >= StateDuration)
                {
                    _stateTimer = 0f;

                    if (_state == PiranhaState.Closed)
                    {
                        _state = PiranhaState.Open;
                    }
                    else
                    {
                        _state = PiranhaState.Closed;
                    }
                }
            }

            UpdateAnimation(gameTime);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            EntityAnimationState animationState;

            if (_state == PiranhaState.Open)
            {
                animationState = EntityAnimationState.Open;
            }
            else
            {
                animationState = EntityAnimationState.Closed;
            }

            Sprite.Update(gameTime, _piranhaAnimations[animationState]);
        }
    }
}
