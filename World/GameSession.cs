using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace TeamUno.Mario.World
{
    internal class GameSession : IGameActions
    {
        private readonly IPlayerMovement _playerMovement;
        private readonly MarioPlayer _player;
        private readonly Level _level;
        private readonly IProjectileFactory _projectileFactory;
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

        public GameSession(MarioPlayer player, Level level, IPlayerMovement playerMovement, IProjectileFactory projectileFactory)
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

            if (projectileFactory == null)
            {
                throw new ArgumentNullException(nameof(projectileFactory));
            }
            _player = player;
            _level = level;
            _playerMovement = playerMovement;
            _projectileFactory = projectileFactory;

            _player.FireballRequested += OnFireballRequested;
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

        public void ThrowFireball()
        {
            Player.ThrowFireball();
        }

        private void OnFireballRequested()
        {
            float direction;

            if (Player.FacingDirection == SpriteEffects.FlipHorizontally)
            {
                direction = 1f; // right
            }
            else
            {
                direction = -1f; // left
            }

            Vector2 position = new Vector2(Player.Position.X + direction * Player.Bounds.Width,
                Player.Position.Y + Player.Bounds.Height / 2);

            Vector2 velocity = new Vector2(direction * 200f, -100f);
            
            IProjectile fireball = _projectileFactory.Create(ProjectileType.Fireball, position, velocity);

            Level.AddProjectiles(fireball);
        }
    }
}
