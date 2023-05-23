using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    private GameObject mGameController;
    private GameManager mGameManager;
    private GridHistoryManager mHistoryManager;
    private Grid mGrid;

    public DebugManager(Grid grid)
    {
        if (GameConfig.RandomSeed != -1)
            Random.InitState(GameConfig.RandomSeed);

        mGameController = GameObject.FindGameObjectWithTag("GameController");
        mGameManager = mGameController.GetComponent<GameManager>();

        mGrid = grid;
        mHistoryManager = new GridHistoryManager();
    }

    void Start()
    {
        mHistoryManager.AddGrid(mGrid.GetDebugGrid());
    }

    void Update()
    {
        CheckDebugKeys();
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
    }

    public void AddGridHistory(GridHistoryNode[,] newGrid)
    {
        mHistoryManager.AddGrid(newGrid);
    }
}