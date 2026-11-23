using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal enum CheepCheepMovementMode
    {
        Swimming,
        Flying
    }

    internal sealed class CheepCheep : Enemy
    {
        public const int CheepCheepWidth = 48;
        public const int CheepCheepHeight = 48;
        private const float SwimSpeed = 60f;
        private const float FlyingVerticalSpeed = -450f;
        private const float SecondsBetweenFlightChecks = 2.5f;
        private const int TileSize = 48;

        private readonly SpriteAnimation _swimAnimation;
        private readonly Vector2 _spawnPosition;
        private readonly CheepCheepMovementMode _movementMode;
        private float _flightTimer;

        public override int Width
        {
            get { return CheepCheepWidth; }
        }

        public override int Height
        {
            get { return CheepCheepHeight; }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + 4,
                    (int)Position.Y + 4,
                    CheepCheepWidth - 8,
                    CheepCheepHeight - 8);
            }
        }

        public CheepCheepMovementMode MovementMode
        {
            get { return _movementMode; }
        }

        public CheepCheep(
            ISprite sprite,
            Vector2 position,
            int sourceTileId,
            CheepCheepMovementMode movementMode = CheepCheepMovementMode.Swimming)
            : base(sprite, position)
        {
            if (movementMode != CheepCheepMovementMode.Swimming
                && movementMode != CheepCheepMovementMode.Flying)
            {
                throw new ArgumentOutOfRangeException(nameof(movementMode));
            }

            _movementMode = movementMode;
            if (_movementMode == CheepCheepMovementMode.Flying)
            {
                position.Y += TileSize;
                StateMachine.Update(Vector2.UnitX);
                _flightTimer = GetNextFlightDelay();
            }

            Position = position;
            _spawnPosition = position;
            _swimAnimation = CheepCheepSpriteFactory.CreateSwimAnimation(sourceTileId);
            if (_movementMode == CheepCheepMovementMode.Swimming)
            {
                Velocity = new Vector2(-SwimSpeed, 0f);
            }

            Sprite.Update(new GameTime(), _swimAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsDead)
            {
                float elapsedSeconds = Math.Min(
                    (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
                if (_movementMode == CheepCheepMovementMode.Swimming)
                {
                    Position += Velocity * elapsedSeconds;
                }
                else
                {
                    UpdateFlying(elapsedSeconds);
                }
            }

            StateMachine.Update(Velocity);
            Sprite.Update(gameTime, _swimAnimation);
        }

        public override void UpdateGravity(GameTime gameTime)
        {
            if (_movementMode == CheepCheepMovementMode.Flying && !IsDead)
            {
                if (Velocity.X != 0f)
                {
                    float elapsedSeconds = Math.Min(
                        (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
                    Position = new Vector2(
                        Position.X,
                        Position.Y + FlyingVerticalSpeed * elapsedSeconds);
                }
            }
            else if (IsDead)
            {
                base.UpdateGravity(gameTime);
            }
        }

        private void UpdateFlying(float elapsedSeconds)
        {
            _flightTimer -= elapsedSeconds;
            if (_flightTimer <= 0f)
            {
                Position = _spawnPosition;
                Velocity = new Vector2(
                    RandomNumberGenerator.GetInt32(2, 5) * 60f,
                    0f);
                _flightTimer = GetNextFlightDelay();
            }
            else if (Velocity.X != 0f)
            {
                Position = new Vector2(Position.X + Velocity.X * elapsedSeconds, Position.Y);
            }
        }

        private static float GetNextFlightDelay()
        {
            return SecondsBetweenFlightChecks
                + RandomNumberGenerator.GetInt32(500, 1501) / 1000f;
        }
    }
}
