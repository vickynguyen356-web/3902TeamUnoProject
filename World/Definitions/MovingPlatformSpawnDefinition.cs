using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal sealed class MovingPlatformSpawnDefinition
    {
        public Vector2 Position { get; }
        public string MotionType { get; }
        public string Direction { get; }
        public int MinimumPoint { get; }
        public int MaximumPoint { get; }
        public int WidthInTiles { get; }
        public bool ShiftByHalfPixel { get; }
        public bool IsUnderground { get; }
        public int SourceTileId { get; }

        public MovingPlatformSpawnDefinition(
            Vector2 position,
            string motionType,
            string direction,
            int minimumPoint,
            int maximumPoint,
            int widthInTiles,
            bool shiftByHalfPixel,
            bool isUnderground = false,
            int sourceTileId = -1)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(motionType);
            ArgumentException.ThrowIfNullOrWhiteSpace(direction);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(minimumPoint, maximumPoint);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(widthInTiles);
            Position = position;
            MotionType = motionType;
            Direction = direction;
            MinimumPoint = minimumPoint;
            MaximumPoint = maximumPoint;
            WidthInTiles = widthInTiles;
            ShiftByHalfPixel = shiftByHalfPixel;
            IsUnderground = isUnderground;
            SourceTileId = sourceTileId;
        }
    }
}
