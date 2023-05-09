using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Grid mGrid;
    private GridHistoryManager mHistoryManager;
    private ScoreManager mScoreManager;

    private bool isUpdating = false;

    private GameObject mFirstSelected;
    private GameObject mSecondSelected;

    public GameObject mScoreObject;
    public TMP_Text mScoreText;

    void Awake()
    {
        mScoreText = mScoreObject.GetComponent<TMP_Text>();
        Random.InitState(42);

        mGrid = new Grid();
        mGrid.InitializeGrid();

        mScoreManager = new ScoreManager(mScoreText);

        if (GameConfig.IsDebug) mHistoryManager = new GridHistoryManager();

        mFirstSelected = null;
        mSecondSelected = null;
    }

    void Start()
    {
        // This step is done because all tiles are spawned randomly
        while (mGrid.CheckMatches())
        {
            mGrid.DestroyMatches();
            mGrid.UpdateGrid();
            mGrid.FillEmptyGridInitial();
            mGrid.AnimateDrops(animation: false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isUpdating)
            StartCoroutine(CheckClick());

        if (!isUpdating && !GameConfig.IsDebug)
            StartCoroutine(UpdateGame());

        if (GameConfig.IsDebug)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                mHistoryManager.DecreaseCursorIndex();
                mGrid.RenderColorGrid(mHistoryManager.GetGridAtCursor());
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                mHistoryManager.IncreaseCursorIndex();
                mGrid.RenderColorGrid(mHistoryManager.GetGridAtCursor());
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                StartCoroutine(UpdateGame());
            }
        }
    }

    private IEnumerator UpdateGame()
    {
        isUpdating = true;

        while (mGrid.CheckAnimations())
        {
            yield return new WaitForSeconds(0.05f);
        }

        while (mGrid.CheckMatches())
        {
            if (GameConfig.IsDebug) mHistoryManager.AddGrid(mGrid.GetColorGrid());
            mGrid.DestroyMatches();
            int removedTiles = mGrid.UpdateGrid();
            mScoreManager.IncreaseScore(removedTiles * 10);
            mGrid.AnimateDrops();
            mGrid.FillEmptyGrids();
            if (GameConfig.IsDebug) mHistoryManager.AddGrid(mGrid.GetColorGrid());
            while (mGrid.CheckAnimations())
            {
                yield return new WaitForSeconds(0.05f);
            }
        }

        isUpdating = false;
    }

    private IEnumerator CheckClick()
    {
        isUpdating = true;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        // Check if clicked on a tile
        if (hit.collider != null)
        {
            if (mFirstSelected == null)
            {
                mFirstSelected = hit.collider.gameObject;
                mFirstSelected.GetComponent<BaseTile>().SetSelected();
            }
            else if (mSecondSelected == null)
            {
                if (hit.collider.gameObject != mFirstSelected)
                {
                    mSecondSelected = hit.collider.gameObject;

                    var firstScript = mFirstSelected.GetComponent<BaseTile>();
                    var secondScript = mSecondSelected.GetComponent<BaseTile>();

                    var firstCoords = firstScript.GetCoords();
                    var secondCoords = secondScript.GetCoords();

                    mGrid.SwitchTiles(firstCoords.x, firstCoords.y, secondCoords.x, secondCoords.y);

                    while (mGrid.CheckAnimations())
                    {
                        yield return new WaitForSeconds(0.05f);
                    }

                    yield return new WaitForSeconds(0.2f);

                    bool validSwitch = mGrid.CheckMatches();
                    if (!validSwitch)
                    {
                        // Reswitch if not valid
                        mGrid.SwitchTiles(secondCoords.x, secondCoords.y, firstCoords.x, firstCoords.y);

                        while (mGrid.CheckAnimations())
                        {
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                }

                mFirstSelected.GetComponent<BaseTile>().SetSelected(selected: false);

                mFirstSelected = null;
                mSecondSelected = null;
            }
        }

        isUpdating = false;
    }
}
