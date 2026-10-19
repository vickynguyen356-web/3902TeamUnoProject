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
        public const int KoopaWidth = 32;
        public const int KoopaHeight = 48;
        private const float WalkSpeed = 45f;
        private const float PatrolDistance = 150f;
        private readonly Func<Vector2, KoopaShell> _createShell;

        private readonly float _leftBound;
        private readonly float _rightBound;
        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _koopaAnimations;

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

        public Koopa(ISprite sprite, Vector2 position, Func<Vector2, KoopaShell> createShell)
            : base(sprite, position)
        {
            _koopaAnimations = KoopaSpriteFactory.CreateKoopaAnimations();
            _leftBound = position.X - PatrolDistance;
            _rightBound = position.X + PatrolDistance;
            Velocity.X = -WalkSpeed;
            _createShell = createShell;

            UpdateMovementAnimation(new GameTime(), _koopaAnimations);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsDead)
            {
                float movement = Velocity.X * elapsedSeconds;

                Position = Position + new Vector2(movement, 0f);

                if (Position.X <= _leftBound)
                {
                    Position = new Vector2(_leftBound, Position.Y);
                    Velocity.X = WalkSpeed;
                }
                else if (Position.X >= _rightBound)
                {
                    Position = new Vector2(_rightBound, Position.Y);
                    Velocity.X = -WalkSpeed;
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

            return _createShell(new Vector2(shellX, shellY));
        }
    }
}
