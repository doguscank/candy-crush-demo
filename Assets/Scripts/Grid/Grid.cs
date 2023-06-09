using System.Collections.Generic;
using System;
using UnityEngine;

public class Grid
{
    private float minXPos;
    private float maxYPos;

    private GridPatterns mGridPatterns;

    [SerializeField] private GameObject[,] mGrid;

    public Grid()
    {
        mGridPatterns = new GridPatterns();

        minXPos = -GameConfig.TileSpacing * (int)(GameConfig.Cols / 2);
        maxYPos = GameConfig.TileSpacing * (int)(GameConfig.Rows / 2);

        mGrid = new GameObject[GameConfig.Rows, GameConfig.Cols];
    }

    public GameObject[,] GetGrid()
    {
        return mGrid;
    }

    public GridHistoryNode[,] GetDebugGrid()
    {
        GridHistoryNode[,] debugGrid = new GridHistoryNode[GameConfig.Rows, GameConfig.Cols];

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                var baseTile = mGrid[i, j].GetComponent<BaseTile>();
                debugGrid[i, j] = new GridHistoryNode(baseTile.GetColor(), baseTile.GetTileType());
            }
        }

        return debugGrid;
    }

    public void InitializeGrid()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                CreateTileAt(i, j);
            }
        }

        Debug.Log("Initialized grid.");
    }

    public void ClearGrid()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                mGrid[i, j].GetComponent<BaseTile>().DestroyImmediate();
            }
        }

        Debug.Log("Cleaned grid");
    }

    public GameObject CreateTileAt(int row, int col, int dropDepth = 0)
    {
        if (row >= GameConfig.Rows || row < 0 || col >= GameConfig.Cols || col < 0)
            return null;

        GameObject newTile = TileSpawnManager.Instance.SpawnTile(Powerups.PowerupType.NoPowerup);
        var script = newTile.GetComponent<BaseTile>();
        script.SetRandomColor();
        script.SetPosition(new Vector3(
            minXPos + col * GameConfig.TileSpacing,
            maxYPos - (row - dropDepth) * GameConfig.TileSpacing,
            0f
        ));
        script.SetCoords(row, col);
        script.IncreaseDropDepth(dropDepth);

        mGrid[row, col] = newTile;

        Debug.Log($"Created new tile at ({row},{col}) with color({script.GetColor()})");

        return newTile;
    }

    public bool CheckMatches()
    {
        Debug.Log("CheckMatches triggered.");

        bool isMatched = false;

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                BaseTile tile = mGrid[i, j].GetComponent<BaseTile>();
                if (tile.GetIsSelected() && tile.GetTileType() == Powerups.PowerupType.ColorRemover)
                {
                    isMatched = true;
                }
            }
        }

        bool IsColorMatched(Color color, int row, int col)
        {
            return mGrid[row, col].GetComponent<BaseTile>().GetColor() == color;
        }

        bool IsInBound(int row, int col)
        {
            return row >= 0 && row < GameConfig.Rows && col >= 0 && col < GameConfig.Cols;
        }

        bool IsTileMarked(int row, int col)
        {
            return mGrid[row, col].GetComponent<BaseTile>().GetMarked();
        }

        bool IsPatternMatched(GridPattern pattern, Color color, int row, int col)
        {
            foreach (var item in pattern.GetElements())
            {
                int newRow = row + item.Item1;
                int newCol = col + item.Item2;

                if (!(IsInBound(newRow, newCol) && IsColorMatched(color, newRow, newCol) && !IsTileMarked(newRow, newCol)))
                {
                    return false;
                }
            }

            return true;
        }

        void MarkPattern(GridPattern pattern, int row, int col)
        {
            bool noneSelected = true;

            foreach (var item in pattern.GetElements())
            {
                int newRow = row + item.Item1;
                int newCol = col + item.Item2;

                BaseTile newTile = mGrid[newRow, newCol].GetComponent<BaseTile>();

                if (!newTile.GetIsSelected() || pattern.GetTileType() == Powerups.PowerupType.NoPowerup)
                {
                    newTile.SetIsMarked();
                }
                else if (newTile.GetIsSelected())
                {
                    newTile.SetTileType(pattern.GetTileType());
                    noneSelected = false;
                }
            }

            if (noneSelected && pattern.GetTileType() != Powerups.PowerupType.NoPowerup)
            {
                int randomIndex = UnityEngine.Random.Range(0, pattern.GetLength());
                var randomCoords = pattern.GetElement(randomIndex);

                int newRow = row + randomCoords.Item1;
                var newCol = col + randomCoords.Item2;

                BaseTile newTile = mGrid[newRow, newCol].GetComponent<BaseTile>();
                newTile.SetIsMarked(isMarked: false);
                newTile.SetTileType(pattern.GetTileType());
            }
        }

        // This algorithm tries to match pattern on the whole grid
        foreach (var pattern in mGridPatterns)
        {
            for (int i = 0; i < GameConfig.Rows; i++)
            {
                for (int j = 0; j < GameConfig.Cols; j++)
                {
                    BaseTile tile = mGrid[i, j].GetComponent<BaseTile>();
                    Color currentColor = tile.GetColor();

                    if (IsPatternMatched(pattern, currentColor, i, j))
                    {
                        MarkPattern(pattern, i, j);
                        Debug.Log($"Match found with pattern {pattern.GetTileType()}. Tile is selected: {tile.GetIsSelected()}");
                        isMatched = true;
                    }
                }
            }
        }

        Debug.Log($"isMatched: {isMatched}");
        return isMatched;
    }

    private int[,] CalculateDropDepths(Func<GameObject, bool> filter, GameObject[,] tiles)
    {
        int[,] dropDepths = new int[GameConfig.Rows, GameConfig.Cols];

        for (int j = 0; j < GameConfig.Cols; j++)
        {
            for (int i = 0; i < GameConfig.Rows; i++)
            {
                if (filter(tiles[i, j]))
                {
                    for (int k = i; k >= 0; k--)
                    {
                        dropDepths[k, j]++;
                    }
                }
            }
        }

        return dropDepths;
    }

    public void DestroyMatches()
    {
        Debug.Log("DestroyMatches called.");

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                mGrid[i, j].GetComponent<BaseTile>().DeactivateMarked();
            }
        }

        bool CheckIsNotActive(GameObject tile)
        {
            return !tile.activeSelf;
        }

        int[,] dropDepths = CalculateDropDepths(CheckIsNotActive, mGrid);

        for (int j = 0; j < GameConfig.Cols; j++)
        {
            for (int i = GameConfig.Rows - 1; i >= 0; i--)
            {
                mGrid[i, j].GetComponent<BaseTile>().IncreaseDropDepth(dropDepths[i, j]);
            }
        }
    }

    public int UpdateGrid()
    {
        int removedTiles = 0;

        for (int i = GameConfig.Rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                var currentTile = mGrid[i, j];
                var obj = currentTile.GetComponent<BaseTile>();

                if (!currentTile.activeSelf)
                {
                    obj.Destroy();
                    mGrid[i, j] = null;
                    removedTiles++;
                    continue;
                }

                if (obj.GetDropDepth() < 1)
                    continue;

                mGrid[i + obj.GetDropDepth(), j] = currentTile;
                obj.SetCoords(i + obj.GetDropDepth(), j);

                mGrid[i, j] = null;
            }
        }

        return removedTiles;
    }

    public void FillEmptyGridInitial()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                if (mGrid[i, j] == null)
                {
                    CreateTileAt(i, j);
                }
            }
        }
    }

    public void FillEmptyGrids()
    {
        bool CheckIsNull(GameObject tile)
        {
            return tile == null;
        }

        int[,] dropDepths = CalculateDropDepths(CheckIsNull, mGrid);

        List<GameObject> createdTiles = new List<GameObject>();

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                if (mGrid[i, j] == null)
                {
                    createdTiles.Add(CreateTileAt(i, j, dropDepths[i, j]));
                }
            }
        }

        foreach (var obj in createdTiles)
        {
            obj.GetComponent<BaseTile>().AnimateDrop();
        }
    }

    public void AnimateDrops(bool isAnimated = true)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                if (mGrid[i, j] != null)
                {
                    mGrid[i, j].GetComponent<BaseTile>().AnimateDrop(isAnimated: isAnimated);
                }
            }
        }
    }

    public void AnimateDestroys()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                var baseTile = mGrid[i, j].GetComponent<BaseTile>();
                if (baseTile.GetMarked())
                {
                    baseTile.AnimateDestroy();
                }
            }
        }
    }

    public bool CheckAnimations()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                if (mGrid[i, j] != null)
                    if (mGrid[i, j].GetComponent<Animation>().isPlaying)
                        return true;
            }
        }

        return false;
    }

    public void SwitchTiles(int row1, int col1, int row2, int col2)
    {
        if ((Mathf.Abs(row1 - row2) == 1 && col1 == col2) || (Mathf.Abs(col1 - col2) == 1 && row1 == row2))
        {
            var item1 = mGrid[row1, col1].GetComponent<BaseTile>();
            var item2 = mGrid[row2, col2].GetComponent<BaseTile>();

            var pos1 = item1.GetPosition();
            var pos2 = item2.GetPosition();

            item1.AnimateSwap(pos2);
            item2.AnimateSwap(pos1);

            var tempCoords = item1.GetCoords();
            item1.SetCoords(item2.GetCoords());
            item2.SetCoords(tempCoords);

            var temp = mGrid[row1, col1];
            mGrid[row1, col1] = mGrid[row2, col2];
            mGrid[row2, col2] = temp;

            Debug.Log($"Switched ({row1},{col1}) with ({row2},{col2})");

            // Both are color remover
            if (item1.GetTileType() == Powerups.PowerupType.ColorRemover && item2.GetTileType() == Powerups.PowerupType.ColorRemover)
            {
                BombAllGrid();
            }
            // One is color remover, other is powerup
            else if (item1.GetTileType() == Powerups.PowerupType.ColorRemover && item2.GetTileType() != Powerups.PowerupType.NoPowerup)
            {
                item1.SetIsMarked();
                ReplaceColorWithPowerup(item2.GetColor(), item2.GetTileType());
            }
            else if (item2.GetTileType() == Powerups.PowerupType.ColorRemover && item1.GetTileType() != Powerups.PowerupType.NoPowerup)
            {
                item2.SetIsMarked();
                ReplaceColorWithPowerup(item1.GetColor(), item1.GetTileType());
            }
            // Only one is color remover
            else if (item1.GetTileType() == Powerups.PowerupType.ColorRemover)
            {
                item1.SetIsMarked();
                RemoveColor(item2.GetColor());
            }
            else if (item2.GetTileType() == Powerups.PowerupType.ColorRemover)
            {
                item2.SetIsMarked();
                RemoveColor(item1.GetColor());
            }
        }
    }

    public void ClearSelected()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                mGrid[i, j].GetComponent<BaseTile>().SetIsSelected(isSelected: false);
            }
        }
    }

    public void RenderDebugGrid(GridHistoryNode[,] debugGrid)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                var debugGridNode = debugGrid[i, j];
                var baseTile = mGrid[i, j].GetComponent<BaseTile>();
                baseTile.SetColor(debugGridNode.GetColor());
                baseTile.SetTileType(debugGridNode.GetTileType());
            }
        }
    }

    public void RemoveColumn(int col)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            mGrid[i, col].GetComponent<BaseTile>().SetIsMarked();
        }
    }

    public void RemoveRow(int row)
    {
        for (int j = 0; j < GameConfig.Cols; j++)
        {
            mGrid[row, j].GetComponent<BaseTile>().SetIsMarked();
        }
    }

    public void RemoveColor(Color color)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                var baseTile = mGrid[i, j].GetComponent<BaseTile>();

                if (baseTile.GetColor() == color)
                {
                    baseTile.SetIsMarked();
                }
            }
        }
    }

    public void ReplaceColorWithPowerup(Color color, Powerups.PowerupType type)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                var baseTile = mGrid[i, j].GetComponent<BaseTile>();

                if (baseTile.GetColor() == color)
                {
                    baseTile.SetTileType(type);
                    baseTile.SetIsMarked();
                }
            }
        }
    }

    public void UseBomb(Vector2Int position)
    {
        int rowStart = Mathf.Max(0, position.x - (int)(GameConfig.BombAreaCoverage / 2));
        int rowEnd = Mathf.Min(rowStart + GameConfig.BombAreaCoverage, GameConfig.Rows);

        int colStart = Mathf.Max(0, position.y - (int)(GameConfig.BombAreaCoverage / 2));
        int colEnd = Mathf.Min(colStart + GameConfig.BombAreaCoverage, GameConfig.Cols);

        Debug.Log($"Bomb used at ({position.x},{position.y}).");
        Debug.Log($"Bomb removed tiles from ({rowStart},{colStart}) to ({rowEnd},{colEnd})");

        for (int i = rowStart; i < rowEnd; i++)
        {
            for (int j = colStart; j < colEnd; j++)
            {
                mGrid[i, j].GetComponent<BaseTile>().SetIsMarked();
            }
        }
    }

    public void BombAllGrid()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                mGrid[i, j].GetComponent<BaseTile>().SetIsMarked();
            }
        }
    }
}
