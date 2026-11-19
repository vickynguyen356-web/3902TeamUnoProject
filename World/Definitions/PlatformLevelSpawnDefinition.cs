using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal sealed class PlatformLevelSpawnDefinition
    {
        public Vector2 LeftPlatform { get; }
        public Vector2 RightPlatform { get; }
        public int PulleyY { get; }

        public PlatformLevelSpawnDefinition(Vector2 leftPlatform, Vector2 rightPlatform, int pulleyY)
        {
            LeftPlatform = leftPlatform;
            RightPlatform = rightPlatform;
            PulleyY = pulleyY;
        }
    }
}
