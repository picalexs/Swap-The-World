using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
   
    public Animator animationFinale;
    public Animator musicAnim;
    public AudioSource soundPlayer;
    [SerializeField] public PlayerDataSave saveManager;
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
        if(i==1)
        {
            PlayerDataSave.ResetData();
            Debug.Log("resetam Datele");
        }
        else 
        {   
            PlayerDataSave.LoadData();
            Debug.Log("incarcam datele");
        } 
        if(!(i==3 && PlayerDataSave.doorKey==0))
           { 
        soundPlayer.Play();
        animationFinale.SetTrigger("end");
        //musicAnim.SetTrigger("fade");
        yield return new WaitForSeconds(0.6f);
            SceneManager.LoadScene(i);
            }
        
    }
    public void ExitGame()
    {
        Debug.Log("PAPA");
        PlayerDataSave.SaveData();
        Application.Quit();
    }
}
