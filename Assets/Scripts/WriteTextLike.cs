using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WriteTextLike : MonoBehaviour
{
    [SerializeField] private TextWriter textWriter;
    [SerializeField] private string textWrite;
    private TMP_Text messageText;
    private void Awake() {
        messageText=GameObject.Find("TextZone").GetComponent<TMP_Text>(); 
        
    }
    private void Start() {
        //messageText.text="Hello World";
        if(messageText)
        {
            Debug.Log("am text");
        }
        StartCoroutine(WaitBeforeShow());
        
    }
    IEnumerator WaitBeforeShow()
    {
        yield return new WaitForSeconds(0.5f);
        textWriter.AddWriter(messageText,textWrite,.05f,true);
    }
}
