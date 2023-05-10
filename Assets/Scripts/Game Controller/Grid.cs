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
        // UNUSED
        
        // Create a new array with the same dimensions as the original
        GameObject[,] copyGrid = new GameObject[GameConfig.Rows, GameConfig.Cols];

        // Copy each element of the original array to the new array
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                GameObject originalGameObject = mGrid[i, j];
                if (originalGameObject != null)
                {
                    // Create a new GameObject instance with the same properties as the original
                    GameObject copyGameObject = GameObject.Instantiate(originalGameObject);
                    copyGameObject.SetActive(false);

                    // Set the copied GameObject to the copied array
                    copyGrid[i, j] = copyGameObject;
                }
            }
        }

        return copyGrid;
    }

    public Color[,] GetColorGrid()
    {
        Color[,] colorGrid = new Color[GameConfig.Rows, GameConfig.Cols];

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                colorGrid[i, j] = mGrid[i, j].GetComponent<BaseTile>().GetColor();
            }
        }

        return colorGrid;
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

        bool IsInBound(int row, int col)
        {
            return row >= 0 && row < GameConfig.Rows && col >= 0 && col < GameConfig.Cols;
        }

        bool IsColorMatched(Color color, int row, int col)
        {
            return mGrid[row, col].GetComponent<BaseTile>().GetColor() == color;
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
                }
            }
        }

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                BaseTile tile = mGrid[i, j].GetComponent<BaseTile>();
                Color currentColor = tile.GetColor();

                // Get each pattern
                foreach (var pattern in mGridPatterns)
                {
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

        int [,] dropDepths = CalculateDropDepths(CheckIsNotActive, mGrid);

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

    public void RenderColorGrid(Color[,] colorGrid)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                mGrid[i, j].GetComponent<BaseTile>().SetColor(colorGrid[i, j]);
            }
        }
    }
}
