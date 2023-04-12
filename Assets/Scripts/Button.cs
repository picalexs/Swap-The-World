using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
   
    public Animator animationFinale;
    public Animator musicAnim;
    public AudioSource soundPlayer;
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
    {
        Debug.Log("PAPA");
        Application.Quit();
    }
}
