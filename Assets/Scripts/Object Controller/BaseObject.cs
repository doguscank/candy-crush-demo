using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using TMPro;

public class BaseObject : MonoBehaviour
{
    private bool mIsMarked = false;
    private bool mIsSelected = false;
    private int mDropDepth;
    private float mAnimationDuration = 0.5f;

    private Vector2Int mCoords;

    private SpriteRenderer mSpriteRenderer;
    private GameObject mHighlight;
    private Color mColor;

    private TextMeshPro mDebugText;

    private Animation mAnimation;

    void Awake()
    {
        mSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        mAnimation = gameObject.GetComponent<Animation>();
        mHighlight = gameObject.transform.GetChild(0).gameObject;
        mDropDepth = 0;

        if (GameConfig.IsDebug)
        {
            // Create a new GameObject to hold the text
            GameObject textObject = new GameObject("Text");
            textObject.transform.parent = transform; // Make the textObject a child of this GameObject

            // Add a TextMeshPro component to the textObject
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
            textMesh.text = "1,1"; // Set the text

            mDebugText = textMesh;

            // Set the font size and alignment of the textMesh
            textMesh.fontSize = 6; // Set the font size
            textMesh.alignment = TextAlignmentOptions.Center; // Set the alignment to center

            // Set the position of the textMesh relative to the GameObject's transform
            textMesh.rectTransform.localPosition = new Vector3(0, 0, 0); // Adjust the position as needed

            // Set local scale to 1
            textMesh.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void UpdateDropAnimation()
    {
        if (mAnimation.GetClip("DropAnimation"))
            mAnimation.RemoveClip("DropAnimation");

        AnimationClip newAnimation = new AnimationClip();

        // Set animation to legacy
        newAnimation.legacy = true;

        newAnimation.name = "DropAnimation";

        // Get the current position of the object
        Vector3 startPosition = transform.position;

        // Calculate the end position of the animation
        Vector3 endPosition = startPosition - new Vector3(0f, mDropDepth * GameConfig.TileSpacing, 0f);

        AnimationCurve X_PositionCurve = new AnimationCurve();
        AnimationCurve Y_PositionCurve = new AnimationCurve();
        AnimationCurve Z_PositionCurve = new AnimationCurve();

        // Create a new keyframe at time 0 with the start position
        X_PositionCurve.AddKey(new Keyframe(0f, startPosition.x));
        // Create a new keyframe at the duration of the animation with the end position
        X_PositionCurve.AddKey(new Keyframe(mAnimationDuration, endPosition.x));

        // Create a new keyframe at time 0 with the start position
        Y_PositionCurve.AddKey(new Keyframe(0f, startPosition.y));
        // Create a new keyframe at the duration of the animation with the end position
        Y_PositionCurve.AddKey(new Keyframe(mAnimationDuration, endPosition.y));
        
        // Create a new keyframe at time 0 with the start position
        Z_PositionCurve.AddKey(new Keyframe(0f, startPosition.z));
        // Create a new keyframe at the duration of the animation with the end position
        Z_PositionCurve.AddKey(new Keyframe(mAnimationDuration, endPosition.z));

        // Update the animation curve
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.x", X_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.y", Y_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.z", Z_PositionCurve);

        // Update clip
        mAnimation.AddClip(newAnimation, "DropAnimation");
        mAnimation.clip = newAnimation;
    }

    private void UpdateSwapAnimation(Vector3 targetPos)
    {
        if (mAnimation.GetClip("SwapAnimation"))
            mAnimation.RemoveClip("SwapAnimation");

        AnimationClip newAnimation = new AnimationClip();

        // Set animation to legacy
        newAnimation.legacy = true;

        newAnimation.name = "SwapAnimation";

        // Get the current position of the object
        Vector3 startPosition = transform.position;

        AnimationCurve X_PositionCurve = new AnimationCurve();
        AnimationCurve Y_PositionCurve = new AnimationCurve();
        AnimationCurve Z_PositionCurve = new AnimationCurve();

        // Create a new keyframe at time 0 with the start position
        X_PositionCurve.AddKey(new Keyframe(0f, startPosition.x));
        // Create a new keyframe at the duration of the animation with the end position
        X_PositionCurve.AddKey(new Keyframe(mAnimationDuration, targetPos.x));

        // Create a new keyframe at time 0 with the start position
        Y_PositionCurve.AddKey(new Keyframe(0f, startPosition.y));
        // Create a new keyframe at the duration of the animation with the end position
        Y_PositionCurve.AddKey(new Keyframe(mAnimationDuration, targetPos.y));
        
        // Create a new keyframe at time 0 with the start position
        Z_PositionCurve.AddKey(new Keyframe(0f, startPosition.z));
        // Create a new keyframe at the duration of the animation with the end position
        Z_PositionCurve.AddKey(new Keyframe(mAnimationDuration, targetPos.z));

        // Update the animation curve
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.x", X_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.y", Y_PositionCurve);
        newAnimation.SetCurve("", typeof(Transform), "m_LocalPosition.z", Z_PositionCurve);

        // Update clip
        mAnimation.AddClip(newAnimation, "SwapAnimation");
        mAnimation.clip = newAnimation;
    }

    public void SetMarked()
    {
        mIsMarked = true;
    }

    public bool DeactivateMarked()
    {
        if (mIsMarked)
            gameObject.SetActive(false);

        return mIsMarked;
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
        return mColor;
    }

    public void SetColor(Color color)
    {
        mSpriteRenderer.color = Color.white;
        mSpriteRenderer.sprite = transform.GetChild(1).GetComponent<Tilemap>().GetSprite(new Vector3Int(TileColors.Colors.IndexOf(mColor) - 1, 0, 0));
        
        mColor = color;
    }

    public void SetRandomColor()
    {
        SetColor(TileColors.GetRandomColor());
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetPosition(Vector2Int position)
    {
        SetPosition(new Vector3(position.x, position.y, 0f));
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void IncreaseDropDepth(int mDropDepth = 1)
    {
        this.mDropDepth += mDropDepth;
    }

    public void AnimateDrop(bool animation = true)
    {
        if (mDropDepth > 0)
        {
            Vector3 endPosition = transform.position - new Vector3(0f, mDropDepth * GameConfig.TileSpacing, 0f);
            
            if (animation)
            {
                UpdateDropAnimation();
                mAnimation.Play("DropAnimation");
            }
            
            SetPosition(endPosition);
            
            mDropDepth = 0;
        }
    }

    public void AnimateSwap(Vector3 endPosition)
    {
        UpdateSwapAnimation(endPosition);
        mAnimation.Play("SwapAnimation");

        SetPosition(endPosition);
    }

    public int GetDropDepth()
    {
        return mDropDepth;
    }

    public void SetCoords(int newRow, int newCol)
    {
        SetCoords(new Vector2Int(newRow, newCol));
    }

    public void SetCoords(Vector2Int newCoords)
    {
        mCoords = newCoords;

        if (GameConfig.IsDebug)
        {
            mDebugText.text = $"{newCoords.x},{newCoords.y}";
        }
    }

    public Vector2Int GetCoords()
    {
        return mCoords;
    }

    public void SetSelected(bool selected = true)
    {
        mIsSelected = selected;
        mHighlight.SetActive(selected);
    }

    public bool GetSelected()
    {
        return mIsSelected;
    }
}
