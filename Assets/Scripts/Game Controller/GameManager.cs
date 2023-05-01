using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public GridHistoryManager historyManager;
    public ScoreManager scoreManager;
    public int gridRows = 5;
    public int gridCols = 5;

    public bool updating = false;

    public GameObject firstSelected;
    public GameObject secondSelected;

    public GameObject scoreObject;
    public TMP_Text scoreText;

    void Awake()
    {
        scoreText = scoreObject.GetComponent<TMP_Text>();
        Random.InitState(42);

        grid = new Grid(gridRows, gridCols);
        grid.InitializeGrid();

        scoreManager = new ScoreManager(scoreText);

        if (GameConfig.Debug) historyManager = new GridHistoryManager();

        firstSelected = null;
        secondSelected = null;
    }

    void Start()
    {
        // This step is done because all tiles are spawned randomly
        while(grid.CheckMatches())
        {
            grid.DestroyMatches();
            grid.UpdateGrid();
            grid.FillEmptyGridInitial();
            grid.AnimateDrops(animation: false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !updating)
        {
            updating = true;
            CheckClick();
            StartCoroutine(UpdateGame());
        }

        if (GameConfig.Debug)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                historyManager.DecreaseCursorIndex();
                grid.RenderColorGrid(historyManager.GetGridAtCursor());
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                historyManager.IncreaseCursorIndex();
                grid.RenderColorGrid(historyManager.GetGridAtCursor());
            }
        }
    }

    private IEnumerator UpdateGame()
    {
        while (grid.CheckMatches())
        {
            if (GameConfig.Debug) historyManager.AddGrid(grid.GetColorGrid());
            grid.DestroyMatches();
            int removedTiles = grid.UpdateGrid();
            scoreManager.IncreaseScore(removedTiles * 10);
            grid.AnimateDrops();
            grid.FillEmptyGrids();
            if (GameConfig.Debug) historyManager.AddGrid(grid.GetColorGrid());
            while (grid.CheckAnimations())
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
        
        updating = false;
    }

    private void CheckClick()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        // Check if clicked on a tile
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
                    bool validSwitch = grid.CheckMatches();
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
    }
}
