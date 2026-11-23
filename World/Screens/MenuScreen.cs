using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly List<LevelSelectionColumn> _levelColumns;
        private KeyboardState _previous;
        private GamePadState _previousGamePad;
        private Page _page = Page.Main;
        private int _selected;
        private bool _isRebinding;
        private int _heldDirection;
        private float _navigationRepeatRemaining;

        private const float InitialNavigationRepeatDelay = 0.35f;
        private const float NavigationRepeatInterval = 0.12f;

        private sealed class LevelSelectionColumn
        {
            public string Title { get; }
            public List<int> AreaIndices { get; } = new List<int>();

            public LevelSelectionColumn(string title)
            {
                Title = title;
            }
        }

        public MenuScreen(GameSession session, KeyBindings bindings, IEnumerable<string> areaIds)
        {
            _session = session;
            _bindings = bindings;
            _areaIds = new List<string>(areaIds);
            _areaIds.RemoveAll(areaId =>
                !string.Equals(areaId, "World1-1", System.StringComparison.OrdinalIgnoreCase)
                && !string.Equals(areaId, "overworld", System.StringComparison.OrdinalIgnoreCase)
                && !string.Equals(areaId, "underground", System.StringComparison.OrdinalIgnoreCase)
                && !string.Equals(areaId, "underwater", System.StringComparison.OrdinalIgnoreCase)
                && !string.Equals(areaId, "test", System.StringComparison.OrdinalIgnoreCase)
                && !string.Equals(areaId, "bowser-party", System.StringComparison.OrdinalIgnoreCase));
            _levelColumns = BuildLevelColumns(_areaIds);
        }

        public void Open(KeyboardState keys, GamePadState gamePad)
        {
            _page = Page.Main;
            _selected = 0;
            _isRebinding = false;
            _heldDirection = 0;
            _navigationRepeatRemaining = 0f;
            _previous = keys;
            _previousGamePad = gamePad;
            _session.OpenMenu();
        }

        public void Update(GameTime gameTime, KeyboardState keys, GamePadState gamePad)
        {
            if (_isRebinding)
            {
                UpdateRebinding(keys, gamePad);
            }
            else
            {
                UpdateNavigation(gameTime, keys, gamePad);
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

        private void UpdateNavigation(GameTime gameTime, KeyboardState keys, GamePadState gamePad)
        {
            Keys pauseKey = _bindings.Get(GameAction.Pause);
            bool pauseKeyIsNavigation = pauseKey == Keys.W || pauseKey == Keys.A
                || pauseKey == Keys.S || pauseKey == Keys.D;
            if ((!pauseKeyIsNavigation && Pressed(keys, pauseKey)) || ButtonPressed(gamePad, Buttons.Start))
            {
                _session.CloseMenu();
                return;
            }

            bool upHeld = keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W)
                || gamePad.IsButtonDown(Buttons.DPadUp) || gamePad.ThumbSticks.Left.Y >= 0.5f;
            bool downHeld = keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S)
                || gamePad.IsButtonDown(Buttons.DPadDown) || gamePad.ThumbSticks.Left.Y <= -0.5f;
            bool leftHeld = _page == Page.Levels
                && (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A)
                    || gamePad.IsButtonDown(Buttons.DPadLeft) || gamePad.ThumbSticks.Left.X <= -0.5f);
            bool rightHeld = _page == Page.Levels
                && (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D)
                    || gamePad.IsButtonDown(Buttons.DPadRight) || gamePad.ThumbSticks.Left.X >= 0.5f);
            int direction = upHeld ? -1 : downHeld ? 1 : leftHeld ? -2 : rightHeld ? 2 : 0;
            UpdateNavigationRepeat(gameTime, direction);

            bool adjustingVolume = _page == Page.Volume && _selected < 2;
            if (Pressed(keys, Keys.Escape) || ButtonPressed(gamePad, Buttons.B)
                || (!adjustingVolume && _page != Page.Levels && Pressed(keys, Keys.A)))
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
                || (!adjustingVolume && _page != Page.Levels && Pressed(keys, Keys.D)))
            {
                SoundPlayer.Play("Audio/SoundEffects/beep");
                Activate();
            }
        }

        private void UpdateNavigationRepeat(GameTime gameTime, int direction)
        {
            if (direction == 0)
            {
                _heldDirection = 0;
                _navigationRepeatRemaining = 0f;
                return;
            }

            bool shouldMove = direction != _heldDirection;
            if (shouldMove)
            {
                _navigationRepeatRemaining = InitialNavigationRepeatDelay;
            }
            else
            {
                _navigationRepeatRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_navigationRepeatRemaining <= 0f)
                {
                    shouldMove = true;
                    _navigationRepeatRemaining = NavigationRepeatInterval;
                }
            }

            _heldDirection = direction;
            if (!shouldMove)
            {
                return;
            }

            SoundPlayer.Play("Audio/SoundEffects/beep");
            if (_page == Page.Levels)
            {
                MoveLevelSelection(direction);
                return;
            }

            int count = GetLines().Count;
            _selected = direction < 0
                ? (_selected + count - 1) % count
                : (_selected + 1) % count;
        }

        private void MoveLevelSelection(int direction)
        {
            if (_levelColumns.Count == 0)
            {
                return;
            }

            if (_selected == _areaIds.Count)
            {
                if (direction == -1)
                {
                    List<int> lastColumn = _levelColumns[_levelColumns.Count - 1].AreaIndices;
                    if (lastColumn.Count > 0)
                    {
                        _selected = lastColumn[lastColumn.Count - 1];
                    }
                }
                else if (direction < 0 && _levelColumns.Count > 1)
                {
                    List<int> previousColumn = _levelColumns[_levelColumns.Count - 2].AreaIndices;
                    if (previousColumn.Count > 0)
                    {
                        _selected = previousColumn[previousColumn.Count - 1];
                    }
                }
                return;
            }

            int columnIndex = FindSelectedColumn();
            List<int> column = _levelColumns[columnIndex].AreaIndices;
            int row = column.IndexOf(_selected);
            bool isHorizontal = direction == -2 || direction == 2;
            if (isHorizontal)
            {
                int nextColumnIndex = columnIndex + (direction < 0 ? -1 : 1);
                if (nextColumnIndex < 0 || nextColumnIndex >= _levelColumns.Count)
                {
                    return;
                }

                List<int> nextColumn = _levelColumns[nextColumnIndex].AreaIndices;
                if (nextColumn.Count > 0)
                {
                    _selected = nextColumn[Math.Min(row, nextColumn.Count - 1)];
                }
                return;
            }

            if (direction > 0 && row == column.Count - 1
                && columnIndex == _levelColumns.Count - 1)
            {
                _selected = _areaIds.Count;
            }
            else if (direction < 0 && row == 0)
            {
                _selected = column[column.Count - 1];
            }
            else if (direction > 0 && row == column.Count - 1)
            {
                _selected = column[0];
            }
            else
            {
                int nextRow = row + (direction < 0 ? -1 : 1);
                _selected = column[nextRow];
            }
        }

        private int FindSelectedColumn()
        {
            for (int columnIndex = 0; columnIndex < _levelColumns.Count; columnIndex++)
            {
                if (_levelColumns[columnIndex].AreaIndices.Contains(_selected))
                {
                    return columnIndex;
                }
            }

            return _levelColumns.Count - 1;
        }

        private static List<LevelSelectionColumn> BuildLevelColumns(List<string> areaIds)
        {
            SortedDictionary<int, LevelSelectionColumn> worlds =
                new SortedDictionary<int, LevelSelectionColumn>();
            List<int> otherAreas = new List<int>();
            for (int index = 0; index < areaIds.Count; index++)
            {
                string areaId = areaIds[index];
                int separator = areaId.IndexOf('-', StringComparison.Ordinal);
                int world;
                if (!areaId.StartsWith("World", StringComparison.Ordinal)
                    || separator <= 5
                    || !int.TryParse(
                        areaId.AsSpan(5, separator - 5),
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out world))
                {
                    otherAreas.Add(index);
                    continue;
                }

                LevelSelectionColumn column;
                if (!worlds.TryGetValue(world, out column))
                {
                    column = new LevelSelectionColumn("World " + world.ToString(CultureInfo.InvariantCulture));
                    worlds.Add(world, column);
                }
                column.AreaIndices.Add(index);
            }

            List<LevelSelectionColumn> columns = new List<LevelSelectionColumn>(worlds.Values);
            foreach (LevelSelectionColumn column in columns)
            {
                column.AreaIndices.Sort((first, second) =>
                    GetAreaNumber(areaIds[first]).CompareTo(GetAreaNumber(areaIds[second])));
            }
            if (otherAreas.Count > 0)
            {
                columns.Add(new LevelSelectionColumn("Other"));
                columns[columns.Count - 1].AreaIndices.AddRange(otherAreas);
            }
            return columns;
        }

        private static int GetAreaNumber(string areaId)
        {
            int separator = areaId.IndexOf('-', StringComparison.Ordinal);
            int level;
            return separator >= 0 && int.TryParse(
                areaId.AsSpan(separator + 1),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out level)
                ? level
                : int.MaxValue;
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

            _selected = _page == Page.Levels && _levelColumns.Count > 0
                && _levelColumns[0].AreaIndices.Count > 0
                ? _levelColumns[0].AreaIndices[0]
                : 0;
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
                    foreach (string areaId in _areaIds)
                    {
                        lines.Add(GetLevelLabel(areaId));
                    }
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

                if (_page == Page.Levels)
                {
                    return "Arrows / WASD / Pad: move   Enter / Pad A: select   Esc / Pad B: back";
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

        public bool IsLevelSelection
        {
            get { return _page == Page.Levels; }
        }

        public int LevelColumnCount
        {
            get { return _levelColumns.Count; }
        }

        public IReadOnlyList<int> GetLevelColumnIndices(int columnIndex)
        {
            return _levelColumns[columnIndex].AreaIndices.AsReadOnly();
        }

        public string GetLevelColumnTitle(int columnIndex)
        {
            return _levelColumns[columnIndex].Title;
        }

        public string GetLevelLabel(int areaIndex)
        {
            return GetLevelLabel(_areaIds[areaIndex]);
        }

        private static string GetLevelLabel(string areaId)
        {
            return areaId.StartsWith("World", System.StringComparison.Ordinal)
                ? areaId.Substring(5)
                : areaId;
        }
    }
}