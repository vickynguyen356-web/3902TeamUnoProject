using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class Koopa : Enemy
    {
        public const int KoopaWidth = 48;
        public const int KoopaHeight = 72;
        private const float WalkSpeed = 45f;
        private readonly Func<Vector2, KoopaShell> _createShell;
        private readonly bool _canFallOffLedges;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _koopaAnimations;
        private const float RemoveOffsetY = 100000f;

        public override int Width
        {
            get
            {
                return KoopaWidth;
            }
        }

        public override int Height
        {
            get
            {
                return KoopaHeight;
            }
        }

        protected override bool TurnsAtLedges
        {
            get
            {
                return !_canFallOffLedges;
            }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + 6,
                    (int)Position.Y + KoopaHeight / 2,
                    KoopaWidth - 12,
                    KoopaHeight / 2);
            }
        }

        public Koopa(
            ISprite sprite,
            Vector2 position,
            Func<Vector2, KoopaShell> createShell,
            bool canFallOffLedges = true)
            : base(sprite, position)
        {
            _koopaAnimations = KoopaSpriteFactory.CreateKoopaAnimations();
            Velocity.X = -WalkSpeed;
            _createShell = createShell;
            _canFallOffLedges = canFallOffLedges;

            UpdateMovementAnimation(new GameTime(), _koopaAnimations);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedSeconds = System.Math.Min(elapsedSeconds, 1f / 30f);

            if (!IsDead)
            {
                if (!TryWalk(elapsedSeconds))
                {
                    Velocity.X = -Velocity.X;
                }
            }

            UpdateMovementAnimation(gameTime, _koopaAnimations);
        }

        // when Mario stomps on Koopa, Koopa becomes KoopaShell
        public KoopaShell Stomp()
        {
            if (IsDead)
            {
                return null;
            }

            Kill();

            float shellX = Position.X + (Width - KoopaShell.ShellWidth) / 2f;
            float shellY = Position.Y + Height - KoopaShell.ShellHeight;

            Position = new Vector2(Position.X, RemoveOffsetY);

            return _createShell(new Vector2(shellX, shellY));
        }
    }
}
