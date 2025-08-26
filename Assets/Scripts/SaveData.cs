using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData Instance;

    [SerializeField] private DataHolder _data;

    public DataHolder Data => _data;

    private const string _saveFileName = "save.json";

    private string SaveFilePath => Path.Combine(Application.dataPath, _saveFileName);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (_data == null) _data = new DataHolder();
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _data = new DataHolder();
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to delete save file: {e}");
            }
            Save();
        }
    }
    private void OnDisable()
    {
        Save();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                var json = File.ReadAllText(SaveFilePath);
                var data = JsonUtility.FromJson<DataHolder>(json);
                if (data != null)
                {
                    _data = data;
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load save file: {e}");
        }
        if (_data == null)
        {
            _data = new DataHolder();
        }
    }

    public void SaveYandex()
    {
        Save();
    }

    public void Save()
    {
        try
        {
            var json = JsonUtility.ToJson(_data, true);
            var directory = Path.GetDirectoryName(SaveFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save file: {e}");
        }
    }

    public void SaveLeaderBoard()
    {

    }

 
}

[Serializable]

public class DataHolder
{
    public string DeyTime;

    public int Coins = 0;

    public int RecordDriftScore = 0;

    public int AppliedCarIndex = 0;

    public bool muteMusic = false;

    public List<bool> IsBuyShop = new List<bool>(); {true, false, false, false};
}


