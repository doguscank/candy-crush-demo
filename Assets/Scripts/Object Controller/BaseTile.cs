using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class BaseTile : MonoBehaviour, ITile
{
    [SerializeField] protected bool mIsMarked;
    [SerializeField] protected bool mIsSelected;
    [SerializeField] protected int mDropDepth;
    [SerializeField] protected Powerups.PowerupType mTileType;

    [SerializeField] protected Vector2Int mCoords;

    [SerializeField] protected GameObject mGameController;
    [SerializeField] protected Grid mGrid;

    protected SpriteRenderer mSpriteRenderer;
    protected GameObject mHighlight;
    protected Color mColor;

    protected TextMeshPro mDebugText;

    protected Animation mAnimation;

    void Awake()
    {
        mIsMarked = false;
        mIsSelected = false;

        mGameController = GameObject.FindGameObjectWithTag("GameController");
        mGrid = mGameController.GetComponent<GameManager>().GetGrid();

        mSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        mSpriteRenderer.color = Color.white;

        mAnimation = gameObject.GetComponent<Animation>();
        mHighlight = gameObject.transform.GetChild(0).gameObject;
        mDropDepth = 0;
        mTileType = Powerups.PowerupType.NoPowerup;

        if (GameConfig.IsDebug)
        {
            CreateDebugText();
        }
    }

    protected void CreateDebugText()
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
        UpdateAnimation(clipName, targetPosition, transform.rotation, transform.localScale);
    }

    public void UpdateAnimation(string clipName, Vector3 targetPosition, Vector3 targetScale)
    {
        UpdateAnimation(clipName, targetPosition, transform.rotation, targetScale);
    }

    public void UpdateAnimation(string clipName, Quaternion targetRotation)
    {
        UpdateAnimation(clipName, transform.position, targetRotation, transform.localScale);
    }

    public void UpdateAnimation(string clipName, Vector3 targetPosition, Quaternion targetRotation)
    {
        UpdateAnimation(clipName, targetPosition, targetRotation, transform.localScale);
    }

    public void UpdateAnimation(string clipName, Quaternion targetRotation, Vector3 targetScale)
    {
        UpdateAnimation(clipName, transform.position, targetRotation, targetScale);
    }

    public void UpdateAnimation(string clipName, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
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
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startScale = transform.localScale;

        AnimationCurve X_PositionCurve = new AnimationCurve();
        AnimationCurve Y_PositionCurve = new AnimationCurve();
        AnimationCurve Z_PositionCurve = new AnimationCurve();

        AnimationCurve X_RotationCurve = new AnimationCurve();
        AnimationCurve Y_RotationCurve = new AnimationCurve();
        AnimationCurve Z_RotationCurve = new AnimationCurve();

        AnimationCurve X_ScaleCurve = new AnimationCurve();
        AnimationCurve Y_ScaleCurve = new AnimationCurve();
        AnimationCurve Z_ScaleCurve = new AnimationCurve();

        // POSITION
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

        // ROTATION
        X_RotationCurve.AddKey(new Keyframe(0f, startRotation.eulerAngles.x));
        X_RotationCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetRotation.eulerAngles.x));

        Y_RotationCurve.AddKey(new Keyframe(0f, startRotation.eulerAngles.y));
        Y_RotationCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetRotation.eulerAngles.y));

        Z_RotationCurve.AddKey(new Keyframe(0f, startRotation.eulerAngles.z));
        Z_RotationCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetRotation.eulerAngles.z));

        // SCALE
        X_ScaleCurve.AddKey(new Keyframe(0f, startScale.x));
        X_ScaleCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetScale.x));

        Y_ScaleCurve.AddKey(new Keyframe(0f, startScale.y));
        Y_ScaleCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetScale.y));

        Z_ScaleCurve.AddKey(new Keyframe(0f, startScale.z));
        Z_ScaleCurve.AddKey(new Keyframe(GameConfig.AnimationDuration, targetScale.z));

        // Update the animation curve
        animationClip.SetCurve("", typeof(Transform), "m_LocalPosition.x", X_PositionCurve);
        animationClip.SetCurve("", typeof(Transform), "m_LocalPosition.y", Y_PositionCurve);
        animationClip.SetCurve("", typeof(Transform), "m_LocalPosition.z", Z_PositionCurve);

        animationClip.SetCurve("", typeof(Transform), "localEulerAnglesRaw.x", X_RotationCurve);
        animationClip.SetCurve("", typeof(Transform), "localEulerAnglesRaw.y", Y_RotationCurve);
        animationClip.SetCurve("", typeof(Transform), "localEulerAnglesRaw.z", Z_RotationCurve);

        animationClip.SetCurve("", typeof(Transform), "m_LocalScale.x", X_ScaleCurve);
        animationClip.SetCurve("", typeof(Transform), "m_LocalScale.y", Y_ScaleCurve);
        animationClip.SetCurve("", typeof(Transform), "m_LocalScale.z", Z_ScaleCurve);

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

    public void AnimateDestroy()
    {
        UpdateAnimation("DestroyTile", Quaternion.Euler(0f, 0f, 359.9f), new Vector3(0.1f, 0.1f, 1f));
        mAnimation.Play("DestroyTile");
    }

    public void SetTileType(Powerups.PowerupType type)
    {
        Debug.Log($"Updated type of tile at ({GetCoords().x},{GetCoords().y}) to {type}");
        mTileType = type;
        UpdateSprite();
    }

    public Powerups.PowerupType GetTileType()
    {
        return mTileType;
    }

    public void SetIsMarked(bool isMarked = true)
    {
        // Perform power up action
        if (mTileType != Powerups.PowerupType.NoPowerup && !mIsMarked)
        {
            mIsMarked = isMarked;

            // Power up action
            PerformPowerup();
        }

        mIsMarked = isMarked;

        if (GameConfig.IsDebug)
            mHighlight.SetActive(isMarked);
    }

    public void PerformPowerup()
    {
        switch (mTileType)
        {
            case Powerups.PowerupType.ColorRemover:
                mGrid.RemoveColor(mColor);
                break;
            case Powerups.PowerupType.Bomb:
                mGrid.UseBomb(mCoords);
                break;
            case Powerups.PowerupType.RowRemover:
                mGrid.RemoveRow(mCoords.x);
                break;
            case Powerups.PowerupType.ColumnRemover:
                mGrid.RemoveColumn(mCoords.y);
                break;
            case Powerups.PowerupType.NoPowerup:
                break;
        }
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
        {
            Object.Destroy(gameObject);
        }
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
        mColor = color;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        int columnIndex = TileColors.Colors.IndexOf(mColor) - 1;
        int rowIndex = -1 * (int)(mTileType);

        if (mTileType == Powerups.PowerupType.ColorRemover)
        {
            columnIndex = -1;
            mColor = TileColors.ColorRemoverColor;
        }

        mSpriteRenderer.sprite = transform.GetChild(1).GetComponent<Tilemap>().GetSprite(new Vector3Int(columnIndex, rowIndex, 0));
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

    public void SetIsSelected(bool isSelected = true)
    {
        mIsSelected = isSelected;
        mHighlight.SetActive(isSelected);
    }

    public bool GetIsSelected()
    {
        return mIsSelected;
    }
}
