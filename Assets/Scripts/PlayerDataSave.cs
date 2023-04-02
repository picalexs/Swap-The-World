using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataSave : MonoBehaviour
{
    public int doorsUnlocked=1;
    [SerializeField] public bool isLevelDoor;
    [SerializeField] public bool isExitDoor;
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("LevelDoor"))
        {
            isLevelDoor=true;
            isExitDoor=false;
            Debug.Log("on");
        }
        else
        if(collision.gameObject.CompareTag("ExitDoor"))
        {
            isLevelDoor=false;
            isExitDoor=true;
            Debug.Log("on");
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("LevelDoor")|| collision.gameObject.CompareTag("ExitDoor"))
        {
            isExitDoor=isLevelDoor=false;
            Debug.Log("off");
        }
    }
    private void Start()
    {
        //ResetData();
        LoadData();
    }
    public void SaveData()
    {
        string filePath = Application.persistentDataPath + "/playerData.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(doorsUnlocked.ToString());
        }
        Debug.Log("Player data saved.");
    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/playerData.txt";
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string doorsString = reader.ReadLine();
                if (int.TryParse(doorsString, out int ledoors))
                {
                    doorsUnlocked = ledoors;
                }
            }
            Debug.Log("Player data loaded.");
        }
        else
        {
            Debug.Log("No player data found.");
        }
    }
    public void ResetData()
{
    doorsUnlocked = 1;
    SaveData();
}
}
