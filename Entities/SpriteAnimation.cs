using System;
using System.Collections.Generic;

namespace Sprint0.Entities
{
    public class SpriteAnimation
    {
        public float FrameDuration { get; }
        public IReadOnlyList<SpriteFrame> Frames { get; }

        public SpriteAnimation(float frameDuration, params SpriteFrame[] frames)
        {
            if (frameDuration <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frameDuration));
            }

            if (frames == null || frames.Length == 0)
            {
                throw new ArgumentException("An animation needs at least one frame.", nameof(frames));
            }

            FrameDuration = frameDuration;
            // Keep our own copy of the frames.
            Frames = new List<SpriteFrame>(frames).AsReadOnly();
        }
    }
}
