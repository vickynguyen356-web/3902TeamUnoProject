using System;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal sealed class FireBarSpawnDefinition
    {
        public Vector2 Position { get; }
        public int StartingAngle { get; }
        public string RotationDirection { get; }
        public int Length { get; }

        public FireBarSpawnDefinition(
            Vector2 position,
            int startingAngle,
            string rotationDirection,
            int length)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rotationDirection);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
            Position = position;
            StartingAngle = startingAngle;
            RotationDirection = rotationDirection;
            Length = length;
        }
    }
}
