using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public void NewScene(string nameOfScene)
    {
        SceneManager.LoadScene(nameOfScene);
    }
}
