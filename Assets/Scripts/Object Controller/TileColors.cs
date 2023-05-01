using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColors
{
    public static List<Color> Colors = new List<Color>
    {
        Color.red,
        Color.green,
        Color.yellow,
        Color.cyan,
        // Color.black
    };

    public static Color GetRandomColor()
    {
        int index = Random.Range(0, TileColors.Colors.Count);
        return TileColors.Colors[index];
    }
}