using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TeamUno.Mario.Audio;
using TeamUno.Mario.Input;

namespace TeamUno.Mario.World
{
    internal class MenuScreen
    {
        private enum Page { Main, Levels, Volume, Keys }

        private readonly GameSession _session;
        private readonly KeyBindings _bindings;
        private readonly List<string> _areaIds;
        private KeyboardState _previous;
        private Page _page = Page.Main;
        private int _selected;
        private bool _isRebinding;

        public MenuScreen(GameSession session, KeyBindings bindings, IEnumerable<string> areaIds)
        {
            _session = session;
            _bindings = bindings;
            _areaIds = new List<string>(areaIds);
        }

        public void Open()
        {
            _page = Page.Main;
            _selected = 0;
            _isRebinding = false;
            _previous = Keyboard.GetState();
            _session.OpenMenu();
        }

        public void Update()
        {
            KeyboardState keys = Keyboard.GetState();
            if (_isRebinding)
            {
                UpdateRebinding(keys);
            }
            else
            {
                UpdateNavigation(keys);
            }

            _previous = keys;
        }

        private bool Pressed(KeyboardState keys, Keys key)
        {
            return keys.IsKeyDown(key) && _previous.IsKeyUp(key);
        }

        private void UpdateRebinding(KeyboardState keys)
        {
            foreach (Keys key in keys.GetPressedKeys())
            {
                if (_previous.IsKeyUp(key))
                {
                    if (key != Keys.Escape)
                    {
                        if (!_bindings.Set((GameAction)_selected, key))
                        {
                            return;   // reserved key: keep waiting for another one
                        }
                    }

                    _isRebinding = false;
                    return;
                }
            }
        }

        private void UpdateNavigation(KeyboardState keys)
        {
            int count = GetLines().Count;
            if (Pressed(keys, Keys.Down))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                _selected = (_selected + 1) % count;
            }
            else if (Pressed(keys, Keys.Up))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                _selected = (_selected + count - 1) % count;
            }

            if (Pressed(keys, Keys.Escape))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                GoBack();
                return;
            }

            int adjust = 0;
            if (Pressed(keys, Keys.Right)) { adjust = 1; }
            if (Pressed(keys, Keys.Left)) { adjust = -1; }
            if (adjust != 0 && _page == Page.Volume)
            {
                AdjustVolume(adjust);
            }

            if (Pressed(keys, Keys.Enter))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                Activate();
            }
        }

        private void GoBack()
        {
            if (_page == Page.Main)
            {
                _session.CloseMenu();
                return;
            }

            _page = Page.Main;
            _selected = 0;
        }

        private void AdjustVolume(int direction)
        {
            float step = 0.1f * direction;
            if (_selected == 0)
            {
                MediaPlayer.Volume = Microsoft.Xna.Framework.MathHelper.Clamp(
                    MediaPlayer.Volume + step, 0f, 1f);
            }
            else if (_selected == 1)
            {
                SoundEffect.MasterVolume = Microsoft.Xna.Framework.MathHelper.Clamp(
                    SoundEffect.MasterVolume + step, 0f, 1f);
            }
        }

        private void Activate()
        {
            switch (_page)
            {
                case Page.Main:
                    if (_selected == 0) { _session.CloseMenu(); }
                    else { _page = (Page)_selected; _selected = 0; }
                    break;
                case Page.Levels:
                    if (_selected < _areaIds.Count)
                    {
                        _session.LoadArea(_areaIds[_selected]);   // also leaves the Menu state
                    }
                    else { GoBack(); }
                    break;
                case Page.Volume:
                    if (_selected == 2) { GoBack(); }
                    break;
                case Page.Keys:
                    if (_selected < System.Enum.GetValues<GameAction>().Length)
                    {
                        _isRebinding = true;
                    }
                    else { GoBack(); }
                    break;
            }
        }

        // the renderer draws these, with ">" in front of the selected one
        public List<string> GetLines()
        {
            List<string> lines = new List<string>();
            switch (_page)
            {
                case Page.Main:
                    lines.AddRange(new[] { "Resume", "Select level", "Volume", "Controls" });
                    break;
                case Page.Levels:
                    lines.AddRange(_areaIds);
                    lines.Add("Back");
                    break;
                case Page.Volume:
                    lines.Add("Music: " + (int)(MediaPlayer.Volume * 100) + "%");
                    lines.Add("Effects: " + (int)(SoundEffect.MasterVolume * 100) + "%");
                    lines.Add("Back");
                    break;
                case Page.Keys:
                    foreach (GameAction action in System.Enum.GetValues<GameAction>())
                    {
                        string key = _bindings.Get(action).ToString();
                        if (_isRebinding && (int)action == _selected) { key = "press a key..."; }
                        lines.Add(action + ": " + key);
                    }
                    lines.Add("Q: quit");
                    lines.Add("Back");
                    break;
            }

            return lines;
        }

        public int SelectedIndex
        {
            get { return _selected; }
        }
    }
}