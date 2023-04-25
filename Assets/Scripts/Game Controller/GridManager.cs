using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Grid grid;

    public int gridRows = 5;
    public int gridCols = 5;

    void Awake()
    {
        grid = new Grid(gridRows, gridCols);

        grid.InitializeGrid();
    }

    async void Start()
    {
        while(grid.CheckMatches())
        {
            grid.UpdateMatches();
            grid.UpdateGrid();
            grid.AnimateDrops();
        }
    }

    void Update()
    {
        if (grid.CheckMatches())
        {
            Debug.Log("Update has matches");
            grid.UpdateMatches();
            grid.UpdateGrid();
            grid.AnimateDrops();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            grid.CheckMatches();
            grid.UpdateMatches();
            grid.UpdateGrid();
            grid.AnimateDrops();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            grid.ClearGrid();
            grid.InitializeGrid();
        }
    }
}
