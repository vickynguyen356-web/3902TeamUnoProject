using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.Interfaces
{
    internal interface IItem : ICollidable
    {
        ItemType Type
        {
            get;
        }

        Vector2 Position
        {
            get;
        }

        bool IsExpired
        {
            get;
        }

        Vector2 CurrentVelocity { get; }
        void ApplyMotion(Vector2 position, Vector2 velocity);
        void Expire();

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
