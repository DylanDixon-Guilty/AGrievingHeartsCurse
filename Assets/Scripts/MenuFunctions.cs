using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Will handle the basic functions of the menus
/// </summary>
public class MenuFunctions : MonoBehaviour
{
    public void NewScene(string nameOfScene)
    {
        SceneManager.LoadScene(nameOfScene);
    }
}
