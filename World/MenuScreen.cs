using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TeamUno.Mario.Audio;
using TeamUno.Mario.Input;

namespace TeamUno.Mario.World
{
    internal class MenuScreen
    {
        private enum Page
        {
            Main,
            Levels,
            TestSpots,
            Volume,
            Keys
        }

        private static readonly string[] MainMenuLines =
        {
            "Resume",
            "Restart area",
            "Select level",
            "Test spots",
            "Volume",
            "Controls",
            "Title screen",
            "Quit"
        };
        private static readonly string[] TestSpawns =
        {
            "start", "items", "combat", "bowser", "pipes", "ledge"
        };
        private static readonly string[] TestLines =
        {
            "Block bumps", "Items and platforms", "Shells and enemies",
            "Bowser and fireballs", "Pipes, vines, and flag", "Floor gap"
        };

        private readonly GameSession _session;
        private readonly KeyBindings _bindings;
        private readonly List<string> _areaIds;
        private KeyboardState _previous;
        private GamePadState _previousGamePad;
        private Page _page = Page.Main;
        private int _selected;
        private bool _isRebinding;

        public MenuScreen(GameSession session, KeyBindings bindings, IEnumerable<string> areaIds)
        {
            _session = session;
            _bindings = bindings;
            _areaIds = new List<string>(areaIds);
        }

        public void Open(KeyboardState keys, GamePadState gamePad)
        {
            _page = Page.Main;
            _selected = 0;
            _isRebinding = false;
            _previous = keys;
            _previousGamePad = gamePad;
            _session.OpenMenu();
        }

        public void Update(KeyboardState keys, GamePadState gamePad)
        {
            if (_isRebinding)
            {
                UpdateRebinding(keys, gamePad);
            }
            else
            {
                UpdateNavigation(keys, gamePad);
            }

            _previous = keys;
            _previousGamePad = gamePad;
        }

        private bool Pressed(KeyboardState keys, Keys key)
        {
            return keys.IsKeyDown(key) && _previous.IsKeyUp(key);
        }

        private void UpdateRebinding(KeyboardState keys, GamePadState gamePad)
        {
            if (ButtonPressed(gamePad, Buttons.B))
            {
                _isRebinding = false;
                return;
            }

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

        private void UpdateNavigation(KeyboardState keys, GamePadState gamePad)
        {
            Keys pauseKey = _bindings.Get(GameAction.Pause);
            bool pauseKeyIsNavigation = pauseKey == Keys.W || pauseKey == Keys.A
                || pauseKey == Keys.S || pauseKey == Keys.D;
            if ((!pauseKeyIsNavigation && Pressed(keys, pauseKey)) || ButtonPressed(gamePad, Buttons.Start))
            {
                _session.CloseMenu();
                return;
            }

            int count = GetLines().Count;
            bool downPressed = Pressed(keys, Keys.Down) || Pressed(keys, Keys.S)
                || ButtonPressed(gamePad, Buttons.DPadDown)
                || AxisCrossed(gamePad.ThumbSticks.Left.Y, _previousGamePad.ThumbSticks.Left.Y, -0.5f);
            bool upPressed = Pressed(keys, Keys.Up) || Pressed(keys, Keys.W)
                || ButtonPressed(gamePad, Buttons.DPadUp)
                || AxisCrossed(gamePad.ThumbSticks.Left.Y, _previousGamePad.ThumbSticks.Left.Y, 0.5f);
            if (downPressed)
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                _selected = (_selected + 1) % count;
            }
            else if (upPressed)
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                _selected = (_selected + count - 1) % count;
            }

            bool adjustingVolume = _page == Page.Volume && _selected < 2;
            if (Pressed(keys, Keys.Escape) || ButtonPressed(gamePad, Buttons.B)
                || (!adjustingVolume && Pressed(keys, Keys.A)))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                GoBack();
                return;
            }

            int adjust = 0;
            if (Pressed(keys, Keys.Right) || Pressed(keys, Keys.D) || ButtonPressed(gamePad, Buttons.DPadRight)
                || AxisCrossed(gamePad.ThumbSticks.Left.X, _previousGamePad.ThumbSticks.Left.X, 0.5f))
            {
                adjust = 1;
            }

            if (Pressed(keys, Keys.Left) || Pressed(keys, Keys.A) || ButtonPressed(gamePad, Buttons.DPadLeft)
                || AxisCrossed(gamePad.ThumbSticks.Left.X, _previousGamePad.ThumbSticks.Left.X, -0.5f))
            {
                adjust = -1;
            }

            if (adjust != 0 && adjustingVolume)
            {
                AdjustVolume(adjust);
            }

            if (Pressed(keys, Keys.Enter) || ButtonPressed(gamePad, Buttons.A)
                || (!adjustingVolume && Pressed(keys, Keys.D)))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                Activate();
            }
        }

        private bool ButtonPressed(GamePadState gamePad, Buttons button)
        {
            return gamePad.IsConnected
                && gamePad.IsButtonDown(button)
                && (!_previousGamePad.IsConnected || _previousGamePad.IsButtonUp(button));
        }

        private static bool AxisCrossed(float current, float previous, float threshold)
        {
            if (threshold > 0)
            {
                return current >= threshold && previous < threshold;
            }

            return current <= threshold && previous > threshold;
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
                    ActivateMain();
                    break;
                case Page.Levels:
                    if (_selected < _areaIds.Count)
                    {
                        _session.CloseMenu();
                        _session.LoadArea(_areaIds[_selected]);
                    }
                    else
                    {
                        GoBack();
                    }
                    break;
                case Page.TestSpots:
                    if (_selected < TestSpawns.Length)
                    {
                        _session.CloseMenu();
                        _session.LoadArea("test", TestSpawns[_selected]);
                    }
                    else
                    {
                        GoBack();
                    }
                    break;
                case Page.Volume:
                    if (_selected == 2)
                    {
                        GoBack();
                    }
                    break;
                case Page.Keys:
                    if (_selected < System.Enum.GetValues<GameAction>().Length)
                    {
                        _isRebinding = true;
                    }
                    else
                    {
                        GoBack();
                    }
                    break;
            }
        }

        private void ActivateMain()
        {
            switch (_selected)
            {
                case 0:
                    _session.CloseMenu();
                    break;
                case 1:
                    _session.Reset();
                    break;
                case 2:
                    _page = Page.Levels;
                    break;
                case 3:
                    _page = Page.TestSpots;
                    break;
                case 4:
                    _page = Page.Volume;
                    break;
                case 5:
                    _page = Page.Keys;
                    break;
                case 6:
                    _session.ReturnToTitle();
                    break;
                case 7:
                    _session.Quit();
                    break;
            }

            _selected = 0;
        }

        // the renderer draws these, with ">" in front of the selected one
        public List<string> GetLines()
        {
            List<string> lines = new List<string>();
            switch (_page)
            {
                case Page.Main:
                    lines.AddRange(MainMenuLines);
                    break;
                case Page.Levels:
                    lines.AddRange(_areaIds);
                    lines.Add("Back");
                    break;
                case Page.TestSpots:
                    lines.AddRange(TestLines);
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
                        if (_isRebinding && (int)action == _selected)
                        {
                            key = "press a key...";
                        }
                        lines.Add(action + ": " + key);
                    }
                    lines.Add("Back");
                    break;
            }

            return lines;
        }

        public string Title
        {
            get
            {
                switch (_page)
                {
                    case Page.Levels:
                        return "Select level";
                    case Page.TestSpots:
                        return "Test spots";
                    case Page.Volume:
                        return "Volume";
                    case Page.Keys:
                        return "Controls";
                    default:
                        return "Paused";
                }
            }
        }

        public string Instructions
        {
            get
            {
                if (_isRebinding)
                {
                    return "Press a new key   Esc / Pad B: cancel";
                }

                if (_page == Page.Volume)
                {
                    return "W/S or arrows: choose   A/D or arrows: adjust   Esc / Pad B: back";
                }

                return "W/S or arrows: choose   D/Enter/Pad A: select   A/Esc/Pad B: back";
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selected;
            }
        }
    }
}