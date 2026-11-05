using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Will handle the basic functions of the menus. Like Options or Main Menu
/// </summary>
public class MenuFunctions : MonoBehaviour
{
    public void NewScene(string nameOfScene)
    {
        SceneManager.LoadScene(nameOfScene);
    }
}
