using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    public class GameSession : IGameActions
    {
        private readonly IPlayerMovement _playerMovement;
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

        public GameSession(MarioPlayer player, Level level, IPlayerMovement playerMovement)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            if (playerMovement == null)
            {
                throw new ArgumentNullException(nameof(playerMovement));
            }

            _player = player;
            _level = level;
            _playerMovement = playerMovement;
        }

        public void Update(GameTime gameTime)
        {
            // Limit large time steps after a slow frame
            float elapsedSeconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);

            Player.UpdateVelocity(elapsedSeconds);
            _playerMovement.Move(Player, elapsedSeconds);

            // Animation uses the updated velocity and grounded state
            Player.UpdateAnimation(gameTime);
            Level.Update(gameTime);
        }

        public void Quit()
        {
            ShouldExit = true;
        }

        public void Reset()
        {
            ShouldExit = false;
            Level.Reset();
            Player.Reset();
        }

        public void TriggerDamage()
        {
            Player.TakeDamage();
        }
    }
}
