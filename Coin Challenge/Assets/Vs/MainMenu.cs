using UnityEngine.SceneManagement;
using UnityEngine;
public class MainMenu : MonoBehaviour
{
    void LoadScene(string ScenePrincipale)
    {
        SceneManager.LoadScene("ScenePrincipale");
    }
    void QuitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}