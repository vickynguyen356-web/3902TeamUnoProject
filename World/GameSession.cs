using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class GameSession : IGameActions
    {
        private static readonly TimeSpan MaximumElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);
        private readonly GameTime _simulationTime = new GameTime();
        private readonly CollisionSystem _collisionSystem = new CollisionSystem();
        private readonly MarioPlayer _player;
        private readonly Level _level;
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

        public GameSession(MarioPlayer player, Level level)
        {
            ArgumentNullException.ThrowIfNull(player);

            ArgumentNullException.ThrowIfNull(level);

            _player = player;
            _level = level;
            Player.FireballRequested += SpawnMarioFireball;
        }

        public void Update(GameTime gameTime)
        {
            TimeSpan elapsedTime = gameTime.ElapsedGameTime;
            if (elapsedTime > MaximumElapsedTime)
            {
                elapsedTime = MaximumElapsedTime;
            }
            else if (elapsedTime < TimeSpan.Zero)
            {
                elapsedTime = TimeSpan.Zero;
            }

            _simulationTime.ElapsedGameTime = elapsedTime;
            _simulationTime.TotalGameTime = _simulationTime.TotalGameTime + elapsedTime;
            _simulationTime.IsRunningSlowly = gameTime.IsRunningSlowly;
            float elapsedSeconds = (float)elapsedTime.TotalSeconds;
            Rectangle previousPlayerBounds = Player.Bounds;

            Player.UpdateVelocity(elapsedSeconds);
            Player.UpdatePosition(elapsedSeconds);
            Level.Update(_simulationTime);
            _collisionSystem.Update(this, previousPlayerBounds);
            Player.UpdateAnimation(_simulationTime);
        }

        public void Quit()
        {
            ShouldExit = true;
        }

        public void Reset()
        {
            ShouldExit = false;
            _simulationTime.ElapsedGameTime = TimeSpan.Zero;
            _simulationTime.TotalGameTime = TimeSpan.Zero;
            _simulationTime.IsRunningSlowly = false;
            Level.Reset();
            Player.Reset();
        }

        private void SpawnMarioFireball()
        {
            Level.SpawnMarioFireball(Player);
        }

        public void SpitFire()
        {
            foreach (IEnemy enemy in Level.Enemies)
            {
                Bowser bowser = enemy as Bowser;
                if (bowser != null)
                {
                    bowser.SpitFire();
                }
            }
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Pipe transitions will update the current session")]
        public void BeginPipeTransition(Block pipe, Vector2 destination)
        {
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Flagpole movement will update the current session")]
        public void BeginFlagpoleSlide(Block flagpole)
        {
        }
    }
}
