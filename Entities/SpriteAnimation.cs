using System;
using System.Collections.Generic;

namespace TeamUno.Mario.Entities
{
    internal class SpriteAnimation
    {
        private readonly float _frameDuration;
        private readonly IReadOnlyList<SpriteFrame> _frames;

        public float FrameDuration
        {
            get
            {
                return _frameDuration;
            }
        }

        public IReadOnlyList<SpriteFrame> Frames
        {
            get
            {
                return _frames;
            }
        }

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

            _frameDuration = frameDuration;
            // Copy the frames so callers cannot change this animation
            _frames = new List<SpriteFrame>(frames).AsReadOnly();
        }
    }
}
