using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.World
{
    internal class BlockSpawnDefinition
    {
        private readonly Vector2 _position;
        private readonly BlockType _type;
        private readonly int _width;
        private readonly int _height;
        private readonly ItemType? _contents;
        private readonly string _destinationArea;
        private readonly string _destinationSpawn;
        private readonly Vector2? _destinationPosition;
        private readonly string _entryDirection;
        private readonly string _exitDirection;
        private readonly Vector2? _destinationCameraPosition;
        private readonly bool _freezeCamera;
        private readonly Color? _destinationBackgroundColor;
        private readonly bool? _destinationIsUnderwater;
        private readonly string _destinationMusicTrack;
        private readonly bool _isTopBrick;
        private readonly bool _isHorizontalPipeAttachment;
        private readonly bool _isUnderground;
        private readonly int? _tileId;
        private readonly int? _usedTileId;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public BlockType Type
        {
            get
            {
                return _type;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public ItemType? Contents
        {
            get
            {
                return _contents;
            }
        }

        public string DestinationArea
        {
            get
            {
                return _destinationArea;
            }
        }

        public string DestinationSpawn
        {
            get
            {
                return _destinationSpawn;
            }
        }

        public Vector2? DestinationPosition
        {
            get
            {
                return _destinationPosition;
            }
        }

        public string EntryDirection
        {
            get
            {
                return _entryDirection;
            }
        }

        public string ExitDirection
        {
            get
            {
                return _exitDirection;
            }
        }

        public Vector2? DestinationCameraPosition
        {
            get
            {
                return _destinationCameraPosition;
            }
        }

        public bool FreezeCamera
        {
            get
            {
                return _freezeCamera;
            }
        }

        public Color? DestinationBackgroundColor
        {
            get
            {
                return _destinationBackgroundColor;
            }
        }

        public bool? DestinationIsUnderwater
        {
            get
            {
                return _destinationIsUnderwater;
            }
        }

        public string DestinationMusicTrack
        {
            get
            {
                return _destinationMusicTrack;
            }
        }

        public bool IsTopBrick
        {
            get
            {
                return _isTopBrick;
            }
        }

        public bool IsHorizontalPipeAttachment
        {
            get
            {
                return _isHorizontalPipeAttachment;
            }
        }

        public bool IsUnderground
        {
            get
            {
                return _isUnderground;
            }
        }

        public int? TileId
        {
            get
            {
                return _tileId;
            }
        }

        public int? UsedTileId
        {
            get
            {
                return _usedTileId;
            }
        }

        public BlockSpawnDefinition(
            Vector2 position,
            BlockType type,
            int width = 48,
            int height = 48,
            ItemType? contents = null,
            string destinationArea = null,
            string destinationSpawn = null,
            Vector2? destinationPosition = null,
            string entryDirection = null,
            Vector2? destinationCameraPosition = null,
            bool freezeCamera = false,
            bool isTopBrick = false,
            bool isHorizontalPipeAttachment = false,
            string exitDirection = null,
            Color? destinationBackgroundColor = null,
            bool? destinationIsUnderwater = null,
            string destinationMusicTrack = null,
            bool isUnderground = false,
            int? tileId = null,
            int? usedTileId = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

            _position = position;
            _type = type;
            _width = width;
            _height = height;
            _contents = contents;
            _destinationArea = destinationArea;
            _destinationSpawn = destinationSpawn;
            _destinationPosition = destinationPosition;
            _entryDirection = entryDirection;
            _exitDirection = exitDirection;
            _destinationCameraPosition = destinationCameraPosition;
            _freezeCamera = freezeCamera;
            _destinationBackgroundColor = destinationBackgroundColor;
            _destinationIsUnderwater = destinationIsUnderwater;
            _destinationMusicTrack = destinationMusicTrack;
            _isTopBrick = isTopBrick;
            _isHorizontalPipeAttachment = isHorizontalPipeAttachment;
            _isUnderground = isUnderground;
            _tileId = tileId;
            _usedTileId = usedTileId;
        }
    }
}
