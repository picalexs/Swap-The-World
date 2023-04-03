using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [SerializeField] RectTransform fader;
    private void Start()
    {
        if (fader == null)
        {
            Debug.LogWarning("Fader is null!");
            return;
        }
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
        LeanTween.scale(fader, Vector3.zero, 0.5f).setOnComplete(() =>
        {
            fader.gameObject.SetActive(false);
        });
       
    }
    public void OpenGameScene(int i)
    {
        PlayerDataSave.SaveData();
        SceneManager.LoadScene(i);
        if (fader == null)
        {
            Debug.LogWarning("Fader is null!");
            return;
        }
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, Vector3.zero, 0f);
        LeanTween.scale(fader, new Vector3(1, 1, 1), 0.5f).setOnComplete(() =>
        {
            SceneManager.LoadScene(i);
        });

    }
}
