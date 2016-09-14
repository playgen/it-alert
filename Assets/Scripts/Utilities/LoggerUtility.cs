using System;
using UnityEngine;
using System.Collections;

public static class LoggerUtility
{
    public static event Action<string> LogErrorEvent;

    public static void LogError(string message)
    {
        if (LogErrorEvent != null)
        {
            LogErrorEvent(message);
        }
    }
}
