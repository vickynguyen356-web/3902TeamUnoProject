using Microsoft.Xna.Framework;
using Sprint0.Interfaces;

namespace Sprint0.Items
{
    public class Mushroom : Item
    {
        private const float MoveSpeed = 60f;

        public Mushroom(Vector2 position, IItemSprite sprite)
            : this(ItemType.Mushroom, position, sprite)
        {
        }

        protected Mushroom(ItemType type, Vector2 position, IItemSprite sprite)
            : base(type, position, sprite)
        {
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += new Vector2(MoveSpeed * seconds, 0);
            base.Update(gameTime);
        }
    }
}
