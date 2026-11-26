using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal static class LevelBounds
    {
        public static bool IsBelowLevel(Rectangle bounds, LevelDefinition definition)
        {
            return bounds.Top >= definition.Height;
        }

        public static bool IsStrictlyBelowLevel(Rectangle bounds, LevelDefinition definition)
        {
            return bounds.Top > definition.Height;
        }

        public static bool IsOutsideHorizontalBounds(Rectangle bounds, LevelDefinition definition)
        {
            return bounds.Right < 0 || bounds.Left > definition.Width;
        }

        public static bool IsOutsideLevel(Rectangle bounds, LevelDefinition definition)
        {
            return IsStrictlyBelowLevel(bounds, definition)
                || IsOutsideHorizontalBounds(bounds, definition);
        }
    }
}
