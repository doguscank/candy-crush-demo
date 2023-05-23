using UnityEngine;

public interface ITile
{
    public void UpdateAnimation(string clipName, Vector3 targetPosition);
    public void UpdateAnimation(string clipName, Vector3 targetPosition, Vector3 targetScale);
    public void UpdateAnimation(string clipName, Quaternion targetRotation);
    public void UpdateAnimation(string clipName, Vector3 targetPosition, Quaternion targetRotation);
    public void UpdateAnimation(string clipName, Quaternion targetRotation, Vector3 targetScale);
    public void UpdateAnimation(string clipName, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale);
    public void AnimateDrop(bool isAnimated = true);
    public void AnimateSwap(Vector3 endPosition);
    public void AnimateDestroy();
    public void SetTileType(Powerups.PowerupType type);
    public Powerups.PowerupType GetTileType();
    public void SetIsMarked(bool isMarked = true);
    public bool GetMarked();
    public bool DeactivateMarked();
    public void Destroy();
    public void DestroyImmediate();
    public Color GetColor();
    public void SetColor(Color color);
    public void UpdateSprite();
    public void SetRandomColor();
    public void SetPosition(Vector3 position);
    public Vector3 GetPosition();
    public void IncreaseDropDepth(int mDropDepth = 1);
    public int GetDropDepth();
    public void SetCoords(int newRow, int newCol);
    public void SetCoords(Vector2Int newCoords);
    public Vector2Int GetCoords();
    public void SetIsSelected(bool isSelected = true);
    public bool GetIsSelected();
}
