using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    #region Fields in inspector

    public static bool gameNotStarted = true;
    public GameObject startMenuUI;
    public GameObject countdownText;
    public GameObject lapsAndPositionUI;
    #endregion

    #region Methods

    void FixedUpdate()
    {
        if (gameNotStarted)
            Time.timeScale = 0f;
    }

    public void StartGame()
    {
        startMenuUI.SetActive(false);
        countdownText.SetActive(true);
        lapsAndPositionUI.SetActive(true);
        Time.timeScale = 1f;
        gameNotStarted = false;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    #endregion
}
