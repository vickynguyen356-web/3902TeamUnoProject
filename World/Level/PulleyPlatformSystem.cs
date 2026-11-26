using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TeamUno.Mario.World
{
    internal static class PulleyPlatformSystem
    {
        public static void Update(
            GameTime gameTime,
            IReadOnlyList<PulleyPlatformPair> platforms,
            IList<Rectangle> ropes)
        {
            foreach (PulleyPlatformPair platform in platforms)
            {
                platform.Update(gameTime);
            }

            UpdateRopes(platforms, ropes);
        }

        public static void UpdateRopes(
            IReadOnlyList<PulleyPlatformPair> platforms,
            IList<Rectangle> ropes)
        {
            ropes.Clear();
            foreach (PulleyPlatformPair platform in platforms)
            {
                Rectangle leftRope = platform.LeftRope;
                Rectangle rightRope = platform.RightRope;
                if (leftRope.Height > 0)
                {
                    ropes.Add(leftRope);
                }

                if (rightRope.Height > 0)
                {
                    ropes.Add(rightRope);
                }
            }
        }
    }
}
