using System.Collections;
using UnityEngine;

public static class ColorExtensions
{
    /// <summary>
    /// Returns a darker color from base.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Darker(this Color colorbase)
    {
        return Color.Lerp(colorbase, Color.black, 0.5f);
    }

    /// <summary>
    /// Returns a darker color from base.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Darker(this Color colorbase, float intensity)
    {
        return Color.Lerp(colorbase, Color.black, intensity);
    }
}