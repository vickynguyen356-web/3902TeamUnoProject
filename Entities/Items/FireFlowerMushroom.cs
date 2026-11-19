using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Items
{
    internal class FireFlowerMushroom : Item
    {
        private const float MoveSpeed = 60f;
        private const float Gravity = 1500f;

        public override ItemType Type
        {
            get
            {
                return ItemType.FireFlowerMushroom;
            }
        }

        public FireFlowerMushroom(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateMushroomAnimation())
        {
            Velocity = new Vector2(MoveSpeed, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = System.Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds,
                MaximumElapsedSeconds);
            Velocity.Y = Velocity.Y + Gravity * elapsedSeconds;
            Position = Position + Velocity * elapsedSeconds;
            base.Update(gameTime);
        }
    }
}
