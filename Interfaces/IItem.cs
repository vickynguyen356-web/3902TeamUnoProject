using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.Interfaces
{
    internal interface IItem
    {
        ItemType Type
        {
            get;
        }

        Vector2 Position
        {
            get;
        }

        Vector2 CurrentVelocity
        {
            get;
        }

        Rectangle Bounds
        {
            get;
        }

        bool IsExpired
        {
            get;
        }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void ApplyMotion(Vector2 position, Vector2 velocity);
        void Expire();
    }
}
