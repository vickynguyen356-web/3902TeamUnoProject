using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal sealed class Blooper : Enemy
    {
        private const int BlooperWidth = 48;
        private const int BlooperHeight = 96;
        private const float ChargeSpeed = 540f;
        private const float ChargeDuration = 0.5f;
        private const float ChargeInterval = 1f;
        private const float PassiveFallAcceleration = 18f;
        private const float MaximumPassiveFallSpeed = 60f;
        private readonly SpriteAnimation _animation;
        private float _chargeTimer = ChargeInterval;
        private float _chargeTimeRemaining;
        private float _passiveFallSpeed;
        private bool _isCharging;

        public Vector2? ListenerPosition { get; set; }

        public override int Width
        {
            get { return BlooperWidth; }
        }

        public override int Height
        {
            get { return BlooperHeight; }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + 4,
                    (int)Position.Y + 8,
                    BlooperWidth - 8,
                    BlooperHeight - 16);
            }
        }

        protected override bool TurnsAtLedges
        {
            get { return false; }
        }

        public Blooper(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _animation = new SpriteAnimation(
                0.5f,
                EnemySpriteSheet.CreateEnemyIdFrame(48, 16, 32),
                EnemySpriteSheet.CreateEnemyIdFrame(49, 16, 32));
            Sprite.Update(new GameTime(), _animation);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = System.Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);

            if (!IsDead)
            {
                if (_isCharging)
                {
                    _chargeTimeRemaining -= elapsedSeconds;
                    Position += Velocity * elapsedSeconds;
                    if (_chargeTimeRemaining <= 0f)
                    {
                        _isCharging = false;
                        Velocity = Vector2.Zero;
                    }
                }
                else
                {
                    _chargeTimer -= elapsedSeconds;
                    if (_chargeTimer <= 0f && ListenerPosition.HasValue)
                    {
                        Vector2 target = ListenerPosition.Value;
                        float horizontalDirection = target.X < Bounds.Center.X ? -1f : 1f;
                        float verticalDirection = target.Y < Bounds.Center.Y ? -1f : 1f;
                        Velocity = new Vector2(
                            horizontalDirection * ChargeSpeed,
                            verticalDirection * ChargeSpeed);
                        _chargeTimeRemaining = ChargeDuration;
                        _chargeTimer = ChargeInterval;
                        _isCharging = true;
                        Position += Velocity * elapsedSeconds;
                    }
                }
            }

            StateMachine.Update(Velocity);
            Sprite.Update(gameTime, _animation);
        }

        public override void UpdateGravity(GameTime gameTime)
        {
            if (IsDead)
            {
                base.UpdateGravity(gameTime);
                return;
            }

            if (_isCharging)
            {
                return;
            }

            float elapsedSeconds = System.Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            _passiveFallSpeed = System.Math.Min(
                _passiveFallSpeed + PassiveFallAcceleration * elapsedSeconds,
                MaximumPassiveFallSpeed);
            Position += new Vector2(0f, _passiveFallSpeed * elapsedSeconds);
        }
    }
}
