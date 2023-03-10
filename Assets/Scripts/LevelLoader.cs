using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
   public void openLevel(int i)
   {
    SceneManager.LoadScene(i);
   }
}
