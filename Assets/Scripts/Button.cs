using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
   
    public Animator animationFinale;
    public Animator musicAnim;
    public AudioSource soundPlayer;
    void Start()
    {
         
=======

public class Button : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
>>>>>>> Temp2
    }

    // Update is called once per frame
    void Update()
    {
        
    }
<<<<<<< HEAD
    public void NextScene(int i) {
        Debug.Log("A mers");
        StartCoroutine(LoadScene(i));
    }
    IEnumerator LoadScene(int i)
    {
        soundPlayer.Play();
        animationFinale.SetTrigger("end");
        musicAnim.SetTrigger("fade");
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(i);
    }
    public void ExitGame()
=======
    public void QuitGame()
>>>>>>> Temp2
    {
        Debug.Log("PAPA");
        Application.Quit();
    }
}
