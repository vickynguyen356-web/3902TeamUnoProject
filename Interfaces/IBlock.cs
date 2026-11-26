using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.World;

namespace TeamUno.Mario.Interfaces
{
    internal interface IBlock : ICollidable
    {
        BlockType Type { get; }
        Vector2 Position { get; }
        int Width { get; }
        int Height { get; }
        bool IsDeleted { get; }
        bool IsUnderground { get; }
        bool IsRemovalReady { get; }
        Rectangle PreviousBounds { get; }
        Vector2 MovementDelta { get; }
        int? TileId { get; }
        string DestinationArea { get; }
        string DestinationSpawn { get; }
        Vector2? DestinationPosition { get; }
        string EntryDirection { get; }
        string ExitDirection { get; }
        Vector2? DestinationCameraPosition { get; }
        bool FreezeCamera { get; }
        Color? DestinationBackgroundColor { get; }
        bool? DestinationIsUnderwater { get; }
        string DestinationMusicTrack { get; }
        bool IsTravelRequested { get; }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, Texture2D blockTexture, Texture2D tileSheetTexture = null);
        void ActivateMovingPlatform();
        bool ConsumePlatformLevelContact();
        void MoveTo(Vector2 position);
        void BeginFlagpoleDescent(float poleBottom);
        void UpdateFlagpoleDescent(float elapsedSeconds);
        void RequestTravel();
        void ClearTravelRequest();
        void MarkDeleted();
        bool RequestVineTravel(Rectangle playerBounds, bool isClimbing);
        VineSpawnDefinition ConsumeVineTravelRequest();
        ItemType? CollectItem();
        void HitFromBelow(bool canBreak, bool isSmall = false);
    }
}
