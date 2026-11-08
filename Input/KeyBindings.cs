using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TeamUno.Mario.Input
{
    internal enum GameAction
    {
        MoveLeft, MoveRight, Jump, Crouch, Fireball, Pause, Mute, Restart
    }

    internal class KeyBindings
    {
        // keys the menu and fixed alternates already use
        private static readonly HashSet<Keys> Reserved = new HashSet<Keys>
        {
            Keys.Escape, Keys.Enter, Keys.Up, Keys.Down, Keys.Left, Keys.Right,
            Keys.Space, Keys.Q, Keys.F1, Keys.F2, Keys.F3, Keys.F4,
            Keys.F5, Keys.F6, Keys.F7, Keys.F8
        };

        private readonly Dictionary<GameAction, Keys> _keys = new Dictionary<GameAction, Keys>
        {
            { GameAction.MoveLeft, Keys.A },
            { GameAction.MoveRight, Keys.D },
            { GameAction.Jump, Keys.W },
            { GameAction.Crouch, Keys.S },
            { GameAction.Fireball, Keys.Z },
            { GameAction.Pause, Keys.P },
            { GameAction.Mute, Keys.M },
            { GameAction.Restart, Keys.R }
        };

        public event Action Changed;

        public Keys Get(GameAction action)
        {
            return _keys[action];
        }

        public bool Set(GameAction action, Keys key)
        {
            if (Reserved.Contains(key))
            {
                return false;
            }

            // swap with any action that already uses this key
            foreach (GameAction other in new List<GameAction>(_keys.Keys))
            {
                if (other != action && _keys[other] == key)
                {
                    _keys[other] = _keys[action];
                }
            }

            _keys[action] = key;
            if (Changed != null)
            {
                Changed();
            }

            return true;
        }
    }
}