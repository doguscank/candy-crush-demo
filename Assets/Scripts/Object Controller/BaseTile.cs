using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class BaseTile : MonoBehaviour, ITile
{
    [SerializeField] private bool mIsMarked = false;
    [SerializeField] private bool mIsSelected = false;
    [SerializeField] private int mDropDepth;

    [SerializeField] private Vector2Int mCoords;

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
            CreateDebugText();
        }
    }

    private void CreateDebugText()
    {
        // Create a new GameObject to hold the text
        GameObject textObject = new GameObject("DebugText");
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

    public void UpdateAnimation(string clipName, Vector3 targetPosition)
    {
        AnimationClip animationClip = mAnimation.GetClip(clipName);
        targetPosition = MathUtils.FixVector3(targetPosition);

        // Check if clip exists
        if (!animationClip)
        {
            // Create new clip
            animationClip = new AnimationClip();
            // Set animation to legacy
            animationClip.legacy = true;
            animationClip.name = clipName;
            // Update clip
            mAnimation.AddClip(animationClip, clipName);
            mAnimation.clip = animationClip;
        }

        // Get the current position of the object
        Vector3 startPosition = GetPosition();

        AnimationCurve X_PositionCurve = new AnimationCurve();
        AnimationCurve Y_PositionCurve = new AnimationCurve();
        AnimationCurve Z_PositionCurve = new AnimationCurve();

        // Create a new keyframe at time 0 with the start position
        X_PositionCurve.AddKey(new Keyframe(0f, startPosition.x));
        // Create a new keyframe at the duration of the animation with the end position
        X_PositionCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetPosition.x));

        // Create a new keyframe at time 0 with the start position
        Y_PositionCurve.AddKey(new Keyframe(0f, startPosition.y));
        // Create a new keyframe at the duration of the animation with the end position
        Y_PositionCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetPosition.y));
        
        // Create a new keyframe at time 0 with the start position
        Z_PositionCurve.AddKey(new Keyframe(0f, startPosition.z));
        // Create a new keyframe at the duration of the animation with the end position
        Z_PositionCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetPosition.z));

        // Update the animation curve
        animationClip.SetCurve("", typeof(Transform), "m_LocalPosition.x", X_PositionCurve);
        animationClip.SetCurve("", typeof(Transform), "m_LocalPosition.y", Y_PositionCurve);
        animationClip.SetCurve("", typeof(Transform), "m_LocalPosition.z", Z_PositionCurve);
    }

    public void AnimateDrop(bool isAnimated = true)
    {
        if (mDropDepth > 0)
        {
            Vector3 endPosition = GetPosition() - MathUtils.FixVector3(new Vector3(0f, mDropDepth * GameConfig.TileSpacing, 0f));

            Debug.Log($"Drop animation played for Tile({mCoords.x},{mCoords.y}) with mDropDepth({mDropDepth}) to endPosition({endPosition.x};{endPosition.y};{endPosition.z})");
            
            if (isAnimated)
            {
                UpdateAnimation("DropAnimation", endPosition);
                mAnimation.Play("DropAnimation");
            }
            
            SetPosition(endPosition);
            
            mDropDepth = 0;
        }
    }

    public void AnimateSwap(Vector3 endPosition)
    {
        Debug.Log($"Swap animation played for Tile({mCoords.x},{mCoords.y}) to endPosition({endPosition.x};{endPosition.y};{endPosition.z})");

        UpdateAnimation("SwapAnimation", endPosition);
        mAnimation.Play("SwapAnimation");

        SetPosition(endPosition);
    }

    public void SetMarked()
    {
        mIsMarked = true;

        if (GameConfig.IsDebug)
            mHighlight.SetActive(true);
    }

    public bool GetMarked()
    {
        return mIsMarked;
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
        mSpriteRenderer.sprite = transform.GetChild(1).GetComponent<Tilemap>().GetSprite(new Vector3Int(TileColors.Colors.IndexOf(color) - 1, 0, 0));
        
        mColor = color;
    }

    public void SetRandomColor()
    {
        SetColor(TileColors.GetRandomColor());
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = MathUtils.FixVector3(position);
    }

    public Vector3 GetPosition()
    {
        return MathUtils.FixVector3(transform.position);
    }

    public void IncreaseDropDepth(int mDropDepth = 1)
    {
        this.mDropDepth += mDropDepth;
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
