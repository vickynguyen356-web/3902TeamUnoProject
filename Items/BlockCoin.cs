using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class BlockCoin : Item
    {
        private const float PopSpeed = -300f;
        private const float Gravity = 1000f;
        private const float DisplayTime = 0.6f;
        private float verticalSpeed = PopSpeed;
        private float elapsedTime;

        public bool IsVisible { get; private set; } = true;

        public BlockCoin(Vector2 position, IItemSprite sprite)
            : base(ItemType.BlockCoin, position, sprite)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
            {
                return;
            }

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += seconds;
            if (elapsedTime >= DisplayTime)
            {
                IsVisible = false;
                return;
            }

            Position += new Vector2(0, verticalSpeed * seconds);
            verticalSpeed += Gravity * seconds;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                base.Draw(spriteBatch);
            }
        }

        public override void Reset()
        {
            base.Reset();
            verticalSpeed = PopSpeed;
            elapsedTime = 0;
            IsVisible = true;
        }
    }
}
