using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerDataSave : MonoBehaviour
{
    public static int doorKey = 0;
    private void Start()
    {
        LoadData();
    }
    public static void SaveData()
    {
        string filePath = Application.persistentDataPath + "/playerData.txt";
        using (StreamWriter writer = new(filePath))
        {
            writer.WriteLine(doorKey.ToString());
        }
        Debug.Log("Player data saved.");
    }

    public static void LoadData()
    {
        string filePath = Application.persistentDataPath + "/playerData.txt";
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new(filePath))
            {
                string doorsString = reader.ReadLine();
                if (int.TryParse(doorsString, out int ledoors))
                {
                    doorKey = ledoors;
                }
            }
            Debug.Log("Player data loaded.");
        }
        else
        {
            Debug.Log("No player data found.");
        }
    }
    public static void ResetData()
    {
        doorKey = 0;
        SaveData();
    }
    public static void ChangeKeyTo(int newKey)
    {
        doorKey = newKey;
        SaveData();
    }
}
