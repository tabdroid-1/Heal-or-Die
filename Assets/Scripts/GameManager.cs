using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("GameState")]
    [Space]
    public int highestScore;
    public int currentHighScore;


    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (currentHighScore > highestScore)
        {
            highestScore = currentHighScore;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int highestScore;
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.highestScore = highestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highestScore = data.highestScore;
        }
    }
}
