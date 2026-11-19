using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal sealed class VineSpawnDefinition
    {
        public Vector2 Position { get; }
        public Vector2 DestinationPosition { get; }
        public Vector2 CameraPosition { get; }
        public int ResetThresholdY { get; }
        public Vector2 ResetPosition { get; }
        public int CameraMaxX { get; }
        public Color BackgroundColor { get; }
        public string LevelType { get; }
        public string MusicTrack { get; }

        public VineSpawnDefinition(
            Vector2 position,
            Vector2 destinationPosition,
            Vector2 cameraPosition,
            int resetThresholdY,
            Vector2 resetPosition,
            int cameraMaxX,
            Color backgroundColor,
            string levelType,
            string musicTrack)
        {
            Position = position;
            DestinationPosition = destinationPosition;
            CameraPosition = cameraPosition;
            ResetThresholdY = resetThresholdY;
            ResetPosition = resetPosition;
            CameraMaxX = cameraMaxX;
            BackgroundColor = backgroundColor;
            LevelType = levelType;
            MusicTrack = musicTrack;
        }
    }
}
