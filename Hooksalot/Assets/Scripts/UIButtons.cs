using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name.ToString());
        GameManager.playerIsDead = false;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        GameManager.playerIsDead = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
