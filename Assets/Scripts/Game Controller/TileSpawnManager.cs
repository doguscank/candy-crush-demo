using UnityEngine;

public class TileSpawnManager : MonoBehaviour
{
    private static TileSpawnManager mInstance;

    [SerializeField] private GameObject mPrefab;

    public static TileSpawnManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                Debug.LogError("TileSpawnManager instance is null. Please ensure an object with the TileSpawnManager component is in your scene.");
            }
            return mInstance;
        }
    }

    private void Awake()
    {
        mPrefab = Resources.Load<GameObject>("Prefabs/Tile");

        // Check if an instance already exists
        if (mInstance != null && mInstance != this)
        {
            // Destroy the duplicate instance
            Destroy(gameObject);
            Debug.LogWarning("Duplicate TileSpawnManager instance detected. The new instance has been destroyed.");
        }
        else
        {
            // Assign the current object as the instance
            mInstance = this;
        }
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
