using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHistoryManager
{
    public LinkedList<Color[,]> history = new LinkedList<Color[,]>();
    public int cursorIndex = 0;

    public GridHistoryManager()
    {

    }

    public void AddGrid(Color[,] newGrid)
    {
        if (history.Count > GameConfig.HistorySize)
            history.RemoveFirst();
        
        history.AddLast(newGrid);
        cursorIndex = history.Count - 1;
    }

    public Color[,] GetLastGrid()
    {
        return history.Last.Value;
    }

    public void RemoveLast()
    {
        history.RemoveLast();
    }

    public Color[,] GetGridAtCursor()
    {
        var cursor = history.First;

        for (int i=0; i<cursorIndex; i++)
        {
            cursor = cursor.Next;
        }

        return cursor.Value;
    }

    public void IncreaseCursorIndex()
    {
        if (cursorIndex < history.Count - 1)
            cursorIndex++;
    }

    public void DecreaseCursorIndex()
    {
        if (cursorIndex > 0)
            cursorIndex--;
    }

    public int GetCursorIndex()
    {
        return cursorIndex;
    }
}