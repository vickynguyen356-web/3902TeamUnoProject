using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Collision;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class GameSession : IGameActions
    {
        private readonly CollisionSystem _collisionSystem;

        public GameProgress Progress { get; } = new GameProgress();
        private readonly MarioPlayer _player;
        private readonly Level _level;
        private readonly Camera _camera;
        private bool _shouldExit;

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

        public GameSession(MarioPlayer player, Level level, Camera camera)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(level);
            ArgumentNullException.ThrowIfNull(camera);

            _player = player;
            _level = level;
            _camera = camera;
            _collisionSystem = CollisionSystem.CreateDefault(Progress);
            Camera.Follow(Player.Bounds);
        }

        public void Update(GameTime gameTime, bool isJumpHeld = false)
        {
            float elapsedSeconds = CalculateElapsedSeconds(gameTime);
            Rectangle previousPlayerBounds = Player.Bounds;

            Player.UpdateState(elapsedSeconds);
            Player.UpdateVelocity(elapsedSeconds);
            Player.UpdatePosition(elapsedSeconds);
            Level.CollectProjectiles(Player);
            Level.Update(gameTime);
            _collisionSystem.Update(this, previousPlayerBounds, isJumpHeld);
            Level.RemoveDeletedBlocks();
            Rectangle playerBounds = Player.Bounds;
            if (playerBounds.Top >= Level.Definition.Height)
            {
                Player.DieFromFall();
            }

            Player.UpdateAnimation(gameTime);
            Camera.Follow(Player.Bounds);
            Player.ClearPipeEntryRequest();
        }

        public void Quit()
        {
            ShouldExit = true;
        }

        public void Reset()
        {
            ShouldExit = false;
            Progress.Reset();
            Level.Reset();
            Player.Reset();
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
