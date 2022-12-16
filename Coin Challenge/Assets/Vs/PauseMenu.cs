using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUi;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        void Resume()
        {
            pauseMenuUi.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
        void Pause()
        {
            pauseMenuUi.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }
    public void QuitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ResumeButton()
    {
        pauseMenuUi.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}