using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class BaseObject : MonoBehaviour
{
    public bool destroy = false;
    public bool isSelected = false;
    public int dropDepth;
    public float dropDuration = 2.5f;

    public Vector2Int coords;

    public SpriteRenderer spriteRenderer;
    public GameObject highlight;
    public Color color;

    public TextMeshPro debugText;

    public Vector3 startPos;
    public Vector3 endPos;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        highlight = gameObject.transform.GetChild(0).gameObject;
        dropDepth = 0;

        if (GameConfig.Debug)
        {
            // Create a new GameObject to hold the text
            GameObject textObject = new GameObject("Text");
            textObject.transform.parent = transform; // Make the textObject a child of this GameObject

            // Add a TextMeshPro component to the textObject
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
            textMesh.text = "1,1"; // Set the text

            debugText = textMesh;

            // Set the font size and alignment of the textMesh
            textMesh.fontSize = 10; // Set the font size
            textMesh.alignment = TextAlignmentOptions.Center; // Set the alignment to center

            // Set the position of the textMesh relative to the GameObject's transform
            textMesh.rectTransform.localPosition = new Vector3(0, 0, 0); // Adjust the position as needed

            // Set local scale to 1
            textMesh.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
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
        // spriteRenderer.color = Color.white;
        // spriteRenderer.sprite = transform.GetChild(1).GetComponent<Tilemap>().GetSprite(new Vector3Int(TileColors.Colors.IndexOf(color) - 1, 0, 0));
        
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

    public void SetPosition(Vector2Int pos)
    {
        SetPosition(new Vector3(pos.x, pos.y, 0f));
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
            Vector3 endPos = transform.position - new Vector3(0f, dropDepth * 0.4f, 0f);
            SetPosition(endPos);
            // StartCoroutine(DropAnimation(dropDepth * 0.4f, dropDuration));
            dropDepth = 0;
        }
    }

    private IEnumerator DropAnimation(float dropDistance, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position - new Vector3(0f, dropDistance, 0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            yield return null;
        }

        // Ensure the sprite ends up at the exact end position
        transform.position = endPos;
    }

    public int GetDropDepth()
    {
        return dropDepth;
    }

    public void SetCoords(int newRow, int newCol)
    {
        SetCoords(new Vector2Int(newRow, newCol));
    }

    public void SetCoords(Vector2Int newCoords)
    {
        coords = newCoords;

        if (GameConfig.Debug)
        {
            debugText.text = $"{newCoords.x},{newCoords.y}";
        }
    }

    public Vector2Int GetCoords()
    {
        return coords;
    }

    public void SetSelected(bool selected = true)
    {
        isSelected = selected;
        highlight.SetActive(selected);
    }

    public bool GetSelected()
    {
        return isSelected;
    }
}
