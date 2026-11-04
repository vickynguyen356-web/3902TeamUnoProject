using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace TeamUno.Mario.Audio
{
    internal static class SoundPlayer
    {
        private static readonly Dictionary<string, SoundEffect> _effects =
            new Dictionary<string, SoundEffect>();
        private static ContentManager _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
        }

        public static void Play(string name)
        {
            if (_content == null)
            {
                return;   // not initialized (e.g. in unit tests): do nothing
            }

            SoundEffect effect;
            if (!_effects.TryGetValue(name, out effect))
            {
                effect = _content.Load<SoundEffect>(name);
                _effects[name] = effect;
            }

            effect.Play();
        }
    }
}