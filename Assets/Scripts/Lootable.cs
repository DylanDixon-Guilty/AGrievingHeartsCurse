using UnityEngine;

public class Lootable : MonoBehaviour
{
    

    public void PickUp()
    {
        Debug.Log("Picked up: " + gameObject.name);
        Destroy(gameObject);
    }
}
