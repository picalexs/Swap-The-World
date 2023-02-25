using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
   [SerializeField] RectTransform fader;
   private void Start() {
      {
         fader.gameObject.SetActive(true);
         LeanTween.scale(fader,new Vector3(1,1,1),0);
         LeanTween.scale(fader,Vector3.zero,0.5f).setOnComplete(()=>{
            fader.gameObject.SetActive(false);
         });
      }
   }
   public void OpenMenuScene()
   {
      fader.gameObject.SetActive(true);
         LeanTween.scale(fader,Vector3.zero,0f);
         LeanTween.scale(fader,new Vector3 (1,1,1),0.5f).setOnComplete(()=>{
            SceneManager.LoadScene(0);
         });
      
   }
    public void OpenGameScene(int i)
   {
      fader.gameObject.SetActive(true);
         LeanTween.scale(fader,Vector3.zero,0f);
         LeanTween.scale(fader,new Vector3 (1,1,1),0.5f).setOnComplete(()=>{
            SceneManager.LoadScene(i);
         });
      
   }
}
