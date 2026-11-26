using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Audio;
using TeamUno.Mario.World;

namespace TeamUno.Mario.Entities.Blocks
{
    internal enum BlockType
    {
        Brick,
        Question,
        Used,
        Ground,
        Pipe,
        Solid,
        Invisible,
        FlagPole,
        Flag,
        Empty,
        Platform,
        Vine,
        Tile,
        Spring,
        Axe,
        Bridge
    }

    internal class Block : IBlock
    {
        private const int DefaultWidth = 48;
        private const int DefaultHeight = 48;
        private const float VineGrowthIntervalSeconds = 0.3f;
        private const float BreakAnimationDurationSeconds = 0.55f;
        private const float DebrisGravity = 720f;
        private const float FlagMovementSpeed = 180f;
        private const int FlagpoleBaseSize = 48;
        private static readonly Rectangle FlagpoleSource = new Rectangle(0, 600, 16, 176);
        private static readonly Rectangle FlagpoleBaseSource = new Rectangle(16, 600, 16, 16);
        private static readonly Rectangle PipeTopSource = new Rectangle(112, 624, 32, 31);
        private static readonly Rectangle PipeBodySource = new Rectangle(112, 656, 32, 32);
        private static readonly Rectangle HorizontalPipeBodySource = new Rectangle(192, 656, 33, 32);
        private static readonly Rectangle HorizontalPipeEndSource = new Rectangle(224, 624, 32, 64);
        private static readonly Rectangle AttachedPipeTopSource = new Rectangle(224, 624, 32, 32);
        private static readonly Rectangle AttachedPipeBodySource = new Rectangle(224, 656, 32, 32);

        private BlockSprite _sprite;
        private BlockType _type;
        private bool _isDeleted;
        private bool _isBreaking;
        private float _breakAnimationElapsed;
        private readonly Vector2[] _debrisPositions = new Vector2[4];
        private readonly Vector2[] _debrisVelocities =
        {
            new Vector2(-150f, -260f),
            new Vector2(150f, -320f),
            new Vector2(-120f, -100f),
            new Vector2(120f, -160f)
        };
        private Vector2 _position;
        private Vector2 _previousPosition;
        private readonly int _width;
        private int _height;
        private readonly MovingPlatformSpawnDefinition _movingPlatform;
        private readonly VineSpawnDefinition _vineSpawn;
        private readonly int? _tileId;
        private readonly int? _usedTileId;
        private readonly bool _isPlatformLevel;
        private bool _platformLevelContact;
        private bool _isFlagMoving;
        private float _flagTargetY;
        private string _movingPlatformDirection;
        private bool _gravityPlatformActivated;
        private float _gravityPlatformVelocity;
        private float _vineGrowthElapsed;
        private int _targetVineHeight;
        private bool _isVineGrowing;
        private bool _vineTravelRequested;
        private readonly ItemType? _contents;
        private ItemType? _pendingItem;
        private bool _isTravelRequested;
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
        private readonly bool _isUnderground;
        private readonly bool _isUnderwater;
        private readonly bool _isTopBrick;
        private readonly bool _isHorizontalPipeAttachment;

        public BlockType Type
        {
            get
            {
                return _type;
            }
            private set
            {
                _type = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
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

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public Rectangle PreviousBounds
        {
            get
            {
                return new Rectangle(
                    (int)_previousPosition.X,
                    (int)_previousPosition.Y,
                    Width,
                    Height);
            }
        }

        public Vector2 MovementDelta
        {
            get
            {
                return Position - _previousPosition;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
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

        public bool IsRemovalReady
        {
            get
            {
                return _isDeleted && (!_isBreaking
                    || _breakAnimationElapsed >= BreakAnimationDurationSeconds);
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

        public bool IsTravelRequested
        {
            get
            {
                return _isTravelRequested;
            }
            private set
            {
                _isTravelRequested = value;
            }
        }

        public Block(
            Vector2 position,
            BlockType type,
            int width = DefaultWidth,
            int height = DefaultHeight,
            ItemType? contents = null,
            string destinationArea = null,
            string destinationSpawn = null,
            Vector2? destinationPosition = null,
            string entryDirection = null,
            Vector2? destinationCameraPosition = null,
            bool freezeCamera = false,
            bool isUnderground = false,
            bool isTopBrick = false,
            bool isHorizontalPipeAttachment = false,
            string exitDirection = null,
            Color? destinationBackgroundColor = null,
            bool? destinationIsUnderwater = null,
            string destinationMusicTrack = null,
            MovingPlatformSpawnDefinition movingPlatform = null,
            bool isPlatformLevel = false,
            VineSpawnDefinition vineSpawn = null,
            int? tileId = null,
            int? usedTileId = null,
            bool isUnderwater = false)
        {
            _position = movingPlatform != null && movingPlatform.ShiftByHalfPixel
                ? new Vector2(position.X + 0.5f, position.Y)
                : position;
            _previousPosition = _position;
            Type = type;
            _width = width;
            _height = height;
            _movingPlatform = movingPlatform;
            _vineSpawn = vineSpawn;
            _tileId = tileId;
            _usedTileId = usedTileId;
            _isPlatformLevel = isPlatformLevel;
            _movingPlatformDirection = movingPlatform == null ? null : movingPlatform.Direction;
            _contents = contents;
            if (_contents == null && type == BlockType.Question)
            {
                _contents = ItemType.BlockCoin;
            }

            if (type == BlockType.Vine && vineSpawn != null)
            {
                _targetVineHeight = Math.Max(
                    height,
                    (int)(position.Y + height - vineSpawn.DestinationPosition.Y));
                _isVineGrowing = _targetVineHeight > _height;
            }

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
            _isUnderground = isUnderground;
            _isUnderwater = isUnderwater;
            _isTopBrick = isTopBrick;
            _isHorizontalPipeAttachment = isHorizontalPipeAttachment;
            _sprite = BlockSpriteFactory.Create(type, isUnderground, isTopBrick, isUnderwater);
        }

        public void Update(GameTime gameTime)
        {
            _previousPosition = _position;
            if (_isBreaking)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float remainingSeconds = BreakAnimationDurationSeconds - _breakAnimationElapsed;
                float animationStep = Math.Min(Math.Max(0, elapsedSeconds), remainingSeconds);
                _breakAnimationElapsed += animationStep;
                for (int index = 0; index < _debrisPositions.Length; index++)
                {
                    _debrisPositions[index] += _debrisVelocities[index] * animationStep;
                    _debrisVelocities[index] += new Vector2(0, DebrisGravity * animationStep);
                }

                return;
            }

            _sprite.Update(gameTime);
            UpdateMovingPlatform(gameTime);
            UpdateVineGrowth(gameTime);
        }

        private void UpdateVineGrowth(GameTime gameTime)
        {
            if (!_isVineGrowing)
            {
                return;
            }

            _vineGrowthElapsed += Math.Max(0f, (float)gameTime.ElapsedGameTime.TotalSeconds);
            while (_vineGrowthElapsed >= VineGrowthIntervalSeconds && _height < _targetVineHeight)
            {
                _vineGrowthElapsed -= VineGrowthIntervalSeconds;
                int growth = Math.Min(DefaultHeight, _targetVineHeight - _height);
                _height += growth;
                _position.Y -= growth;
            }

            _isVineGrowing = _height < _targetVineHeight;
        }

        private void UpdateMovingPlatform(GameTime gameTime)
        {
            if (_movingPlatform == null)
            {
                return;
            }

            float elapsedSeconds = Math.Max(0f, (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (string.Equals(
                _movingPlatform.MotionType,
                "GRAVITY",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_gravityPlatformActivated && elapsedSeconds > 0f)
                {
                    _gravityPlatformVelocity += 540f * elapsedSeconds;
                    _position.Y += _gravityPlatformVelocity * elapsedSeconds;
                }

                return;
            }

            bool isHorizontal = string.Equals(_movingPlatformDirection, "LEFT", StringComparison.OrdinalIgnoreCase)
                || string.Equals(_movingPlatformDirection, "RIGHT", StringComparison.OrdinalIgnoreCase);
            bool isPositiveDirection = string.Equals(
                _movingPlatformDirection,
                "RIGHT",
                StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    _movingPlatformDirection,
                    "DOWN",
                    StringComparison.OrdinalIgnoreCase);
            bool isNegativeDirection = string.Equals(
                _movingPlatformDirection,
                "LEFT",
                StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    _movingPlatformDirection,
                    "UP",
                    StringComparison.OrdinalIgnoreCase);
            if (!isPositiveDirection && !isNegativeDirection)
            {
                return;
            }

            if (elapsedSeconds == 0f)
            {
                return;
            }

            float minimum = _movingPlatform.MinimumPoint * DefaultWidth;
            float maximum = (_movingPlatform.MaximumPoint + 1) * DefaultWidth;
            float position = isHorizontal ? Position.X : Position.Y;
            float extent = isHorizontal ? Width : Height;
            bool isBackAndForth = string.Equals(
                _movingPlatform.MotionType,
                "BACK_AND_FORTH",
                StringComparison.OrdinalIgnoreCase);
            bool isRepeated = string.Equals(
                _movingPlatform.MotionType,
                "ONE_DIRECTION_REPEATED",
                StringComparison.OrdinalIgnoreCase);
            if (!isBackAndForth && !isRepeated
                && !string.Equals(
                    _movingPlatform.MotionType,
                    "ONE_DIRECTION_CONTINUOUS",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (isBackAndForth)
            {
                bool atLimit = isPositiveDirection
                    ? position + extent >= maximum
                    : position <= minimum;
                if (atLimit)
                {
                    _movingPlatformDirection = isHorizontal
                        ? (isPositiveDirection ? "LEFT" : "RIGHT")
                        : (isPositiveDirection ? "UP" : "DOWN");
                    isPositiveDirection = !isPositiveDirection;
                }
            }

            float direction = isPositiveDirection ? 1f : -1f;
            float nextPosition = position + direction * 180f * elapsedSeconds;
            if (isRepeated)
            {
                if (isPositiveDirection && nextPosition > maximum)
                {
                    nextPosition = minimum;
                }
                else if (!isPositiveDirection && nextPosition < minimum)
                {
                    nextPosition = maximum;
                }
            }

            _position = isHorizontal
                ? new Vector2(nextPosition, Position.Y)
                : new Vector2(Position.X, nextPosition);
        }

        public void ActivateMovingPlatform()
        {
            if (_isPlatformLevel)
            {
                _platformLevelContact = true;
            }

            if (_movingPlatform != null
                && string.Equals(
                    _movingPlatform.MotionType,
                    "GRAVITY",
                    StringComparison.OrdinalIgnoreCase))
            {
                _gravityPlatformActivated = true;
            }
        }

        public bool ConsumePlatformLevelContact()
        {
            bool wasContacted = _platformLevelContact;
            _platformLevelContact = false;
            return wasContacted;
        }

        public void MoveTo(Vector2 position)
        {
            _position = position;
        }

        public void BeginFlagpoleDescent(float poleBottom)
        {
            _flagTargetY = poleBottom - Height;
            _isFlagMoving = Position.Y < _flagTargetY;
            if (!_isFlagMoving)
            {
                _position = new Vector2(Position.X, _flagTargetY);
            }
        }

        public void UpdateFlagpoleDescent(float elapsedSeconds)
        {
            if (!_isFlagMoving)
            {
                return;
            }

            float nextY = Position.Y + FlagMovementSpeed * Math.Max(0f, elapsedSeconds);
            if (nextY >= _flagTargetY)
            {
                nextY = _flagTargetY;
                _isFlagMoving = false;
            }

            _position = new Vector2(Position.X, nextY);
        }

        public void RequestTravel()
        {
            IsTravelRequested = true;
        }

        public void ClearTravelRequest()
        {
            IsTravelRequested = false;
        }

        public void MarkDeleted()
        {
            _isDeleted = true;
        }

        public bool RequestVineTravel(Rectangle playerBounds, bool isClimbing)
        {
            if (_vineSpawn == null || _isVineGrowing || _vineTravelRequested
                || !isClimbing || playerBounds.Top > Bounds.Top)
            {
                return false;
            }

            _vineTravelRequested = true;
            return true;
        }

        public VineSpawnDefinition ConsumeVineTravelRequest()
        {
            if (!_vineTravelRequested)
            {
                return null;
            }

            _vineTravelRequested = false;
            return _vineSpawn;
        }

        public ItemType? CollectItem()
        {
            ItemType? item = _pendingItem;
            _pendingItem = null;
            return item;
        }

        private ItemType? ResolveItemContents(bool isSmall)
        {
            if (_contents == ItemType.FireFlowerMushroom)
            {
                return isSmall ? ItemType.Mushroom : ItemType.FireFlower;
            }

            return _contents;
        }

        public void HitFromBelow(bool canBreak, bool isSmall = false)
        {
            if (Type == BlockType.Question || Type == BlockType.Invisible)
            {
                if (_contents == ItemType.BlockCoin)
                {
                    SoundPlayer.Play("Audio/SoundEffects/coin");
                }
                else if (_contents.HasValue)
                {
                    SoundPlayer.Play("Audio/SoundEffects/item");
                }
                else
                {
                    SoundPlayer.Play("Audio/SoundEffects/bump");
                }

                Type = BlockType.Used;
                _sprite = BlockSpriteFactory.Create(
                    Type,
                    isUnderground: _isUnderground,
                    isTopBrick: _isTopBrick,
                    isUnderwater: _isUnderwater);
                _pendingItem = ResolveItemContents(isSmall);
            }
            else if (Type == BlockType.Brick && _contents.HasValue)
            {
                SoundPlayer.Play(_contents == ItemType.BlockCoin
                    ? "Audio/SoundEffects/coin"
                    : "Audio/SoundEffects/item");
                Type = BlockType.Used;
                _sprite = BlockSpriteFactory.Create(
                    Type,
                    isUnderground: _isUnderground,
                    isTopBrick: _isTopBrick,
                    isUnderwater: _isUnderwater);
                _pendingItem = ResolveItemContents(isSmall);
            }
            else if (Type == BlockType.Brick && canBreak)
            {
                SoundPlayer.Play("Audio/SoundEffects/brick");
                _pendingItem = _contents;
                _isDeleted = true;
                _isBreaking = true;
                int fragmentWidth = Width / 2;
                int fragmentHeight = Height / 2;
                for (int index = 0; index < _debrisPositions.Length; index++)
                {
                    int column = index % 2;
                    int row = index / 2;
                    _debrisPositions[index] = new Vector2(
                        Position.X + column * fragmentWidth,
                        Position.Y + row * fragmentHeight);
                }
            }
            else if (Type == BlockType.Brick || Type == BlockType.Used || Type == BlockType.Solid)
            {
                // small Mario on a brick or empty/solid block
                SoundPlayer.Play("Audio/SoundEffects/bump");
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D blockTexture, Texture2D tileSheetTexture = null)
        {
            if (blockTexture == null)
            {
                return;
            }

            if (_isBreaking)
            {
                DrawBrickDebris(spriteBatch, blockTexture, tileSheetTexture);
                return;
            }

            if (Type == BlockType.Vine)
            {
                DrawVine(spriteBatch, tileSheetTexture ?? blockTexture);
                return;
            }

            if (Type == BlockType.Tile)
            {
                return;
            }

            if (Type == BlockType.FlagPole)
            {
                return;
            }

            if (Type == BlockType.Flag || Type == BlockType.Pipe)
            {
                return;
            }

            if (Type == BlockType.Platform)
            {
                if (!TryDrawReferencePlatform(spriteBatch, tileSheetTexture))
                {
                    DrawPlatform(spriteBatch, blockTexture);
                }

                return;
            }

            if (Type == BlockType.Spring)
            {
                DrawSpring(spriteBatch, tileSheetTexture);
                return;
            }

            if (TryDrawReferenceTile(spriteBatch, tileSheetTexture))
            {
                return;
            }

            if (Type == BlockType.FlagPole)
            {
                // keep the base one tile tall instead of stretching it with the pole
                Rectangle pole = new Rectangle(Bounds.X, Bounds.Y, Width, Math.Max(0, Height - FlagpoleBaseSize));
                Rectangle poleBase = new Rectangle(
                    Bounds.Center.X - FlagpoleBaseSize / 2, Bounds.Bottom - FlagpoleBaseSize, FlagpoleBaseSize, FlagpoleBaseSize);
                spriteBatch.Draw(blockTexture, pole, FlagpoleSource, Color.White);
                spriteBatch.Draw(blockTexture, poleBase, FlagpoleBaseSource, Color.White);
                return;
            }

            Rectangle sourceRectangle = _sprite.GetSourceRectangle();
            if (sourceRectangle == Rectangle.Empty)
            {
                return;
            }

            spriteBatch.Draw(blockTexture, Bounds, sourceRectangle, Color.White);
        }

        private void DrawSpring(SpriteBatch spriteBatch, Texture2D tileSheetTexture)
        {
            if (tileSheetTexture == null || !_tileId.HasValue)
            {
                return;
            }

            const int SourceTileColumns = 48;
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    int tileId = _tileId.Value + row * SourceTileColumns + column;
                    Rectangle destination = new Rectangle(
                        Bounds.Left + column * DefaultWidth,
                        Bounds.Top + row * DefaultHeight,
                        DefaultWidth,
                        DefaultHeight);
                    spriteBatch.Draw(
                        tileSheetTexture,
                        destination,
                        LevelTile.GetSourceRectangle(tileId),
                        Color.White);
                }
            }
        }

        private void DrawVerticalPipe(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            const int SourceSegmentHeight = DefaultHeight * 2;
            int destinationY = Bounds.Top;
            bool isTopSegment = true;
            while (destinationY < Bounds.Bottom)
            {
                int sliceHeight = Math.Min(SourceSegmentHeight, Bounds.Bottom - destinationY);
                Rectangle source;
                if (isTopSegment)
                {
                    source = _isHorizontalPipeAttachment ? AttachedPipeTopSource : PipeTopSource;
                }
                else
                {
                    source = _isHorizontalPipeAttachment ? AttachedPipeBodySource : PipeBodySource;
                    if (sliceHeight < SourceSegmentHeight)
                    {
                        source.Height = source.Height * sliceHeight / SourceSegmentHeight;
                    }
                }

                source = BlockSpriteFactory.GetThemedSource(source, _isUnderground, _isUnderwater);
                spriteBatch.Draw(
                    blockTexture,
                    new Rectangle(Bounds.Left, destinationY, Width, sliceHeight),
                    source,
                    Color.White);
                destinationY += sliceHeight;
                isTopSegment = false;
            }
        }

        private void DrawHorizontalPipe(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            int attachedWidth = Width > DefaultWidth * 2
                ? DefaultWidth * 2
                : Math.Min(DefaultWidth, Width);
            Rectangle bodyDestination = new Rectangle(Bounds.Left, Bounds.Top, attachedWidth, Height);
            spriteBatch.Draw(
                blockTexture,
                bodyDestination,
                BlockSpriteFactory.GetThemedSource(
                    HorizontalPipeBodySource, _isUnderground, _isUnderwater),
                Color.White);

            if (Width <= DefaultWidth * 2)
            {
                int endWidth = Width - attachedWidth;
                if (endWidth <= 0)
                {
                    return;
                }

                spriteBatch.Draw(
                    blockTexture,
                    new Rectangle(Bounds.Right - endWidth, Bounds.Top, endWidth, Height),
                    BlockSpriteFactory.GetThemedSource(
                        HorizontalPipeEndSource, _isUnderground, _isUnderwater),
                    Color.White);
            }
        }

        private void DrawPlatform(SpriteBatch spriteBatch, Texture2D blockTexture)
        {
            int segmentWidth = Math.Min(DefaultWidth, Width);
            if (Width <= DefaultWidth)
            {
                spriteBatch.Draw(
                    blockTexture,
                    Bounds,
                    BlockSpriteFactory.GetPlatformSource(1, _isUnderground, _isUnderwater),
                    Color.White);
                return;
            }

            int x = Bounds.Left;
            int remainingWidth = Width;
            spriteBatch.Draw(
                blockTexture,
                new Rectangle(x, Bounds.Top, segmentWidth, Height),
                BlockSpriteFactory.GetPlatformSource(0, _isUnderground, _isUnderwater),
                Color.White);
            x += segmentWidth;
            remainingWidth -= segmentWidth;

            while (remainingWidth > segmentWidth)
            {
                spriteBatch.Draw(
                    blockTexture,
                    new Rectangle(x, Bounds.Top, segmentWidth, Height),
                    BlockSpriteFactory.GetPlatformSource(1, _isUnderground, _isUnderwater),
                    Color.White);
                x += segmentWidth;
                remainingWidth -= segmentWidth;
            }

            spriteBatch.Draw(
                blockTexture,
                new Rectangle(x, Bounds.Top, remainingWidth, Height),
                BlockSpriteFactory.GetPlatformSource(2, _isUnderground, _isUnderwater),
                Color.White);
        }

        private void DrawBrickDebris(
            SpriteBatch spriteBatch,
            Texture2D blockTexture,
            Texture2D tileSheetTexture)
        {
            int fragmentWidth = Width / 2;
            int fragmentHeight = Height / 2;
            float opacity = 1f - _breakAnimationElapsed / BreakAnimationDurationSeconds;
            bool useTileSheet = _tileId.HasValue && tileSheetTexture != null;
            Rectangle brickSource = useTileSheet
                ? LevelTile.GetSourceRectangle(_tileId.Value)
                : BlockSpriteFactory.GetBrickSource(_isUnderground, _isTopBrick, _isUnderwater);

            for (int index = 0; index < _debrisPositions.Length; index++)
            {
                int column = index % 2;
                int row = index / 2;
                Rectangle source = new Rectangle(
                    brickSource.X + column * 8,
                    brickSource.Y + row * 8,
                    8,
                    8);
                Rectangle destination = new Rectangle(
                    (int)_debrisPositions[index].X,
                    (int)_debrisPositions[index].Y,
                    fragmentWidth,
                    fragmentHeight);
                spriteBatch.Draw(
                    useTileSheet ? tileSheetTexture : blockTexture,
                    destination,
                    source,
                    Color.White * opacity);
            }
        }

        private bool TryDrawReferenceTile(SpriteBatch spriteBatch, Texture2D tileSheetTexture)
        {
            if (tileSheetTexture == null || !_tileId.HasValue)
            {
                return false;
            }

            int tileId;
            if (Type == BlockType.Used && _usedTileId.HasValue)
            {
                tileId = _usedTileId.Value;
            }
            else if (Type == BlockType.Used && _tileId.HasValue)
            {
                int? paletteUsedTileId = BlockTileIds.GetUsedTileId(_tileId.Value);
                if (!paletteUsedTileId.HasValue)
                {
                    return false;
                }

                tileId = paletteUsedTileId.Value;
            }
            else if (Type == BlockType.Question)
            {
                tileId = _tileId.Value + _sprite.FrameIndex;
            }
            else if (Type == BlockType.Brick)
            {
                tileId = _tileId.Value;
            }
            else if (Type == BlockType.Axe || Type == BlockType.Bridge)
            {
                tileId = _tileId.Value;
            }
            else
            {
                return false;
            }

            spriteBatch.Draw(
                tileSheetTexture,
                Bounds,
                LevelTile.GetSourceRectangle(tileId),
                Color.White);
            return true;
        }

        private bool TryDrawReferencePlatform(SpriteBatch spriteBatch, Texture2D tileSheetTexture)
        {
            if (tileSheetTexture == null || !_tileId.HasValue)
            {
                return false;
            }

            if (_isPlatformLevel)
            {
                Rectangle source = LevelTile.GetSourceRectangle(_tileId.Value);
                source.Width = 16 * 3;
                spriteBatch.Draw(
                    tileSheetTexture,
                    new Rectangle(Bounds.Left, Bounds.Top, Width, DefaultHeight),
                    source,
                    Color.White);
                return true;
            }

            int segmentCount = (Width + DefaultWidth - 1) / DefaultWidth;
            for (int segment = 0; segment < segmentCount; segment++)
            {
                int destinationX = Bounds.Left + segment * DefaultWidth;
                int width = Math.Min(DefaultWidth, Bounds.Right - destinationX);
                Rectangle source = LevelTile.GetSourceRectangle(_tileId.Value + segment);
                spriteBatch.Draw(
                    tileSheetTexture,
                    new Rectangle(destinationX, Bounds.Top, width, Height),
                    source,
                    Color.White);
            }

            return true;
        }

        private void DrawVine(SpriteBatch spriteBatch, Texture2D vineTexture)
        {
            int segmentCount = (_height + DefaultHeight - 1) / DefaultHeight;
            for (int segment = 0; segment < segmentCount; segment++)
            {
                int destinationY = Bounds.Top + segment * DefaultHeight;
                int height = Math.Min(DefaultHeight, Bounds.Bottom - destinationY);
                int tileId = segment == 0 ? 100 : 148;
                spriteBatch.Draw(
                    vineTexture,
                    new Rectangle(Bounds.Left, destinationY, Width, height),
                    LevelTile.GetSourceRectangle(tileId),
                    Color.White);
            }
        }
    }
}
