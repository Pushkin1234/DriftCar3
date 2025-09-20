using System.Collections.Generic;
using UnityEngine;

public class DataModule : BaseGameModule
{
    public override string ModuleName => "Data";
    
    [System.Serializable]
    public class GameData
    {
        public int coins = 0;
        public int recordDriftScore = 0;
        public int appliedCarIndex = 0;
        public bool muteMusic = false;
        public List<bool> isBuyShop = new List<bool>() {true, false, false, false, false};
    }
    
    private GameData _data = new GameData();
    private const string SAVE_KEY = "GameData";
    
    public GameData Data => _data;
    
    public override void Initialize()
    {
        LoadData();
        base.Initialize();
    }
    
    public void SaveData()
    {
        var json = JsonUtility.ToJson(_data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            var json = PlayerPrefs.GetString(SAVE_KEY);
            _data = JsonUtility.FromJson<GameData>(json);
        }
    }
}