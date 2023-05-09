using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using TMPro;

public class BaseObject : MonoBehaviour
{
    public bool destroy = false;
    public bool isSelected = false;
    public int dropDepth;
    private float animationDuration = 0.5f;

    public Vector2Int coords;

    public SpriteRenderer spriteRenderer;
    public GameObject highlight;
    public Color color;

    public TextMeshPro debugText;

    public Animation anim;
    public AnimationCurve animationCurve;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animation>();
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
            textMesh.fontSize = 6; // Set the font size
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

    private void UpdateDropAnimation()
    {
        if (anim.GetClip("DropAnimation"))
            anim.RemoveClip("DropAnimation");

        AnimationClip newAnimation = new AnimationClip();

        // Set animation to legacy
        newAnimation.legacy = true;

        newAnimation.name = "DropAnimation";

        // Get the current position of the object
        Vector3 startPos = transform.position;

        // Calculate the end position of the animation
        Vector3 endPos = startPos - new Vector3(0f, dropDepth * GameConfig.TileSpacing, 0f);

        AnimationCurve X_PositionCurve = new AnimationCurve();
        AnimationCurve Y_PositionCurve = new AnimationCurve();
        AnimationCurve Z_PositionCurve = new AnimationCurve();

        // Create a new keyframe at time 0 with the start position
        X_PositionCurve.AddKey(new Keyframe(0f, startPos.x));
        // Create a new keyframe at the duration of the animation with the end position
        X_PositionCurve.AddKey(new Keyframe(animationDuration, endPos.x));

        // Create a new keyframe at time 0 with the start position
        Y_PositionCurve.AddKey(new Keyframe(0f, startPos.y));
        // Create a new keyframe at the duration of the animation with the end position
        Y_PositionCurve.AddKey(new Keyframe(animationDuration, endPos.y));
        
        // Create a new keyframe at time 0 with the start position
        Z_PositionCurve.AddKey(new Keyframe(0f, startPos.z));
        // Create a new keyframe at the duration of the animation with the end position
        Z_PositionCurve.AddKey(new Keyframe(animationDuration, endPos.z));

        // Update the animation curve
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.x", X_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.y", Y_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.z", Z_PositionCurve);

        // Update clip
        anim.AddClip(newAnimation, "DropAnimation");
        anim.clip = newAnimation;
    }

    private void UpdateSwapAnimation(Vector3 targetPos)
    {
        if (anim.GetClip("SwapAnimation"))
            anim.RemoveClip("SwapAnimation");

        AnimationClip newAnimation = new AnimationClip();

        // Set animation to legacy
        newAnimation.legacy = true;

        newAnimation.name = "SwapAnimation";

        // Get the current position of the object
        Vector3 startPos = transform.position;

        AnimationCurve X_PositionCurve = new AnimationCurve();
        AnimationCurve Y_PositionCurve = new AnimationCurve();
        AnimationCurve Z_PositionCurve = new AnimationCurve();

        // Create a new keyframe at time 0 with the start position
        X_PositionCurve.AddKey(new Keyframe(0f, startPos.x));
        // Create a new keyframe at the duration of the animation with the end position
        X_PositionCurve.AddKey(new Keyframe(animationDuration, targetPos.x));

        // Create a new keyframe at time 0 with the start position
        Y_PositionCurve.AddKey(new Keyframe(0f, startPos.y));
        // Create a new keyframe at the duration of the animation with the end position
        Y_PositionCurve.AddKey(new Keyframe(animationDuration, targetPos.y));
        
        // Create a new keyframe at time 0 with the start position
        Z_PositionCurve.AddKey(new Keyframe(0f, startPos.z));
        // Create a new keyframe at the duration of the animation with the end position
        Z_PositionCurve.AddKey(new Keyframe(animationDuration, targetPos.z));

        // Update the animation curve
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.x", X_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.y", Y_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.z", Z_PositionCurve);

        // Update clip
        anim.AddClip(newAnimation, "SwapAnimation");
        anim.clip = newAnimation;
    }

    public void SetToBeDestroyed()
    {
        destroy = true;
    }

    public bool DeactivateChecked()
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
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = transform.GetChild(1).GetComponent<Tilemap>().GetSprite(new Vector3Int(TileColors.Colors.IndexOf(color) - 1, 0, 0));
        
        // spriteRenderer.color = color;
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

    public void AnimateDrop(bool animation = true)
    {
        if (dropDepth > 0)
        {
            Vector3 endPos = transform.position - new Vector3(0f, dropDepth * GameConfig.TileSpacing, 0f);
            
            if (animation)
            {
                UpdateDropAnimation();
                anim.Play("DropAnimation");
            }
            
            SetPosition(endPos);
            
            dropDepth = 0;
        }
    }

    public void AnimateSwap(Vector3 targetPosition)
    {
        UpdateSwapAnimation(targetPosition);
        anim.Play("SwapAnimation");

        SetPosition(targetPosition);
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
