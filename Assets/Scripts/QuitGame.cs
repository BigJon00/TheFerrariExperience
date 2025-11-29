using UnityEngine;
using UnityEngine.SceneManagement;
public class QuitGame : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void Quiting()
    {
        // This will quit the application when run as a built game.
        // It will be ignored in the Unity Editor's Play Mode.
        Application.Quit();

        // For testing in the Unity Editor, you can use the following line.
        // This line should be wrapped in #if UNITY_EDITOR directives
        // to ensure it's only included when compiling for the editor.
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        //Debug.Log("Game is exiting."); // Optional: for debugging purposes
    }
    
    // This function demonstrates quitting with a key press (e.g., Escape key)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quiting(); // Call the QuitGame function
        }
    }
}
