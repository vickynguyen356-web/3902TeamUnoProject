using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal readonly struct LevelTile
    {
        private const int SourceTileSize = 16;
        private const int SourceTileStride = 17;
        private const int SourceTileColumns = 48;
        private const int SourceTileRows = 22;
        private const int DestinationTileSize = 48;

        public Rectangle Bounds { get; }
        public Rectangle SourceRectangle { get; }
        public int TileId { get; }

        public LevelTile(int tileId, int x, int y)
        {
            if (tileId < 0 || tileId >= SourceTileColumns * SourceTileRows)
            {
                throw new ArgumentOutOfRangeException(nameof(tileId));
            }

            TileId = tileId;
            Bounds = new Rectangle(x, y, DestinationTileSize, DestinationTileSize);
            SourceRectangle = GetSourceRectangle(tileId);
        }

        public static Rectangle GetSourceRectangle(int tileId)
        {
            if (tileId < 0 || tileId >= SourceTileColumns * SourceTileRows)
            {
                throw new ArgumentOutOfRangeException(nameof(tileId));
            }

            return new Rectangle(
                1 + tileId % SourceTileColumns * SourceTileStride,
                1 + tileId / SourceTileColumns * SourceTileStride,
                SourceTileSize,
                SourceTileSize);
        }
    }
}
