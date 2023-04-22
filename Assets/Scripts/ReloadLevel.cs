using UnityEngine;

public class ReloadLevel : MonoBehaviour
{
    public KeyCode resetKey = KeyCode.F4;

    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
