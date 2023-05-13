using System.Collections.Generic;
using UnityEngine;

public class TileColors
{
    public static List<Color> Colors = new List<Color>
    {
        Color.green,
        Color.blue,
        Color.red,
        Color.yellow,
        Color.gray,
        Color.magenta,
        Color.cyan,
        Color.black
    };

    public static readonly Color ColorRemoverColor = Color.white;

    public static Color GetRandomColor()
    {
        int index = Random.Range(0, Mathf.Min(GameConfig.MaxColorsCount, TileColors.Colors.Count));
        return TileColors.Colors[index];
    }
}