using UnityEngine;

public interface ITile
{
    public void UpdateAnimation(string clipName, Vector3 targetPosition);
    public void AnimateDrop(bool animation = true);
    public void AnimateSwap(Vector3 endPosition);
    public void SetMarked();
    public bool GetMarked();
    public bool DeactivateMarked();
    public void Destroy();
    public void DestroyImmediate();
    public Color GetColor();
    public void SetColor(Color color);
    public void SetRandomColor();
    public void SetPosition(Vector3 position);
    public Vector3 GetPosition();
    public void IncreaseDropDepth(int mDropDepth = 1);
    public int GetDropDepth();
    public void SetCoords(int newRow, int newCol);
    public void SetCoords(Vector2Int newCoords);
    public Vector2Int GetCoords();
    public void SetSelected(bool selected = true);
    public bool GetSelected();
}
