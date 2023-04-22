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
    private bool buttonEnabled;

    public void AddWriter(TMP_Text uiText, string textToWrite, float timePerCharacter, bool invisCharacter)
    {
        this.uiText = uiText;
        this.textToWrite = textToWrite;
        this.timePerCharacter = timePerCharacter;
        this.invisCharacter = invisCharacter;
        charaterIndex = 0;
        buttonEnabled = false;
    }

    private void Update()
    {
        if (uiText)
        {
            timer -= Time.deltaTime;
            while (timer <= 0f && textToWrite != null)
            {
                timer += timePerCharacter;
                charaterIndex++;
                string text = textToWrite.Substring(0, charaterIndex);
                if (invisCharacter)
                {
                    text += "<color=#00000000>" + textToWrite.Substring(charaterIndex) + "</color>";
                }
                uiText.text = text;
                if (charaterIndex >= textToWrite.Length)
                {
                    uiText = null;
                    return;
                }
            }
        }

        if ((!buttonEnabled && Time.timeSinceLevelLoad > 25f) || Input.GetKeyDown("space"))
        {
            button.SetActive(true);
            buttonEnabled = true;
        }
    }
}
