using System;
using System.Collections.Generic;
using System.Text;

namespace LC_VEGA
{
    public enum VocalLevels
    {
        None = 0, // Only speaks when asked to. Doesn't give any extra info.
        Low = 1, // Gives you useful info, doesn't talk on interactions and new enemy scans.
        Medium = 2, // Gives you useful info, doesn't talk on interactions.
        High = 3 // The default. Will talk any time it can. Intended for less experienced players.
    }

    public enum Colors
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
        Orange = 4,
        Pink = 5,
        Purple = 6,
        Magenta = 7,
        Cyan = 8,
        Crimson = 9,
        White = 10,
        Gray = 11,
        Black = 12,
        LunxaraPink = 13,
        LunxaraPurple = 14,
        LunxaraBlue = 15,
        LyraGreen = 16
    }
}
