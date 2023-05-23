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

    public void Initialize(Grid grid)
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

        if (GameConfig.SpawnRandomPowerups)
        {
            SpawnRandomPowerups();
        }
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

    private void SpawnRandomPowerups()
    {
        int numberOfVectors = GameConfig.NumberOfRandomPowerups * 4; // 4 is number of powerups
        var grid = mGrid.GetGrid();
        var randomCoords = MathUtils.GenerateRandomVectors(numberOfVectors);

        for (int i = 0; i < randomCoords.Count; i++)
        {
            var coord = randomCoords[i];
            grid[coord.x, coord.y].GetComponent<BaseTile>().SetTileType((Powerups.PowerupType)(1 + i / GameConfig.NumberOfRandomPowerups));
        }
    }
}