using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal class DemoMovement : IPlayerMovement
    {
        private readonly int _stageWidth;
        private readonly int _floorY;

        public int FloorY
        {
            get
            {
                return _floorY;
            }
        }

        public DemoMovement(int stageWidth, int floorY)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stageWidth);

            _stageWidth = stageWidth;
            _floorY = floorY;
        }

        public void Move(MarioPlayer player, float elapsedSeconds)
        {
            Vector2 velocity = player.Velocity;
            Vector2 position = player.Position + velocity * elapsedSeconds;

            float maximumPlayerX = Math.Max(0, _stageWidth - player.Bounds.Width);
            float clampedPlayerX = MathHelper.Clamp(position.X, 0, maximumPlayerX);
            if (position.X != clampedPlayerX)
            {
                position.X = clampedPlayerX;
                velocity.X = 0;
            }

            // Use the crouching or standing height when landing
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
