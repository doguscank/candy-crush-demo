using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
