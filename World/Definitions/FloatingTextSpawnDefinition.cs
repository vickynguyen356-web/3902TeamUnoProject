using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal sealed class FloatingTextSpawnDefinition
    {
        public Vector2 Position { get; }
        public string Text { get; }

        public FloatingTextSpawnDefinition(Vector2 position, string text)
        {
            Position = position;
            Text = text;
        }
    }
}
