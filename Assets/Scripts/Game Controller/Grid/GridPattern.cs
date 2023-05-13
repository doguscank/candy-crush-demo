using System.Collections.Generic;

public class GridPattern
{
    private Powerups.PowerupType mType;
    private List<(int, int)> mElements;

    public GridPattern(Powerups.PowerupType type, List<(int, int)> elements)
    {
        mType = type;
        mElements = elements;
    }

    public Powerups.PowerupType GetTileType()
    {
        return mType;
    }

    public List<(int, int)> GetElements()
    {
        return mElements;
    }

    public (int, int) GetElement(int idx)
    {
        return mElements[idx];
    }
}