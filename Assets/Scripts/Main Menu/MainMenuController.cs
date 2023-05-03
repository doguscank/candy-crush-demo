using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{

    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
