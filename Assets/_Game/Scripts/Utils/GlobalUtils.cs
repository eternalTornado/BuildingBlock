using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUtils : MonoBehaviour
{
    public static bool IsEditor() {
        return Application.isEditor;
    }

    public static bool IsAndroidOrIOS() {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public static bool IsAndroid() {
        return Application.platform == RuntimePlatform.Android;
    }

    public static bool IsIOS() {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public static Color GetColorByStringHex(string hex) {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    /// <summary>
    /// Return string time with format : mm:ss
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns></returns>
    public static string GetStringTimeFromSecond(float time)
    {
        int minute = (int)time / 60;
        int second = (int)time - minute * 60;

        return $"{minute.ToString("##.")}:{second.ToString("##.")}";
    }
}
