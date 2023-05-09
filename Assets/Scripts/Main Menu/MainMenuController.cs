using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
