using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    [SerializeField] private int rows;
    [SerializeField] private int cols;
    [SerializeField] private float spacing;
    private const int minSequenceLength = 3;
    private const int maxSequenceLength = 5;

    private float minXPos;
    private float maxYPos;

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject[,] grid;

    public Grid(int rows, int cols, float spacing = 0.4f)
    {
        this.rows = rows;
        this.cols = cols;
        this.spacing = spacing;

        minXPos = -spacing * (int)(cols / 2);
        maxYPos = spacing * (int)(rows / 2);

        prefab = Resources.Load<GameObject>("Prefabs/Tile");
        
        grid = new GameObject[rows, cols];
    }

    public GameObject[,] GetGrid()
    {
        // Create a new array with the same dimensions as the original
        GameObject[,] copyGrid = new GameObject[rows, cols];

        // Copy each element of the original array to the new array
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject originalGameObject = grid[i, j];
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
        Color[,] colorGrid = new Color[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                colorGrid[i, j] = grid[i, j].GetComponent<BaseObject>().GetColor();
            }
        }

        return colorGrid;
    }

    public void InitializeGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                CreateTileAt(i, j);
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j].GetComponent<BaseObject>().DestroyImmediate();
            }
        }
    }

    public void SetGridSize(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        grid = new GameObject[rows, cols];
    }

    public GameObject CreateTileAt(int row, int col)
    {
        if (row >= rows || row < 0 || col >= cols || col < 0)
            return null;

        GameObject newTile = GameObject.Instantiate(prefab);
        var script = newTile.GetComponent<BaseObject>();
        script.SetRandomColor();
        script.SetPosition(new Vector3(
            minXPos + col * spacing,
            maxYPos - row * spacing,
            0f
        ));
        script.SetCoords(row, col);

        grid[row, col] = newTile;

        return newTile;
    }

    public GameObject CreateTileAt(int row, int col, int dropDepth)
    {
        if (row >= rows || row < 0 || col >= cols || col < 0)
            return null;

        GameObject newTile = GameObject.Instantiate(prefab);
        var script = newTile.GetComponent<BaseObject>();
        script.SetRandomColor();
        script.SetPosition(new Vector3(
            minXPos + col * spacing,
            maxYPos - (row - dropDepth) * spacing,
            0f
        ));
        script.SetCoords(row, col);
        script.IncreaseDropDepth(dropDepth);

        grid[row, col] = newTile;

        return newTile;
    }

    public bool CheckMatches()
    {
        bool hasMatch = false;

        // Check grid for horizontal matches
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols - minSequenceLength + 1; j++)
            {
                Color currentItemColor = grid[i, j].GetComponent<BaseObject>().GetColor();
                int currentSequenceLength = 1;

                for (int k = j + 1; k < Mathf.Min(j + maxSequenceLength, cols); k++)
                {
                    if (grid[i, k].GetComponent<BaseObject>().GetColor() == currentItemColor)
                    {
                        currentSequenceLength++;
                    }
                    else
                        break;
                }

                if (currentSequenceLength >= minSequenceLength)
                {
                    for (int k = 0; k < currentSequenceLength; k++)
                    {
                        hasMatch = true;
                        if (GameConfig.Debug)
                            grid[i, j + k].GetComponent<BaseObject>().SetSelected(true);
                        grid[i, j + k].GetComponent<BaseObject>().SetToBeDestroyed();
                    }
                    
                    break;
                }
                
            }
        }

        // Check grid for vertical matches
        for (int j = 0; j < cols; j++)
        {
            for (int i = 0; i < rows - minSequenceLength + 1; i++)
            {
                Color currentItemColor = grid[i, j].GetComponent<BaseObject>().GetColor();
                int currentSequenceLength = 1;

                for (int k = i + 1; k < Mathf.Min(i + maxSequenceLength, rows); k++)
                {
                    if (grid[k, j].GetComponent<BaseObject>().GetColor() == currentItemColor)
                    {
                        currentSequenceLength++;
                    }
                    else
                        break;
                }

                if (currentSequenceLength >= minSequenceLength)
                {
                    for (int k = 0; k < currentSequenceLength; k++)
                    {
                        hasMatch = true;
                        if (GameConfig.Debug)
                            grid[i + k, j].GetComponent<BaseObject>().SetSelected(true);
                        grid[i + k, j].GetComponent<BaseObject>().SetToBeDestroyed();
                    }
                    
                    break;
                }
                
            }
        }

        return hasMatch;
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                grid[i, j].GetComponent<BaseObject>().DeactivateChecked();

        for (int j = 0; j < cols; j++)
        {
            for (int i = rows - 1; i >= 0; i--)
            {
                if (!grid[i, j].activeSelf)
                {
                    for (int k = i - 1; k >= 0; k--)
                    {
                        grid[k, j].GetComponent<BaseObject>().IncreaseDropDepth();
                    }
                }
            }
        }
    }

    public int UpdateGrid()
    {
        int removedTiles = 0;

        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                var currentTile = grid[i, j];
                var obj = currentTile.GetComponent<BaseObject>();

                if (!currentTile.activeSelf)
                {
                    obj.Destroy();
                    grid[i, j] = null;
                    removedTiles++;
                    continue;
                }

                if (obj.GetDropDepth() < 1)
                    continue;

                grid[i + obj.GetDropDepth(), j] = currentTile;
                obj.SetCoords(i + obj.GetDropDepth(), j);

                grid[i, j] = null;
            }
        }

        return removedTiles;
    }

    public void FillEmptyGridInitial()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == null)
                {
                    CreateTileAt(i, j);
                }
            }
        }
    }

    public void FillEmptyGrids()
    {
        int[] dropDepths = new int[cols];
        List<GameObject> createdTiles = new List<GameObject>();

        for (int j = 0; j < cols; j++)
        {
            for (int i = 0; i < rows; i++)
            {
                if (grid[i, j] == null)
                    dropDepths[j]++;
                else
                    break;
            }
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == null)
                {
                    createdTiles.Add(CreateTileAt(i, j, dropDepths[j]));
                }
            }
        }

        foreach (var obj in createdTiles)
        {
            obj.GetComponent<BaseObject>().AnimateDrop();
        }
    }

    public void AnimateDrops(bool animation = true)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] != null)
                {
                    grid[i, j].GetComponent<BaseObject>().AnimateDrop(animation: animation);
                }
            }
        }
    }

    public bool CheckAnimations()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] != null)
                    if (grid[i, j].GetComponent<Animation>().isPlaying)
                        return true;
            }
        }

        return false;
    }

    public void SwitchTiles(int row1, int col1, int row2, int col2)
    {
        if ((Mathf.Abs(row1 - row2) == 1 && col1 == col2) || (Mathf.Abs(col1 - col2) == 1 && row1 == row2))
        {
            var item1 = grid[row1, col1].GetComponent<BaseObject>();
            var item2 = grid[row2, col2].GetComponent<BaseObject>();

            var tempCoords = item1.GetCoords();
            item1.SetCoords(item2.GetCoords());
            item2.SetCoords(tempCoords);           

            var temp = grid[row1, col1];
            grid[row1, col1] = grid[row2, col2];
            grid[row2, col2] = temp;

            Debug.Log($"Switched ({row1},{col1}) with ({row2},{col2})");
        }
    }

    public void RenderColorGrid(Color[,] colorGrid)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j].GetComponent<BaseObject>().SetColor(colorGrid[i, j]);
            }
        }
    }
}
