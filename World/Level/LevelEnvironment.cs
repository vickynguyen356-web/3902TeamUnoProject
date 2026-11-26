using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal static class LevelEnvironment
    {
        private const int LavaTileId = 634;
        private const int LavaTileIdAlternate = 682;

        public static bool IsLavaAt(
            Rectangle bounds,
            LevelDefinition definition)
        {
            return ContainsLava(bounds, definition.BackgroundTiles)
                || ContainsLava(bounds, definition.ForegroundTiles)
                || ContainsLava(bounds, definition.AboveForegroundTiles);
        }

        private static bool ContainsLava(
            Rectangle bounds,
            IReadOnlyList<LevelTile> tiles)
        {
            foreach (LevelTile tile in tiles)
            {
                if ((tile.TileId == LavaTileId
                        || tile.TileId == LavaTileIdAlternate)
                    && bounds.Intersects(tile.Bounds))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
