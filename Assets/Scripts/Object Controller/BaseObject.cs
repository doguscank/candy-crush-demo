using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public bool destroy = false;
    public int dropDepth;

    public Vector2Int coords;

    public SpriteRenderer spriteRenderer;
    public Color color;

    public Vector3 startPos;
    public Vector3 endPos;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        dropDepth = 0;
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetToBeDestroyed()
    {
        destroy = true;
    }

    public bool DestroyChecked()
    {
        if (destroy)
            gameObject.SetActive(false);

        return destroy;
    }

    public void Destroy()
    {
        if (!gameObject.activeSelf)
            Object.Destroy(gameObject);
    }

    public void DestroyImmediate()
    {
        Object.Destroy(gameObject);
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
        this.color = color;
    }

    public void SetRandomColor()
    {
        SetColor(TileColors.GetRandomColor());
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void IncreaseDropDepth(int dropDepth = 1)
    {
        this.dropDepth += dropDepth;
    }

    public void AnimateDrop()
    {
        if (dropDepth > 0)
        {
            Vector3 newPos = transform.position - new Vector3(0f, dropDepth * 0.4f, 0f);
            SetPosition(newPos);

            dropDepth = 0;
        }
    }

    public int GetDropDepth()
    {
        return dropDepth;
    }

    public void SetCoords(int newRow, int newCol)
    {
        coords = new Vector2Int(newRow, newCol);
    }

    public void SetCoords(Vector2Int newCoords)
    {
        coords = newCoords;
    }

    public Vector2Int GetCoords()
    {
        return coords;
    }
}
