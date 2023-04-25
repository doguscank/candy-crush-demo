using System.Collections;
using UnityEngine;

public class TileColors
{
    public static Color[] Colors = new Color[]
    {
        Color.red,
        Color.green,
        Color.yellow,
        Color.cyan,
        Color.black
    };

    public static Color GetRandomColor()
    {
        int index = Random.Range(0, TileColors.Colors.Length);
        return TileColors.Colors[index];
    }
}