using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class Lakitu : Enemy
    {
        private const float MovementSpeed = 360f;
        private const float SideDistance = 288f;
        private const float ThrowIntervalSeconds = 3f;
        private const float CloudDurationSeconds = 0.75f;
        private const float CameraRange = 960f;
        private const float FlagRetreatDistance = 30f * 48f;
        private const float RetreatSpeed = 240f;
        public const int LakituWidth = 48;
        public const int LakituHeight = 96;

        private readonly SpriteAnimation _lakituAnimation;
        private readonly SpriteAnimation _cloudAnimation;
        private readonly List<Spiny> _spinySpawns = new List<Spiny>();
        private readonly float _startingY;
        private float _sideChangeElapsed;
        private float _throwElapsed;
        private float _cloudElapsed;
        private bool _isInCloud;
        private bool _isRetreating;
        private FacingDirection _side = FacingDirection.Left;

        public Vector2? ListenerPosition { get; set; }
        public Vector2? FlagPosition { get; set; }
        internal bool ShouldDespawn
        {
            get
            {
                return _isRetreating
                    && ListenerPosition.HasValue
                    && Position.X + Width < ListenerPosition.Value.X - CameraRange;
            }
        }

        public override int Width
        {
            get { return LakituWidth; }
        }

        public override int Height
        {
            get { return LakituHeight; }
        }

        public Lakitu(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _startingY = position.Y;
            _lakituAnimation = LakituSpriteFactory.CreateLakituAnimation();
            _cloudAnimation = LakituSpriteFactory.CreateCloudAnimation();
            Sprite.Update(new GameTime(), _lakituAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            if (IsDead)
            {
                Velocity = new Vector2(0f, Velocity.Y);
                Sprite.Update(gameTime, _lakituAnimation);
                return;
            }

            if (ListenerPosition.HasValue)
            {
                UpdatePosition(elapsedSeconds, ListenerPosition.Value);
                UpdateSpinyThrow(elapsedSeconds, ListenerPosition.Value);
            }

            StateMachine.Update(new Vector2(
                ListenerPosition.HasValue ? ListenerPosition.Value.X - Position.X : Velocity.X,
                0f));
            Sprite.Update(gameTime, _isInCloud ? _cloudAnimation : _lakituAnimation);
        }

        public override void UpdateGravity(GameTime gameTime)
        {
            if (IsDead)
            {
                base.UpdateGravity(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Bounds, StateMachine.FacingDirection);
        }

        internal List<Spiny> DrainSpinySpawns()
        {
            List<Spiny> result = new List<Spiny>(_spinySpawns);
            _spinySpawns.Clear();
            return result;
        }

        private void UpdatePosition(float elapsedSeconds, Vector2 listener)
        {
            if (!_isRetreating
                && FlagPosition.HasValue
                && FlagPosition.Value.X - listener.X <= FlagRetreatDistance)
            {
                _isRetreating = true;
                _isInCloud = false;
            }

            if (_isRetreating)
            {
                Velocity = new Vector2(-RetreatSpeed, 0f);
                Position = new Vector2(Position.X - RetreatSpeed * elapsedSeconds, _startingY);
                return;
            }

            _sideChangeElapsed += elapsedSeconds;
            if (_sideChangeElapsed >= 8f)
            {
                _sideChangeElapsed -= 8f;
                _side = _side == FacingDirection.Left
                    ? FacingDirection.Right
                    : FacingDirection.Left;
            }

            float desiredX = listener.X
                + (_side == FacingDirection.Right ? SideDistance : -SideDistance);
            float deltaX = desiredX - Position.X;
            float movement = Math.Min(Math.Abs(deltaX), MovementSpeed * elapsedSeconds);
            Velocity = new Vector2(Math.Sign(deltaX) * MovementSpeed, 0f);
            Position = new Vector2(
                Position.X + Math.Sign(deltaX) * movement,
                _startingY);
        }

        private void UpdateSpinyThrow(float elapsedSeconds, Vector2 listener)
        {
            if (_isRetreating)
            {
                return;
            }

            if (_isInCloud)
            {
                _cloudElapsed += elapsedSeconds;
                if (_cloudElapsed >= CloudDurationSeconds)
                {
                    float direction = StateMachine.FacingDirection == FacingDirection.Right ? 1f : -1f;
                    _spinySpawns.Add(EnemyFactory.CreateSpiny(
                        Position,
                        direction * Spiny.ThrowSpeed));
                    _isInCloud = false;
                }

                return;
            }

            if (Math.Abs(listener.X - Position.X) > CameraRange)
            {
                return;
            }

            _throwElapsed += elapsedSeconds;
            if (_throwElapsed >= ThrowIntervalSeconds)
            {
                _throwElapsed = 0f;
                _cloudElapsed = 0f;
                _isInCloud = true;
            }
        }
    }
}
