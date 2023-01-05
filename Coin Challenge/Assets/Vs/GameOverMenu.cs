using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
    [SerializeField] GameObject gameOverMenu, gui;
    public bool gameOver = false;
    void Start()
    {
        gameOver = false;
    }
    void Update()
    {
        CheckGameOver();
        if (gameOver == true)
        {
            gameOverMenu.SetActive(true);
            gui.SetActive(false);
        }
    }
    void CheckGameOver()
    {
        if (ScoreManager.Instance.remaningTime <= 0)
        {
            gameOver = true;
        }
        else return;
    }
    public void QuitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
    public void RetryButton()
    {
        SceneManager.LoadScene(1);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}