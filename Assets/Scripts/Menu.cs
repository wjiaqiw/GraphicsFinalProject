using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartGameButton()
    {
        // Load the main game scene
        SceneManager.LoadScene("MainScene");
    }
}
