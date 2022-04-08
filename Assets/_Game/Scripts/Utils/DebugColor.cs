using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStringers.Framework;
using UnityEngine.UI;

public class DebugColor : ManualSingleton<DebugColor>
{
    public GameObject ObjectDebug;

    public override void Awake()
    {
        base.Awake();
#if TESTING
        this.ObjectDebug.SetActive(true);
#else
        this.ObjectDebug.SetActive(false);
        if (this.ObjectDebug.gameObject != null) Destroy(this.ObjectDebug);
#endif

    }

    // public static bool _isShowDebug;
    /// <summary>
    /// Show Debug with Chosen Color
    /// </summary>
    /// <param name="strContent">Debug Text</param>
    /// <param name="color"> color = ColorName.Red/ColorName.Green....</param>
    public static void Log(string strContent, ColorName color)
    {
        //if (!_isShowDebug) return;
#if TESTING
        Debug.Log(strContent % color);
#endif
    }

    public static void Log(string strContent, Color color)
    {
        var colorName = new ColorName(color);
#if TESTING
        Log(strContent, colorName);
#endif
    }

    /// <summary>
    /// Show Debug with Hex Color
    /// </summary>
    /// <param name="strContent">Debug Text</param>
    /// <param name="hexColor">hex color with #</param>
    public static void Log(string strContent, string hexColor)
    {
        //if (!_isShowDebug) return;
        var color = new ColorName(hexColor);
#if TESTING
        Debug.Log(strContent % color);
#endif
    }
}

public class ColorName
{

    public static ColorName Red = new ColorName(Color.red);
    public static ColorName Yellow = new ColorName(Color.yellow);
    public static ColorName Green = new ColorName(Color.green);
    public static ColorName Cyan = new ColorName(Color.cyan);

    public static ColorName colorByHex;

    private readonly string _prefix;
    private const string _subffix = "</color>";


    public ColorName() { }
    public ColorName(Color color)
    {
        _prefix = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
    }

    public ColorName(string hexColor)
    {
        _prefix = $"<color={hexColor}>";
    }

    public static string operator %(string strContent, ColorName color)
    {
        return color._prefix + strContent + _subffix;
    }
}
