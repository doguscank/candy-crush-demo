using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int rows;
    public int cols;
    private const int minSequenceLength = 3;
    private const int maxSequenceLength = 5;

    public float minXPos;
    public float maxYPos;

    public GameObject prefab;
    public GameObject[,] grid;

    public Grid(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        minXPos = -0.4f * (int)(cols / 2);
        maxYPos = 0.4f * (int)(rows / 2);

        prefab = Resources.Load<GameObject>("Prefabs/Tile");
        grid = new GameObject[rows, cols];
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

    public bool CreateTileAt(int row, int col)
    {
        if (row >= rows || row < 0 || col >= cols || col < 0)
            return false;

        GameObject newTile = GameObject.Instantiate(prefab);
        var script = newTile.GetComponent<BaseObject>();
        script.SetRandomColor();
        script.SetPosition(new Vector3(
            minXPos + col * 0.4f,
            maxYPos - row * 0.4f,
            0f
        ));

        grid[row, col] = newTile;

        return true;
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
                        grid[i + k, j].GetComponent<BaseObject>().SetToBeDestroyed();
                    }
                    
                    break;
                }
                
            }
        }

        return hasMatch;
    }

    public void UpdateMatches()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                grid[i, j].GetComponent<BaseObject>().DestroyChecked();

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

    public void UpdateGrid()
    {
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                var currentTile = grid[i, j];
                var obj = grid[i, j].GetComponent<BaseObject>();

                if (!currentTile.activeSelf)
                {
                    obj.Destroy();
                    grid[i, j] = null;
                    continue;
                }

                if (obj.GetDropDepth() < 1)
                    continue;

                grid[i + obj.GetDropDepth(), j] = currentTile;
                grid[i, j] = null;
            }
        }

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

    public void AnimateDrops()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] != null)
                {
                    grid[i, j].GetComponent<BaseObject>().AnimateDrop();
                }
            }
        }
    }
}
