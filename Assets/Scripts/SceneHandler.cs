using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void PlayTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void PlayMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void PlayTest()
    {
        SceneManager.LoadScene("TestTrack");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
