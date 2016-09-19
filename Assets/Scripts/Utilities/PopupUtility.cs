using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public static class PopupUtility
{
    public static event Action<string> LogErrorEvent;
    public static event Action StartLoadingEvent;
    public static event Action EndLoadingEvent;
    public static event Action<Color> ColorPickerEvent;

    public static PopupController PopupController;

    public static void LogError(string message)
    {
        if (LogErrorEvent != null)
        {
            LogErrorEvent(message);
        }
    }
    public static void ShowLoadingPopup()
    {
        if (StartLoadingEvent != null)
        {
            StartLoadingEvent();
        }
    }

    public static void HideLoadingPopup()
    {
        if (EndLoadingEvent != null)
        {
            EndLoadingEvent();
        }
    }

    public static void ShowColorPicker(Action<Color> callbackAction)
    {
        //get the controller
        PopupController.ShowColorPickerPopup(callbackAction);
    }
}
