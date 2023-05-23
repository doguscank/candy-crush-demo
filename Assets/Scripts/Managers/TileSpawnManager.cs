using UnityEngine;

public class TileSpawnManager : MonoBehaviour
{
    private static TileSpawnManager mInstance;
    private GameObject mPrefab;

    public static TileSpawnManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                // Try to find the instance in the scene first.
                mInstance = FindObjectOfType<TileSpawnManager>();

                if (mInstance == null)
                {
                    // Create new instance if one doesn't exist in the scene
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    mInstance = obj.AddComponent<TileSpawnManager>();
                }

                // Load the prefab when the instance is created or accessed
                mInstance.mPrefab = Resources.Load<GameObject>("Prefabs/Tile");
            }
            return mInstance;
        }
    }

    void Awake()
    {
        // Make sure there is only one instance of this class.
        if (mInstance == null)
        {
            mInstance = this;
        }
        else if (mInstance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate TileSpawnManager instance detected. The new instance has been destroyed.");
        }

        mPrefab = Resources.Load<GameObject>("Prefabs/Tile");
        DontDestroyOnLoad(this.gameObject); // This ensures that the singleton won't be destroyed when changing scenes
    }

    public GameObject SpawnTile(Powerups.PowerupType type)
    {
        GameObject newTile = GameObject.Instantiate(mPrefab);
        newTile.GetComponent<BaseTile>().SetTileType(type);
        return newTile;
    }

    public GameObject SpawnTile(Color color, Powerups.PowerupType type)
    {
        GameObject newTile = SpawnTile(type);
        newTile.GetComponent<BaseTile>().SetColor(color);
        return newTile;
    }
}
