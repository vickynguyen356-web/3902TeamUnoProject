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
        private readonly CollisionSystem _collisionSystem;
        private readonly MarioPlayer _player;
        private readonly Level _level;
        private readonly Camera _camera;
        private readonly IReadOnlyDictionary<string, LevelDefinition> _areas;
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
        private float _timeRemaining = LevelTimerDurationSeconds;

        public GameProgress Progress { get; } = new GameProgress();
        public GameSessionState State { get; private set; } = GameSessionState.Title;
        public float PipeTransitionOpacity { get; private set; }
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
            Player.UpdateVelocity(elapsedSeconds);
            Player.UpdatePosition(elapsedSeconds);
            Level.CollectProjectiles(Player);
            UpdateStarMusic();
            Level.ListenerPosition = new Vector2(_player.Bounds.Center.X, _player.Bounds.Center.Y);
            Level.Update(gameTime);
            _collisionSystem.Update(this, previousPlayerBounds, isJumpHeld);
            Level.CollectProjectiles(Player);

            int coinsCollected = Level.CollectBlockItems();
            for (int index = 0; index < coinsCollected; index = index + 1)
            {
                Progress.AddCoin();
            }

            Level.RemoveDeletedBlocks();
            Block pipe = Level.GetPipeTravelRequest();
            if (Player.Bounds.Top >= Level.Definition.Height)
            {
                Player.DieFromFall();
            }

            if (Player.IsDead)
            {
                BeginDeath();
            }
            else if (Player.IsSlidingOnFlagpole)
            {
                _music.Stop();
                _isStarThemePlaying = false;
                SoundPlayer.Play("Audio/SoundEffects/flagpole");
                State = GameSessionState.FinishingLevel;
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

        private void BeginPipeTransition(Block pipe)
        {
            _pipeDestinationArea = pipe.DestinationArea;
            _pipeDestinationSpawn = pipe.DestinationSpawn;
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
                LoadArea(_pipeDestinationArea, _pipeDestinationSpawn);
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
                return;
            }

            _timeRemaining = LevelTimerDurationSeconds;
            Player.Respawn();
            LoadArea(Level.Definition.LevelStartAreaId);
        }

        private void UpdateLevelFinish(GameTime gameTime, float elapsedSeconds)
        {
            Player.UpdateState(elapsedSeconds);
            Player.UpdateVelocity(elapsedSeconds);
            Player.UpdatePosition(elapsedSeconds);
            Player.UpdateAnimation(gameTime);
            Camera.Follow(Player.Bounds);
            if (Player.IsSlidingOnFlagpole)
            {
                return;
            }

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

        public void TogglePause()
        {
            if (State == GameSessionState.Playing)
            {
                SoundPlayer.Play("Audio/SoundEffects/pause");
                State = GameSessionState.Paused;
                Player.ClearControls();
                _music.Pause();
            }
            else if (State == GameSessionState.Paused)
            {
                State = GameSessionState.Playing;
                _music.Resume();
            }
        }

        public void StartGame()
        {
            if (State != GameSessionState.Title)
            {
                return;
            }

            State = GameSessionState.Playing;
            _timeRemaining = LevelTimerDurationSeconds;
            _music.Play(Level.Definition.MusicTrack);
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
            _music.Stop();
            ShouldExit = false;
            Progress.Reset();
            _timeRemaining = LevelTimerDurationSeconds;
            Player.Reset();
            LoadArea(Level.Definition.Id);
        }

        public void LoadArea(string areaId, string spawnId = "start")
        {
            LevelDefinition definition = _areas[areaId];
            Vector2 spawn = definition.GetSpawnPoint(spawnId);
            if (Progress.Lives == 0)
            {
                Progress.Reset();
            }

            if (Player.IsDead)
            {
                Player.Respawn();
            }

            Level.LoadArea(definition);
            Player.EnterArea(spawn);
            _music.Play(definition.MusicTrack);
            _deathTimeRemaining = 0;
            State = GameSessionState.Playing;
            Camera.SetLevelWidth(definition.Width);
            Camera.ResumeFollowing();
            Camera.Follow(Player.Bounds);
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
