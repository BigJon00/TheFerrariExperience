using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class Pause : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu; 
    // Start is called before the first frame update
    public void Paused()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;  
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale =1f; 
    }
    public void Home()
    {
        Time.timeScale =1f; 
        SceneManager.LoadScene("TitleScreen");
    }
}
