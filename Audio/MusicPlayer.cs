using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace TeamUno.Mario.Audio
{
    internal class MusicPlayer
    {
        private readonly Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private readonly Dictionary<string, SoundEffect> _effects = new Dictionary<string, SoundEffect>();
        private readonly ContentManager _content;
        private string _currentTrack;
        private SoundEffectInstance _interrupt;

        public MusicPlayer(ContentManager content)
        {
            _content = content;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
        }

        public void Play(string trackName, bool restart = false)
        {
            if (trackName == null)
            {
                Stop();
                return;
            }

            // don't restart if the same track is already playing
            if (!restart && trackName == _currentTrack
                && (MediaPlayer.State == MediaState.Playing || _interrupt != null))
            {
                return;
            }

            Song song;
            if (!_songs.TryGetValue(trackName, out song))
            {
                song = _content.Load<Song>(trackName);
                _songs[trackName] = song;
            }

            MediaPlayer.Play(song);
            if (_interrupt != null)
            {
                MediaPlayer.Pause(); // area's song waits until invincibility ends
            }

            _currentTrack = trackName;
        }

        public void Stop()
        {
            ClearInterrupt();
            MediaPlayer.Stop();
            _currentTrack = null;
        }

        public void Pause()
        {
            if (_interrupt != null)
            {
                _interrupt.Pause();
            }
            else
            {
                MediaPlayer.Pause();
            }
        }

        public void Resume()
        {
            if (_interrupt != null)
            {
                _interrupt.Resume();
            }
            else
            {
                MediaPlayer.Resume();
            }
        }

        public void PlayInterrupt(string name)
        {
            if (_interrupt != null)
            {
                return;
            }

            SoundEffect effect;
            if (!_effects.TryGetValue(name, out effect))
            {
                effect = _content.Load<SoundEffect>(name);
                _effects[name] = effect;
            }

            MediaPlayer.Pause();
            _interrupt = effect.CreateInstance();
            _interrupt.IsLooped = true;
            _interrupt.Play();
        }

        // stops the theme and resumes the level song where it left off
        public void StopInterrupt()
        {
            if (_interrupt == null)
            {
                return;
            }

            ClearInterrupt();
            MediaPlayer.Resume();
        }

        private void ClearInterrupt()
        {
            if (_interrupt != null)
            {
                _interrupt.Stop();
                _interrupt.Dispose();
                _interrupt = null;
            }
        }
    }
}
