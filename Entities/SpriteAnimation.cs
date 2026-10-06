using System;
using System.Collections.Generic;

namespace TeamUno.Mario.Entities
{
    public class SpriteAnimation
    {
        private readonly float _frameDuration;
        private readonly bool _isLooping;
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

        public bool IsLooping
        {
            get
            {
                return _isLooping;
            }
        }

        public SpriteAnimation(float frameDuration, params SpriteFrame[] frames)
            : this(frameDuration, true, frames)
        {
        }

        public SpriteAnimation(float frameDuration, bool isLooping, params SpriteFrame[] frames)
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
            _isLooping = isLooping;
            _frames = new List<SpriteFrame>(frames).AsReadOnly();
        }
    }
}
