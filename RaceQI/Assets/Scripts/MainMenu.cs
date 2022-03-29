using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Methods

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    #endregion
}
