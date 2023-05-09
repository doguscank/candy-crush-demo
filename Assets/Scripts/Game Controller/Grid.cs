using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private float minXPos;
    private float maxYPos;

    [SerializeField] private GameObject mPrefab;
    [SerializeField] private GameObject[,] mGrid;

    public Grid()
    {
        minXPos = -GameConfig.TileSpacing * (int)(GameConfig.Cols / 2);
        maxYPos = GameConfig.TileSpacing * (int)(GameConfig.Rows / 2);

        mPrefab = Resources.Load<GameObject>("Prefabs/Tile");

        mGrid = new GameObject[GameConfig.Rows, GameConfig.Cols];
    }

    public GameObject[,] GetGrid()
    {
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
    }

    public GameObject CreateTileAt(int row, int col, int dropDepth = 0)
    {
        if (row >= GameConfig.Rows || row < 0 || col >= GameConfig.Cols || col < 0)
            return null;

        GameObject newTile = GameObject.Instantiate(mPrefab);
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

        return newTile;
    }

    public bool CheckMatches()
    {
        bool hasMatch = false;

        // Check mGrid for horizontal matches
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols - GameConfig.MinSequenceLength + 1; j++)
            {
                Color currentItemColor = mGrid[i, j].GetComponent<BaseTile>().GetColor();
                int currentSequenceLength = 1;

                for (int k = j + 1; k < Mathf.Min(j + GameConfig.MaxSequenceLength, GameConfig.Cols); k++)
                {
                    if (mGrid[i, k].GetComponent<BaseTile>().GetColor() == currentItemColor)
                    {
                        currentSequenceLength++;
                    }
                    else
                        break;
                }

                if (currentSequenceLength >= GameConfig.MinSequenceLength)
                {
                    for (int k = 0; k < currentSequenceLength; k++)
                    {
                        hasMatch = true;
                        if (GameConfig.IsDebug)
                            mGrid[i, j + k].GetComponent<BaseTile>().SetSelected(true);
                        mGrid[i, j + k].GetComponent<BaseTile>().SetMarked();
                    }

                    break;
                }

            }
        }

        // Check mGrid for vertical matches
        for (int j = 0; j < GameConfig.Cols; j++)
        {
            for (int i = 0; i < GameConfig.Rows - GameConfig.MinSequenceLength + 1; i++)
            {
                Color currentItemColor = mGrid[i, j].GetComponent<BaseTile>().GetColor();
                int currentSequenceLength = 1;

                for (int k = i + 1; k < Mathf.Min(i + GameConfig.MaxSequenceLength, GameConfig.Rows); k++)
                {
                    if (mGrid[k, j].GetComponent<BaseTile>().GetColor() == currentItemColor)
                    {
                        currentSequenceLength++;
                    }
                    else
                        break;
                }

                if (currentSequenceLength >= GameConfig.MinSequenceLength)
                {
                    for (int k = 0; k < currentSequenceLength; k++)
                    {
                        hasMatch = true;
                        if (GameConfig.IsDebug)
                            mGrid[i + k, j].GetComponent<BaseTile>().SetSelected(true);
                        mGrid[i + k, j].GetComponent<BaseTile>().SetMarked();
                    }

                    break;
                }

            }
        }

        return hasMatch;
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < GameConfig.Rows; i++)
            for (int j = 0; j < GameConfig.Cols; j++)
                mGrid[i, j].GetComponent<BaseTile>().DeactivateMarked();

        for (int j = 0; j < GameConfig.Cols; j++)
        {
            for (int i = GameConfig.Rows - 1; i >= 0; i--)
            {
                if (!mGrid[i, j].activeSelf)
                {
                    for (int k = i - 1; k >= 0; k--)
                    {
                        mGrid[k, j].GetComponent<BaseTile>().IncreaseDropDepth();
                    }
                }
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
        int[] dropDepths = new int[GameConfig.Cols];
        List<GameObject> createdTiles = new List<GameObject>();

        for (int j = 0; j < GameConfig.Cols; j++)
        {
            for (int i = 0; i < GameConfig.Rows; i++)
            {
                if (mGrid[i, j] == null)
                    dropDepths[j]++;
                else
                    break;
            }
        }

        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                if (mGrid[i, j] == null)
                {
                    createdTiles.Add(CreateTileAt(i, j, dropDepths[j]));
                }
            }
        }

        foreach (var obj in createdTiles)
        {
            obj.GetComponent<BaseTile>().AnimateDrop();
        }
    }

    public void AnimateDrops(bool animation = true)
    {
        for (int i = 0; i < GameConfig.Rows; i++)
        {
            for (int j = 0; j < GameConfig.Cols; j++)
            {
                if (mGrid[i, j] != null)
                {
                    mGrid[i, j].GetComponent<BaseTile>().AnimateDrop(animation: animation);
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
