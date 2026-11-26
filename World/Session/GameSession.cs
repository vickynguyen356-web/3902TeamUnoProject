using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TeamUno.Mario.Audio;
using TeamUno.Mario.Collision;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class GameSession : IGameActions
    {
        private const float DeathDelaySeconds = 4f;
        private const float LevelTimerDurationSeconds = 300f;
        private const float PipeTransitionDurationSeconds = 0.8f;
        private const float PipeTransitionHalfDurationSeconds = PipeTransitionDurationSeconds / 2f;
        private const float FlagpoleRunDelaySeconds = 0.6f;
        private const float LevelFinishDelaySeconds = 4.5f;
        private readonly CollisionSystem _collisionSystem;
        private readonly MarioPlayer _player;
        private readonly Level _level;
        private readonly Camera _camera;
        private readonly IReadOnlyDictionary<string, LevelDefinition> _areas;
        private readonly string _startingAreaId;
        private float _deathTimeRemaining;
        private bool _shouldExit;
        private readonly MusicPlayer _music;
        private const string StarTheme = "Audio/SoundEffects/invincibilityTheme";
        private bool _isStarThemePlaying;
        private bool _isMuted;
        private float _pipeTransitionElapsed;
        private bool _pipeDestinationLoaded;
        private string _pipeDestinationArea;
        private string _pipeDestinationSpawn;
        private Vector2? _pipeDestinationPosition;
        private Vector2? _pipeDestinationCameraPosition;
        private string _pipeExitDirection;
        private bool _pipeFreezeCamera;
        private Color? _pipeDestinationBackgroundColor;
        private bool? _pipeDestinationIsUnderwater;
        private string _pipeDestinationMusicTrack;
        private Color _backgroundColor;
        private bool _isUnderwater;
        private float _timeRemaining = LevelTimerDurationSeconds;
        private LevelFinishPhase _levelFinishPhase;
        private float _levelFinishWaitRemaining;

        private enum LevelFinishPhase
        {
            FlagSlide,
            WaitBeforeWalk,
            WalkToCastle,
            WaitAfterCastle,
            WaitAfterBoss
        }

        public GameProgress Progress { get; } = new GameProgress();
        public GameSessionState State { get; private set; } = GameSessionState.Title;
        public float PipeTransitionOpacity { get; private set; }
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
        }

        public bool IsUnderwater
        {
            get { return _isUnderwater; }
        }

        public int TimeRemaining
        {
            get { return (int)Math.Ceiling(_timeRemaining); }
        }

        public MarioPlayer Player
        {
            get
            {
                return _player;
            }
        }

        public Level Level
        {
            get
            {
                return _level;
            }
        }

        public Camera Camera
        {
            get
            {
                return _camera;
            }
        }

        public bool CanControlPlayer
        {
            get
            {
                return State == GameSessionState.Playing;
            }
        }

        public bool ShouldExit
        {
            get
            {
                return _shouldExit;
            }
            private set
            {
                _shouldExit = value;
            }
        }

        public GameSession(
            MarioPlayer player,
            Level level,
            Camera camera,
            IReadOnlyDictionary<string, LevelDefinition> areas,
            MusicPlayer music)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(level);
            ArgumentNullException.ThrowIfNull(camera);
            ArgumentNullException.ThrowIfNull(areas);
            ArgumentNullException.ThrowIfNull(music);

            _player = player;
            _level = level;
            _camera = camera;
            _areas = areas;
            _backgroundColor = level.Definition.BackgroundColor;
            _isUnderwater = level.Definition.IsUnderwater;
            _startingAreaId = level.Definition.LevelStartAreaId;
            _collisionSystem = CollisionSystem.CreateDefault(Progress);
            _music = music;
            Camera.Follow(Player.Bounds);
        }

        public void Update(GameTime gameTime, bool isJumpHeld = false)
        {
            float elapsedSeconds = CalculateElapsedSeconds(gameTime);

            switch (State)
            {
                case GameSessionState.Playing:
                    UpdateGameplay(gameTime, elapsedSeconds, isJumpHeld);
                    break;
                case GameSessionState.Dying:
                    UpdateDeath(gameTime, elapsedSeconds);
                    break;
                case GameSessionState.FinishingLevel:
                    UpdateLevelFinish(gameTime, elapsedSeconds);
                    break;
                case GameSessionState.PipeTransition:
                    UpdatePipeTransition(elapsedSeconds);
                    break;
            }
        }

        private void UpdateStarMusic()
        {
            bool starActive = Player.IsStarActive && !Player.IsDead;

            if (starActive && !_isStarThemePlaying)
            {
                _music.PlayInterrupt(StarTheme);
                _isStarThemePlaying = true;
            }
            else if (!starActive && _isStarThemePlaying)
            {
                _music.StopInterrupt();
                _isStarThemePlaying = false;
            }
        }

        private void UpdateGameplay(GameTime gameTime, float elapsedSeconds, bool isJumpHeld)
        {
            _timeRemaining = Math.Max(0f, _timeRemaining - elapsedSeconds);
            if (_timeRemaining <= 0f)
            {
                Player.DieFromFall();
            }

            if (Player.IsDead)
            {
                BeginDeath();
                Player.UpdateAnimation(gameTime);
                return;
            }

            Rectangle previousPlayerBounds = Player.Bounds;
            Player.UpdateState(elapsedSeconds);
            Player.UpdateVelocity(elapsedSeconds, isJumpHeld);
            Player.UpdatePosition(elapsedSeconds);
            if (Level.IsLavaAt(Player.Bounds))
            {
                Player.DieFromFall();
            }

            if (Player.IsDead)
            {
                BeginDeath();
                Player.UpdateAnimation(gameTime);
                return;
            }

            bool bowserWasDefeated = Level.IsBowserDefeated;
            if (Level.TryActivateCastleAxe(Player.Bounds))
            {
                Progress.AddScore(5000);
            }

            Level.CollectProjectiles(Player);
            UpdateStarMusic();
            Level.ListenerPosition = new Vector2(_player.Bounds.Center.X, _player.Bounds.Center.Y);
            Level.ListenerBounds = _player.Bounds;
            Level.Update(gameTime);
            bool bowserDefeatedThisFrame = !bowserWasDefeated && Level.IsBowserDefeated;
            Level.CarryPlayerWithMovingPlatforms(Player, previousPlayerBounds);
            _collisionSystem.Update(this, previousPlayerBounds, isJumpHeld);
            Level.CollectProjectiles(Player);

            int coinsCollected = Level.CollectBlockItems();
            for (int index = 0; index < coinsCollected; index = index + 1)
            {
                Progress.AddCoin();
            }

            VineSpawnDefinition climbedVine = Level.GetVineTravelRequest();
            if (climbedVine != null)
            {
                Vector2 exitPosition = new Vector2(
                    climbedVine.DestinationPosition.X + 48f,
                    climbedVine.DestinationPosition.Y - Player.Bounds.Height);
                Player.CompleteVineClimb(exitPosition);
                _backgroundColor = climbedVine.BackgroundColor;
                _isUnderwater = climbedVine.LevelType.Contains(
                    "UNDERWATER", StringComparison.OrdinalIgnoreCase);
                _music.Play(climbedVine.MusicTrack);
                Camera.SetMaximumX(climbedVine.CameraMaxX);
                Camera.MoveTo(climbedVine.CameraPosition.X, climbedVine.CameraPosition.Y);
            }

            Level.RemoveDeletedBlocks();
            IBlock pipe = Level.GetPipeTravelRequest();
            if (Player.Bounds.Top >= Level.Definition.Height)
            {
                Player.DieFromFall();
            }

            if (Player.IsDead)
            {
                BeginDeath();
            }
            else if (bowserDefeatedThisFrame)
            {
                BeginBowserRescue();
            }
            else if (Player.IsSlidingOnFlagpole
                && (!Level.HasBowser || Level.IsBowserDefeated))
            {
                _music.Stop();
                _isStarThemePlaying = false;
                SoundPlayer.Play("Audio/SoundEffects/flagpole");
                State = GameSessionState.FinishingLevel;
                _levelFinishPhase = LevelFinishPhase.FlagSlide;
                _levelFinishWaitRemaining = 0f;
                Level.BeginFlagpoleSequence();
                Player.ClearControls();
                Level.ClearEnemies();
                _music.Stop();
            }
            else if (pipe != null && !Player.IsChangingForm)
            {
                BeginPipeTransition(pipe);
                return;
            }

            Player.UpdateAnimation(gameTime);
            Camera.Follow(Player.Bounds);
            Player.ClearPipeEntryRequest();
        }

        private void BeginPipeTransition(IBlock pipe)
        {
            _pipeDestinationArea = pipe.DestinationArea;
            _pipeDestinationSpawn = pipe.DestinationSpawn;
            _pipeDestinationPosition = pipe.DestinationPosition;
            _pipeDestinationCameraPosition = pipe.DestinationCameraPosition;
            _pipeExitDirection = pipe.ExitDirection;
            _pipeFreezeCamera = pipe.FreezeCamera;
            _pipeDestinationBackgroundColor = pipe.DestinationBackgroundColor;
            _pipeDestinationIsUnderwater = pipe.DestinationIsUnderwater;
            _pipeDestinationMusicTrack = pipe.DestinationMusicTrack;
            _pipeTransitionElapsed = 0f;
            _pipeDestinationLoaded = false;
            PipeTransitionOpacity = 0f;
            Player.ClearControls();
            State = GameSessionState.PipeTransition;
            SoundPlayer.Play("Audio/SoundEffects/pipepowerdown");
        }

        private void UpdatePipeTransition(float elapsedSeconds)
        {
            _pipeTransitionElapsed = Math.Min(
                PipeTransitionDurationSeconds, _pipeTransitionElapsed + elapsedSeconds);

            if (!_pipeDestinationLoaded
                && _pipeTransitionElapsed >= PipeTransitionHalfDurationSeconds)
            {
                if (string.Equals(
                    _pipeDestinationArea, Level.Definition.Id, StringComparison.OrdinalIgnoreCase)
                    && _pipeDestinationPosition.HasValue)
                {
                    ApplyPipeEnvironment();
                    Player.EnterPipe(
                        _pipeDestinationPosition.Value,
                        _isUnderwater,
                        _pipeExitDirection);
                    ApplyPipeCamera();
                }
                else
                {
                    LoadArea(
                        _pipeDestinationArea,
                        _pipeDestinationSpawn,
                        _pipeDestinationPosition,
                        _pipeDestinationCameraPosition,
                        _pipeFreezeCamera,
                        _pipeExitDirection,
                        _pipeDestinationBackgroundColor,
                        _pipeDestinationIsUnderwater,
                        _pipeDestinationMusicTrack);
                }

                State = GameSessionState.PipeTransition;
                _pipeDestinationLoaded = true;
            }

            if (_pipeTransitionElapsed <= PipeTransitionHalfDurationSeconds)
            {
                PipeTransitionOpacity = _pipeTransitionElapsed / PipeTransitionHalfDurationSeconds;
            }
            else
            {
                PipeTransitionOpacity = 1f
                    - (_pipeTransitionElapsed - PipeTransitionHalfDurationSeconds)
                    / PipeTransitionHalfDurationSeconds;
            }

            if (_pipeTransitionElapsed >= PipeTransitionDurationSeconds)
            {
                PipeTransitionOpacity = 0f;
                _pipeDestinationArea = null;
                _pipeDestinationSpawn = null;
                _pipeDestinationPosition = null;
                _pipeDestinationCameraPosition = null;
                _pipeExitDirection = null;
                _pipeFreezeCamera = false;
                _pipeDestinationBackgroundColor = null;
                _pipeDestinationIsUnderwater = null;
                _pipeDestinationMusicTrack = null;
                _pipeDestinationLoaded = false;
                State = GameSessionState.Playing;
            }
        }

        private void BeginDeath()
        {
            _music.Stop();
            _isStarThemePlaying = false;
            SoundPlayer.Play("Audio/SoundEffects/death");
            Progress.LoseLife();
            State = GameSessionState.Dying;
            _deathTimeRemaining = DeathDelaySeconds;
            Player.ClearControls();
        }

        private void BeginBowserRescue()
        {
            _music.Stop();
            _isStarThemePlaying = false;
            State = GameSessionState.FinishingLevel;
            _levelFinishPhase = LevelFinishPhase.WaitAfterBoss;
            _levelFinishWaitRemaining = LevelFinishDelaySeconds;
            Player.ClearControls();
            Level.ClearEnemies();
        }

        private void UpdateDeath(GameTime gameTime, float elapsedSeconds)
        {
            Player.UpdateAnimation(gameTime);
            _deathTimeRemaining = _deathTimeRemaining - elapsedSeconds;
            if (_deathTimeRemaining > 0)
            {
                return;
            }

            if (Progress.Lives == 0)
            {
                State = GameSessionState.GameOver;
                SoundPlayer.Play("Audio/SoundEffects/gameover");
                return;
            }

            _timeRemaining = LevelTimerDurationSeconds;
            Player.Respawn();
            LoadArea(Level.Definition.LevelStartAreaId);
        }

        private void UpdateLevelFinish(GameTime gameTime, float elapsedSeconds)
        {
            Level.UpdateFlagpoleFlags(gameTime);
            if (_levelFinishPhase == LevelFinishPhase.FlagSlide)
            {
                Player.UpdatePosition(elapsedSeconds);
                if (!Player.IsSlidingOnFlagpole)
                {
                    _levelFinishPhase = LevelFinishPhase.WaitBeforeWalk;
                    _levelFinishWaitRemaining = FlagpoleRunDelaySeconds;
                }
            }
            else if (_levelFinishPhase == LevelFinishPhase.WaitBeforeWalk)
            {
                _levelFinishWaitRemaining -= elapsedSeconds;
                if (_levelFinishWaitRemaining <= 0f)
                {
                    Player.BeginLevelFinishWalk();
                    _levelFinishPhase = LevelFinishPhase.WalkToCastle;
                }
            }
            else if (_levelFinishPhase == LevelFinishPhase.WalkToCastle)
            {
                Rectangle previousBounds = Player.Bounds;
                Player.UpdateLevelFinishWalk(elapsedSeconds);
                if (Level.ResolveLevelFinishWalk(Player, previousBounds))
                {
                    Player.StopLevelFinishWalk(Player.Position.X);
                    _levelFinishPhase = LevelFinishPhase.WaitAfterCastle;
                    _levelFinishWaitRemaining = LevelFinishDelaySeconds;
                }
            }
            else if (_levelFinishPhase == LevelFinishPhase.WaitAfterCastle
                || _levelFinishPhase == LevelFinishPhase.WaitAfterBoss)
            {
                _levelFinishWaitRemaining -= elapsedSeconds;
                if (_levelFinishWaitRemaining <= 0f)
                {
                    string nextAreaId = Level.Definition.NextAreaId;
                    if (nextAreaId == null)
                    {
                        State = GameSessionState.Won;
                    }
                    else
                    {
                        LoadArea(nextAreaId);
                    }
                }
            }

            Player.UpdateAnimation(gameTime);
            Camera.Follow(Player.Bounds);
        }

        public void TogglePause()
        {
            if (State == GameSessionState.Playing)
            {
                OpenMenu();
                SoundPlayer.Play("Audio/SoundEffects/pause");
                _music.Pause();
            }
            else if (State == GameSessionState.Menu)
            {
                CloseMenu();
            }
        }

        public void StartGame()
        {
            if (State != GameSessionState.Title)
            {
                return;
            }

            Reset(_startingAreaId);
        }

        public void ReturnToTitle()
        {
            _music.Stop();
            _isStarThemePlaying = false;
            Player.ClearControls();
            State = GameSessionState.Title;
        }

        public void ToggleMute()
        {
            _isMuted = !_isMuted;
            if (_isMuted)
            {
                MediaPlayer.IsMuted = true;
                SoundEffect.MasterVolume = 0f;
            }
            else
            {
                MediaPlayer.IsMuted = false;
                SoundEffect.MasterVolume = 1f;
            }
        }

        public void Quit()
        {
            ShouldExit = true;
            _music.Stop();
        }

        public void Reset()
        {
            string areaId = Level.Definition.Id;
            if (State == GameSessionState.GameOver || State == GameSessionState.Won)
            {
                areaId = _startingAreaId;
            }
            Reset(areaId);
        }

        private void Reset(string areaId)
        {
            _music.Stop();
            _isStarThemePlaying = false;
            ShouldExit = false;
            Progress.Reset();
            _timeRemaining = LevelTimerDurationSeconds;
            if (State == GameSessionState.GameOver || State == GameSessionState.Won)
            {
                Player.Respawn();
            }
            else
            {
                Player.Reset();
            }
            LoadArea(areaId);
        }

        public void LoadArea(string areaId, string spawnId = "start")
        {
            LoadArea(areaId, spawnId, null);
        }

        private void LoadArea(
            string areaId,
            string spawnId,
            Vector2? spawnPosition,
            Vector2? cameraPosition = null,
            bool freezeCamera = false,
            string exitDirection = null,
            Color? backgroundColor = null,
            bool? isUnderwater = null,
            string musicTrack = null)
        {
            LevelDefinition definition = _areas[areaId];
            _levelFinishPhase = LevelFinishPhase.FlagSlide;
            _levelFinishWaitRemaining = 0f;
            Vector2 spawn = spawnPosition ?? definition.GetSpawnPoint(spawnId);
            if (Progress.Lives == 0)
            {
                Progress.Reset();
            }

            if (Player.IsDead)
            {
                Player.Respawn();
            }

            Level.LoadArea(definition);
            _backgroundColor = backgroundColor ?? definition.BackgroundColor;
            _isUnderwater = isUnderwater ?? definition.IsUnderwater;
            if (spawnPosition.HasValue)
            {
                Player.EnterPipe(spawnPosition.Value, _isUnderwater, exitDirection);
            }
            else
            {
                Player.EnterArea(spawn, _isUnderwater);
            }

            _music.Play(musicTrack ?? definition.MusicTrack);
            _deathTimeRemaining = 0;
            State = GameSessionState.Playing;
            Camera.SetLevelSize(definition.Width, definition.Height);
            Vector2 camera = cameraPosition ?? definition.CameraStart;
            Camera.MoveTo(camera.X, camera.Y);
            if (freezeCamera)
            {
                Camera.PauseFollowing();
            }
            else
            {
                Camera.ResumeFollowing();
                Camera.Follow(Player.Bounds);
            }
        }

        private void ApplyPipeEnvironment()
        {
            _backgroundColor = _pipeDestinationBackgroundColor ?? Level.Definition.BackgroundColor;
            _isUnderwater = _pipeDestinationIsUnderwater ?? Level.Definition.IsUnderwater;
            _music.Play(_pipeDestinationMusicTrack ?? Level.Definition.MusicTrack);
        }

        private void ApplyPipeCamera()
        {
            Vector2 camera = _pipeDestinationCameraPosition ?? Vector2.Zero;
            Camera.MoveTo(camera.X, camera.Y);
            if (_pipeFreezeCamera)
            {
                Camera.PauseFollowing();
            }
            else
            {
                Camera.ResumeFollowing();
                Camera.Follow(Player.Bounds);
            }
        }

        private static float CalculateElapsedSeconds(GameTime gameTime)
        {
            const float maximumElapsedSeconds = 1f / 30f;
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            return Math.Min(elapsedSeconds, maximumElapsedSeconds);
        }

        public IEnumerable<string> AreaIds
        {
            get
            {
                return _areas.Keys;
            }
        }

        public void OpenMenu()
        {
            if (State != GameSessionState.Playing)
            {
                return;
            }

            State = GameSessionState.Menu;
            Player.ClearControls();
            Player.SetSprinting(false);
            _music.Pause();
        }

        public void CloseMenu()
        {
            if (State == GameSessionState.Menu)
            {
                State = GameSessionState.Playing;
                _music.Resume();
            }
        }
    }
}
