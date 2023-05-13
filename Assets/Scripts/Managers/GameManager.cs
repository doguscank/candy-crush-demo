using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Grid mGrid;
    private GridHistoryManager mHistoryManager;
    private ScoreManager mScoreManager;

    [SerializeField] private bool mIsUpdating = false;
    [SerializeField] private bool mIsClicked = false;

    [SerializeField] private GameObject mFirstSelected;
    [SerializeField] private GameObject mSecondSelected;

    public GameObject mScoreObject;
    public TMP_Text mScoreText;

    void Awake()
    {
        mScoreText = mScoreObject.GetComponent<TMP_Text>();

        if (GameConfig.IsDebug)
        {
            if (GameConfig.RandomSeed != -1)
                Random.InitState(GameConfig.RandomSeed);

            mHistoryManager = new GridHistoryManager();
        }

        mGrid = new Grid();
        mGrid.InitializeGrid();

        mScoreManager = new ScoreManager(mScoreText);

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
            mGrid.AnimateDrops(isAnimated: false);
        }

        if (GameConfig.IsDebug) { mHistoryManager.AddGrid(mGrid.GetDebugGrid()); }
    }

    void Update()
    {
        // MouseButton0 is left click
        if (Input.GetMouseButtonDown(0) && !mIsUpdating)
        { StartCoroutine(CheckClick()); }

        // Don't update game in debug mode
        if (!mIsUpdating && mIsClicked)
        { StartCoroutine(UpdateGame()); }

        if (GameConfig.IsDebug)
        {
            CheckDebugKeys();
        }
    }

    private void CheckDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mHistoryManager.DecreaseCursorIndex();
            mGrid.RenderDebugGrid(mHistoryManager.GetGridAtCursor());
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mHistoryManager.IncreaseCursorIndex();
            mGrid.RenderDebugGrid(mHistoryManager.GetGridAtCursor());
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(UpdateGame());
        }
    }

    private IEnumerator UpdateGame()
    {
        mIsUpdating = true;
        
        yield return StartCoroutine(WaitForAnimations());

        while (mGrid.CheckMatches())
        {
            yield return StartCoroutine(CallUpdateLoop());
        }

        Debug.Log("Finished UpdateGame call.");

        mIsClicked = false;
        mIsUpdating = false;
    }

    private IEnumerator CallUpdateLoop()
    {
        if (GameConfig.IsDebug) { mHistoryManager.AddGrid(mGrid.GetDebugGrid()); }
        mGrid.DestroyMatches();
        int removedTiles = mGrid.UpdateGrid();
        mScoreManager.IncreaseScore(removedTiles * 10);
        mGrid.AnimateDrops();
        mGrid.FillEmptyGrids();
        if (GameConfig.IsDebug) { mHistoryManager.AddGrid(mGrid.GetDebugGrid()); }
        yield return StartCoroutine(WaitForAnimations());
        mGrid.ClearSelected();
    }

    private IEnumerator CheckClick()
    {
        mIsUpdating = true;
        mIsClicked = true;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        // Check if clicked on a tile
        if (hit.collider != null)
        {
            yield return StartCoroutine(CheckHit(hit));
        }

        mIsUpdating = false;
    }

    private IEnumerator CheckHit(RaycastHit2D hit)
    {
        if (mFirstSelected == null)
        {
            mFirstSelected = hit.collider.gameObject;
            mFirstSelected.GetComponent<BaseTile>().SetIsSelected();
        }
        else
        {
            if (hit.collider.gameObject != mFirstSelected)
            {
                mSecondSelected = hit.collider.gameObject;
                mSecondSelected.GetComponent<BaseTile>().SetIsSelected();

                var firstCoords = mFirstSelected.GetComponent<BaseTile>().GetCoords();
                var secondCoords = mSecondSelected.GetComponent<BaseTile>().GetCoords();

                yield return StartCoroutine(SwitchTiles(firstCoords, secondCoords));
            }
            else
            {
                mFirstSelected.GetComponent<BaseTile>().SetIsSelected(isSelected: false);
            }

            mFirstSelected = null;
            mSecondSelected = null;

            yield return StartCoroutine(CallUpdateLoop());
        }
    }

    private IEnumerator SwitchTiles(Vector2Int firstCoords, Vector2Int secondCoords)
    {
        mGrid.SwitchTiles(firstCoords.x, firstCoords.y, secondCoords.x, secondCoords.y);

        yield return StartCoroutine(WaitForAnimations());

        yield return new WaitForSeconds(GameConfig.GridCheckDoneDelay);

        bool isValidSwitch = mGrid.CheckMatches();
        yield return StartCoroutine(CallUpdateLoop());
        
        if (!isValidSwitch)
        {
            // Reswitch if not valid
            mGrid.SwitchTiles(secondCoords.x, secondCoords.y, firstCoords.x, firstCoords.y);
            mGrid.ClearSelected();

            yield return StartCoroutine(WaitForAnimations());
        }
        else
        {
            yield return StartCoroutine(CallUpdateLoop());
        }
    }

    private IEnumerator WaitForAnimations()
    {
        Debug.Log("Animation check started.");

        while (mGrid.CheckAnimations())
        {
            yield return new WaitForSeconds(GameConfig.GridCheckDelay);
        }

        Debug.Log("Animation check finished.");
    }

    public Grid GetGrid()
    {
        return mGrid;
    }
}
