using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWriter : MonoBehaviour
{
    private TMP_Text uiText;
    private string textToWrite;
    private float timePerCharacter;
    private float timer;
    private int charaterIndex;
    private bool invisCharacter;
    private bool pressed;
    [SerializeField] private GameObject button;
    public void AddWriter(TMP_Text uiText, string textToWrite, float timePerCharacter, bool invisCharacter )
    {
        this.uiText = uiText;
        this.textToWrite= textToWrite;
        this.timePerCharacter = timePerCharacter;
        this.invisCharacter =invisCharacter;
        charaterIndex=0;
    }
    private void Update() {
        if(uiText)
        {
            timer-= Time.deltaTime;
            while(timer<=0f && textToWrite!=null )
            {
                timer+=timePerCharacter;
                charaterIndex++;
                string text=textToWrite.Substring(0,charaterIndex);
                if(invisCharacter)
                {
                    text+="<color=#00000000>"+textToWrite.Substring(charaterIndex)+"</color>";
                }
                uiText.text= text;
                if(charaterIndex>=textToWrite.Length)
                {
                    uiText=null;
                    if(button.activeInHierarchy==false) {
                button.SetActive(true);
            }
                    return ;
                }
            }
        }
        if(Input.GetKeyDown("space")){
            WrtieAllAndeDestroy();
            Debug.Log("merge");
            return ;
        }
    }
    public void WrtieAllAndeDestroy()
    {
        if(!pressed)
        {
            timer=-0.1f;
        uiText.text=textToWrite;
        charaterIndex=textToWrite.Length-1;
        pressed=true;
        if(button.activeInHierarchy==false) {
                button.SetActive(true);
            }
        }
        
    }
}
