using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Enemies
{
    internal class Goomba : Enemy
    {
        public const int GoombaWidth = 48;
        public const int GoombaHeight = 48;
        private const float RunSpeed = 45f;

        private readonly IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> _goombaAnimations;

        public override int Width
        {
            get
            {
                return GoombaWidth;
            }
        }

        public override int Height
        {
            get
            {
                return GoombaHeight;
            }
        }

        protected override bool TurnsAtLedges
        {
            get
            {
                return false;
            }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + 6,
                    (int)Position.Y + 4,
                    GoombaWidth - 12,
                    GoombaHeight - 4);
            }
        }

        public Goomba(ISprite sprite, Vector2 position)
            : base(sprite, position)
        {
            _goombaAnimations = GoombaSpriteFactory.CreateGoombaAnimations();
            Velocity.X = -RunSpeed;

            UpdateMovementAnimation(new GameTime(), _goombaAnimations);
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

            UpdateMovementAnimation(gameTime, _goombaAnimations);
        }
    }
}
