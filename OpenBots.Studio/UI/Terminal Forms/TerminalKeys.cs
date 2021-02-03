﻿using Open3270;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenBots.Commands.Terminal.Library
{
    public static class TerminalKeys
    {
        public static Dictionary<Keys, TnKey> TerminalKeysDict = new Dictionary<Keys, TnKey>()
        {
            { Keys.F1, TnKey.F1 },
            { Keys.F2, TnKey.F2 },
            { Keys.F3, TnKey.F3 },
            { Keys.F4, TnKey.F4 },
            { Keys.F5, TnKey.F5 },
            { Keys.F6, TnKey.F6 },
            { Keys.F7, TnKey.F7 },
            { Keys.F8, TnKey.F8 },
            { Keys.F9, TnKey.F9 },
            { Keys.F10, TnKey.F10 },
            { Keys.F11, TnKey.F11 },
            { Keys.F12, TnKey.F12 },
            { Keys.F13, TnKey.F13 },
            { Keys.F14, TnKey.F14 },
            { Keys.F15, TnKey.F15 },
            { Keys.F16, TnKey.F16 },
            { Keys.F17, TnKey.F17 },
            { Keys.F18, TnKey.F18 },
            { Keys.F19, TnKey.F19 },
            { Keys.F20, TnKey.F20 },
            { Keys.F21, TnKey.F21 },
            { Keys.F22, TnKey.F22 },
            { Keys.F23, TnKey.F23 },
            { Keys.F24, TnKey.F24 },
            { Keys.Tab, TnKey.Tab },
            { Keys.Enter, TnKey.Enter },
            { Keys.Back, TnKey.Backspace },
            { Keys.Clear, TnKey.Clear },
            { Keys.Delete, TnKey.Delete },
            { Keys.Left, TnKey.Left },
            { Keys.Up, TnKey.Up },
            { Keys.Right, TnKey.Right },
            { Keys.Down, TnKey.Down },
            { Keys.Attn, TnKey.Attn },
            { Keys.Home, TnKey.Home },
            { Keys.Insert, TnKey.Insert }
        };
    }
}
