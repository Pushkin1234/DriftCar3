using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataModule : BaseGameModule, IPersistentModule
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
        
        // Кастомизация для каждой машины (индекс соответствует индексу машины в магазине)
        public string customizationDataJson = ""; // JSON с Dictionary<int, CarCustomizationData>
    }
    
    [System.Serializable]
    public class CarCustomizationData
    {
        public int carIndex = 0;
        public string colorR = "1";
        public string colorG = "1";
        public string colorB = "1";
        public string colorA = "1";
        public int selectedWheelIndex = 0;
        public int selectedSpoilerIndex = -1;
        public int engineLevel = 0;
        public int brakeLevel = 0;
        public int nitroLevel = 0;
        public int handlingLevel = 0;
        public string unlockedColorsJson = ""; // JSON массив bool
        public string unlockedEngineLevelsJson = "";
        public string unlockedBrakeLevelsJson = "";
        public string unlockedNitroLevelsJson = "";
        public string unlockedHandlingLevelsJson = "";
        public string unlockedSpoilersJson = "";
        public string unlockedWheelsJson = "";
    }
    
    private GameData _data = new GameData();
    private const string SAVE_KEY = "GameData";
    
    public GameData Data => _data;
    
    public override void Initialize()
    {
        LoadData();
        base.Initialize();
    }

    public override void Update()
    {
        ResetData();
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
    
    /// <summary>
    /// Сбросить все сохранения игры к значениям по умолчанию
    /// </summary>
    public void ResetData()
    {
        Debug.LogWarning("[DataModule] Сброс всех сохранений!");
        
        // Удаляем все сохранения из PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Создаем новые данные по умолчанию
        _data = new GameData();
        
        // Также очищаем кастомизацию всех машин
        var customizationModule = ModuleManager.Instance?.GetModule<CustomizationModule>();
        if (customizationModule != null)
        {
            // Сбрасываем кастомизацию для всех 5 машин
            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.DeleteKey($"CarCustomization_{i}");
            }
        }
        
        // Сохраняем новые данные
        SaveData();
        
        Debug.Log("[DataModule] ✅ Все сохранения успешно сброшены!");
    }
}