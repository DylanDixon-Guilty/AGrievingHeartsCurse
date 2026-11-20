using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject objectToSpawn;
    public GameObject objectToDestroy;
    public Transform spawnLocation;
    public Transform cameraNode;
}

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private SpawnEntry[] spawnEntries;

    [Header("Scene Restoration (Optional)")]
    [SerializeField] private bool restoreOnSceneLoad;
    [SerializeField] private string restorationFlag;

    private bool hasSpawned;

    private void Start()
    {
        if (!restoreOnSceneLoad)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(restorationFlag))
        {
            Debug.LogWarning(
                $"ObjectSpawner on {gameObject.name} has restoration enabled, " +
                "but no restoration flag is assigned."
            );

            return;
        }

        if (GameSession.Instance == null)
        {
            Debug.LogWarning(
                $"ObjectSpawner on {gameObject.name} could not find GameSession."
            );

            return;
        }

        if (GameSession.Instance.GetFlag(restorationFlag))
        {
            SpawnObject();
        }
    }

    public void SpawnObject()
    {
        // Prevent this spawner from creating duplicate objects
        // during the current scene instance.
        if (hasSpawned)
        {
            return;
        }

        hasSpawned = true;

        foreach (SpawnEntry entry in spawnEntries)
        {
            if (entry.objectToSpawn != null && entry.spawnLocation != null)
            {
                GameObject spawnedObject = Instantiate(
                    entry.objectToSpawn,
                    entry.spawnLocation.position,
                    entry.spawnLocation.rotation
                );

                if (entry.cameraNode != null)
                {
                    CameraHotSpot cameraHotSpot =
                        spawnedObject.GetComponent<CameraHotSpot>();

                    if (cameraHotSpot != null)
                    {
                        cameraHotSpot.SetNode(entry.cameraNode);
                    }
                }
            }
            else if (entry.objectToSpawn != null)
            {
                Debug.LogWarning(
                    $"ObjectSpawner on {gameObject.name} cannot spawn " +
                    $"{entry.objectToSpawn.name} because its spawn location is missing."
                );
            }

            if (entry.objectToDestroy != null)
            {
                Destroy(entry.objectToDestroy);
            }
        }
    }
}