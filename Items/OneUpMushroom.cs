using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Items
{
    public class OneUpMushroom : Item
    {
        private const float MoveSpeed = 60f;

        public override ItemType Type
        {
            get
            {
                return ItemType.OneUpMushroom;
            }
        }

        public OneUpMushroom(Vector2 position, ISprite sprite)
            : base(position, sprite, ItemSpriteFactory.CreateOneUpMushroomAnimation())
        {
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            Position = Position + new Vector2(MoveSpeed * seconds, 0);
            base.Update(gameTime);
        }
    }
}
