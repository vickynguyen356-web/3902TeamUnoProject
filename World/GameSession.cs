using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Collision;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class GameSession : IGameActions
    {
        private const float DeathDelaySeconds = 1.2f;
        private readonly CollisionSystem _collisionSystem;
        private readonly MarioPlayer _player;
        private readonly Level _level;
        private readonly Camera _camera;
        private readonly IReadOnlyDictionary<string, LevelDefinition> _areas;
        private float _deathTimeRemaining;
        private bool _shouldExit;

        public GameProgress Progress { get; } = new GameProgress();
        public GameSessionState State { get; private set; } = GameSessionState.Playing;

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
            IReadOnlyDictionary<string, LevelDefinition> areas)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(level);
            ArgumentNullException.ThrowIfNull(camera);
            ArgumentNullException.ThrowIfNull(areas);

            _player = player;
            _level = level;
            _camera = camera;
            _areas = areas;
            _collisionSystem = CollisionSystem.CreateDefault(Progress);
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
            }
        }

        private void UpdateGameplay(GameTime gameTime, float elapsedSeconds, bool isJumpHeld)
        {
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
                State = GameSessionState.FinishingLevel;
                Player.ClearControls();
            }
            else if (pipe != null && !Player.IsChangingForm)
            {
                LoadArea(pipe.DestinationArea, pipe.DestinationSpawn);
                return;
            }

            Player.UpdateAnimation(gameTime);
            Camera.Follow(Player.Bounds);
            Player.ClearPipeEntryRequest();
        }

        private void BeginDeath()
        {
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
                State = GameSessionState.Paused;
                Player.ClearControls();
            }
            else if (State == GameSessionState.Paused)
            {
                State = GameSessionState.Playing;
            }
        }

        public void Quit()
        {
            ShouldExit = true;
        }

        public void Reset()
        {
            ShouldExit = false;
            Progress.Reset();
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
    }
}
