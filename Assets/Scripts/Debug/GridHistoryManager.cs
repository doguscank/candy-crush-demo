using System.Collections.Generic;
using UnityEngine;

public class GridHistoryManager
{
    private LinkedList<GridHistoryNode[,]> mHistory = new LinkedList<GridHistoryNode[,]>();
    private int mCursorIndex = 0;

    public GridHistoryManager()
    {

    }

    public void AddGrid(GridHistoryNode[,] newGrid)
    {
        if (mHistory.Count > GameConfig.HistorySize)
            mHistory.RemoveFirst();

        mHistory.AddLast(newGrid);
        mCursorIndex = mHistory.Count - 1;
    }

    public GridHistoryNode[,] GetLastGrid()
    {
        return mHistory.Last.Value;
    }

    public void RemoveLast()
    {
        mHistory.RemoveLast();
    }

    public GridHistoryNode[,] GetGridAtCursor()
    {
        var cursor = mHistory.First;

        for (int i = 0; i < mCursorIndex; i++)
        {
            cursor = cursor.Next;
        }

        return cursor.Value;
    }

    public void IncreaseCursorIndex()
    {
        if (mCursorIndex < mHistory.Count - 1)
            mCursorIndex++;
    }

    public void DecreaseCursorIndex()
    {
        if (mCursorIndex > 0)
            mCursorIndex--;
    }

    public int GetCursorIndex()
    {
        return mCursorIndex;
    }
}