using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// Will handle spawning of objects then destroys itself
/// </summary>
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
    [SerializeField] private SpawnEntry[] spawnEntries;

    public void SpawnObject()
    {
        foreach (SpawnEntry entry in spawnEntries)
        {
            if (entry.objectToSpawn != null && entry.spawnLocation != null)
            {
                GameObject _spawnedObject = Instantiate(entry.objectToSpawn, entry.spawnLocation.position, entry.spawnLocation.rotation);

                if (entry.cameraNode != null)
                {
                    CameraHotSpot cameraHotSpot = _spawnedObject.GetComponent<CameraHotSpot>();

                    if (cameraHotSpot != null)
                    {
                        cameraHotSpot.SetNode(entry.cameraNode);
                    }
                }
            }

            if (entry.objectToDestroy != null)
            {
                Destroy(entry.objectToDestroy);
            }
        }
    }
}
