using System;
using Microsoft.Xna.Framework;
using Sprint0.Entities;
using Sprint0.Interfaces;

namespace Sprint0.World
{
    public class GameSession : IGameActions
    {
        private readonly IPlayerMovement _playerMovement;
        private readonly ICollisionSystem _collisionSystem;

        public MarioPlayer Player { get; }
        public Level Level { get; }
        public bool ShouldExit { get; private set; }

        public GameSession(
            MarioPlayer player,
            Level level,
            IPlayerMovement playerMovement,
            ICollisionSystem collisionSystem)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            Player = player;
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            Level = level;
            if (playerMovement == null)
            {
                throw new ArgumentNullException(nameof(playerMovement));
            }

            _playerMovement = playerMovement;
            if (collisionSystem == null)
            {
                throw new ArgumentNullException(nameof(collisionSystem));
            }

            _collisionSystem = collisionSystem;
        }

        public void Update(GameTime gameTime)
        {
            // Limit large time steps after a slow frame
            float elapsedSeconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            Vector2 previousPosition = Player.Position;

            // Update speed, move Mario, then check collisions
            Player.UpdateVelocity(elapsedSeconds);
            _playerMovement.Move(Player, elapsedSeconds);
            _collisionSystem.Resolve(Player, previousPosition);

            // Pick the animation after movement is finished
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
    }
}
