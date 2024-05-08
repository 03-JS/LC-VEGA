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
}
