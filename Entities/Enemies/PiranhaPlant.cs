using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class PiranhaPlant : Enemy
    {
        public const int PiranhaWidth = 48;
        public const int PiranhaHeight = 96;
        public const int ReferenceTileId = 44;
        private const float StateDuration = 3.0f;
        private const float VerticalSpeed = 180f;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _piranhaAnimations;
        private readonly float _extendedY;
        private readonly float _pipeTopY;
        private float _stateTimer;
        private PiranhaState _state;

        internal Rectangle? PlayerBounds { get; set; }

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

        public override Rectangle CollisionBounds
        {
            get
            {
                if (_state == PiranhaState.Hidden)
                {
                    return Rectangle.Empty;
                }

                return new Rectangle(
                    (int)Position.X + PiranhaWidth / 4,
                    (int)Position.Y + PiranhaHeight / 6,
                    PiranhaWidth / 2,
                    PiranhaHeight * 2 / 3);
            }
        }

        internal enum PiranhaState
        {
            Extended,
            Retracting,
            Hidden,
            Extending
        }

        public PiranhaPlant(ISprite sprite, Vector2 position, int sourceTileId = ReferenceTileId)
            : base(sprite, position)
        {
            _piranhaAnimations = PiranhaSpriteFactory.CreatePiranhaAnimations(sourceTileId);
            _extendedY = position.Y;
            _pipeTopY = position.Y + PiranhaHeight;
            _state = PiranhaState.Hidden;
            _stateTimer = 0f;
            Position = new Vector2(position.X, _pipeTopY);

            UpdateAnimation(new GameTime());
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                if (IsPlayerTouchingPipe() && _state == PiranhaState.Hidden)
                {
                    _stateTimer = 0f;
                    UpdateAnimation(gameTime);
                    return;
                }

                if (_state == PiranhaState.Extended || _state == PiranhaState.Hidden)
                {
                    _stateTimer += elapsedSeconds;
                    if (_stateTimer >= StateDuration)
                    {
                        _stateTimer = 0f;
                        _state = _state == PiranhaState.Extended
                            ? PiranhaState.Retracting
                            : PiranhaState.Extending;
                    }
                }
                else if (_state == PiranhaState.Retracting)
                {
                    Position = new Vector2(
                        Position.X, Math.Min(_pipeTopY, Position.Y + VerticalSpeed * elapsedSeconds));
                    if (Position.Y >= _pipeTopY)
                    {
                        _state = PiranhaState.Hidden;
                        _stateTimer = 0f;
                    }
                }
                else
                {
                    Position = new Vector2(
                        Position.X, Math.Max(_extendedY, Position.Y - VerticalSpeed * elapsedSeconds));
                    if (Position.Y <= _extendedY)
                    {
                        _state = PiranhaState.Extended;
                        _stateTimer = 0f;
                    }
                }
            }

            UpdateAnimation(gameTime);
        }

        private bool IsPlayerTouchingPipe()
        {
            if (!PlayerBounds.HasValue)
            {
                return false;
            }

            Rectangle pipeBounds = new Rectangle(
                (int)Position.X - PiranhaWidth / 2,
                (int)_pipeTopY,
                PiranhaWidth * 2,
                PiranhaHeight);
            Rectangle playerBounds = PlayerBounds.Value;
            return playerBounds.Right >= pipeBounds.Left
                && playerBounds.Left <= pipeBounds.Right
                && playerBounds.Bottom >= pipeBounds.Top
                && playerBounds.Top <= pipeBounds.Bottom;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_state != PiranhaState.Hidden)
            {
                base.Draw(spriteBatch);
            }
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            EntityAnimationState animationState = _state == PiranhaState.Extended
                ? EntityAnimationState.Open
                : EntityAnimationState.Closed;

            Sprite.Update(gameTime, _piranhaAnimations[animationState]);
        }
    }
}
