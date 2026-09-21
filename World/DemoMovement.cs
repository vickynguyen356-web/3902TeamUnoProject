using System;
using Microsoft.Xna.Framework;
using Sprint0.Entities;
using Sprint0.Interfaces;

namespace Sprint0.World
{
    // Temporary floor and window limits for the movement demo
    public class DemoMovement : IPlayerMovement
    {
        private readonly int _stageWidth;
        public int FloorY { get; }

        public DemoMovement(int stageWidth, int floorY)
        {
            if (stageWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stageWidth));
            }

            _stageWidth = stageWidth;
            FloorY = floorY;
        }

        public void Move(MarioPlayer player, float elapsedSeconds)
        {
            Vector2 velocity = player.Velocity;
            Vector2 position = player.Position + velocity * elapsedSeconds;

            // Keep the whole body inside the window.
            float maximumPlayerX = Math.Max(0, _stageWidth - player.Bounds.Width);
            float clampedPlayerX = MathHelper.Clamp(position.X, 0, maximumPlayerX);
            if (position.X != clampedPlayerX)
            {
                position.X = clampedPlayerX;
                velocity.X = 0;
            }

            // Use the crouching or standing height when landing.
            float playerTopAtFloor = FloorY - player.Bounds.Height;
            bool isGrounded = velocity.Y >= 0 && position.Y >= playerTopAtFloor;
            if (isGrounded)
            {
                position.Y = playerTopAtFloor;
                velocity.Y = 0;
            }

            player.ApplyMotion(position, velocity, isGrounded);
        }
    }
}
