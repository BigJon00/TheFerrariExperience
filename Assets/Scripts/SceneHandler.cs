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
        SceneManager.LoadScene("MainSccene");
    }

    public void PlayTest()
    {
        SceneManager.LoadScene("TestTrack");
    }
    
    public void PlayEnd()
    {
        SceneManager.LoadScene("End_Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
