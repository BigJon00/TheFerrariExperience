using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadMain : MonoBehaviour
{
 public string MainSccene; // Assign the scene name in the Inspector

        public void Load()
        {
            SceneManager.LoadScene(MainSccene);
        }

        
}
