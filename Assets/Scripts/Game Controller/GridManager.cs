using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Grid grid;
    public int gridRows = 5;
    public int gridCols = 5;

    public GameObject firstSelected;
    public GameObject secondSelected;

    void Awake()
    {
        grid = new Grid(gridRows, gridCols);
        grid.InitializeGrid();

        firstSelected = null;
        secondSelected = null;
    }

    void Start()
    {
        while(grid.CheckMatches())
        {
            grid.DestroyMatches();
            grid.UpdateGrid();
            grid.AnimateDrops();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool validSwitch = false;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (firstSelected == null)
                {
                    firstSelected = hit.collider.gameObject;
                    firstSelected.GetComponent<BaseObject>().SetSelected();
                }
                else if (secondSelected == null)
                {
                    if (hit.collider.gameObject != firstSelected)
                    {
                        secondSelected = hit.collider.gameObject;

                        var firstScript = firstSelected.GetComponent<BaseObject>();
                        var secondScript = secondSelected.GetComponent<BaseObject>();

                        var firstCoords = firstScript.GetCoords();
                        var secondCoords = secondScript.GetCoords();

                        grid.SwitchTiles(firstCoords.x, firstCoords.y, secondCoords.x, secondCoords.y);
                        validSwitch = grid.CheckMatches();
                        if (!validSwitch)
                            // Reswitch if not valid
                            grid.SwitchTiles(secondCoords.x, secondCoords.y, firstCoords.x, firstCoords.y);
                        else
                        {
                            var tempPos = firstScript.GetPosition();
                            firstScript.SetPosition(secondScript.GetPosition());
                            secondScript.SetPosition(tempPos);
                        }
                    }

                    firstSelected.GetComponent<BaseObject>().SetSelected(selected: false);

                    firstSelected = null;
                    secondSelected = null;
                }
            }

            while (grid.CheckMatches() || validSwitch)
            {
                grid.DestroyMatches();
                grid.UpdateGrid();
                grid.AnimateDrops();
                validSwitch = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            grid.CheckMatches();
            grid.DestroyMatches();
            grid.UpdateGrid();
            grid.AnimateDrops();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            grid.ClearGrid();
            grid.InitializeGrid();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            grid.CheckMatches();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            grid.DestroyMatches();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            grid.UpdateGrid();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            grid.AnimateDrops();
        }
    }
}
