using UnityEngine;

public class GridHistoryNode
{
    private Color mColor;
    private Powerups.PowerupType mType;

    public GridHistoryNode(Color color, Powerups.PowerupType type)
    {
        mColor = color;
        mType = type;
    }

    public Color GetColor()
    {
        return mColor;
    }

    public Powerups.PowerupType GetTileType()
    {
        return mType;
    }
}