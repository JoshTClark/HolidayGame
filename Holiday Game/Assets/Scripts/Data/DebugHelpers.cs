using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugHelpers
{
    private static bool debugMode = true;

    /// <summary>
    /// True if the game currently in debug mode
    /// Debug mode enables developer tools
    /// </summary>
    public static bool DEBUG
    {
        get { return debugMode; }
    }

    /// <summary>
    /// Toggles debug mode on and off
    /// </summary>
    public static void ToggleDebugMode()
    {
        debugMode = !debugMode;
    }
}
