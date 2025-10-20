using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Модуль улучшения характеристик машин.
/// Отвечает ТОЛЬКО за прокачку: двигатель, тормоза, нитро, управляемость.
/// </summary>
public class PerformanceUpgradeModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "PerformanceUpgrade";
    
    #region Data Structures
    
    [System.Serializable]
    public class UpgradeData
    {
        public string upgradeName = "";
        public int level = 0;
        public int price = 0;
        public float powerMultiplier = 1.0f; // Множитель мощности
        public string description = "";
        public bool isUnlocked = false;
    }
    
    [System.Serializable]
    public class CarPerformanceData
    {
        public int engineLevel = 0;
        public int brakeLevel = 0;
        public int nitroLevel = 0;
        public int handlingLevel = 0;
        
        // Разблокированные уровни улучшений
        public bool[] unlockedEngineLevels = new bool[5];
        public bool[] unlockedBrakeLevels = new bool[5];
        public bool[] unlockedNitroLevels = new bool[5];
        public bool[] unlockedHandlingLevels = new bool[5];
        
        public CarPerformanceData()
        {
            // Базовые уровни (0) разблокированы по умолчанию
            unlockedEngineLevels[0] = true;
            unlockedBrakeLevels[0] = true;
            unlockedNitroLevels[0] = true;
            unlockedHandlingLevels[0] = true;
        }
    }
    
    #endregion
    
    #region Upgrade Configurations
    
    [Header("Engine Upgrades")]
    public UpgradeData[] engineUpgrades = {
        new UpgradeData { upgradeName = "Стандартный", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовый двигатель", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенный", level = 1, price = 1000, powerMultiplier = 1.2f, description = "+20% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивный", level = 2, price = 2500, powerMultiplier = 1.5f, description = "+50% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Турбо", level = 3, price = 5000, powerMultiplier = 1.8f, description = "+80% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Максимальный", level = 4, price = 10000, powerMultiplier = 2.2f, description = "+120% мощности", isUnlocked = false }
    };
    
    [Header("Brake Upgrades")]
    public UpgradeData[] brakeUpgrades = {
        new UpgradeData { upgradeName = "Стандартные", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовые тормоза", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенные", level = 1, price = 800, powerMultiplier = 1.3f, description = "+30% эффективности", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивные", level = 2, price = 2000, powerMultiplier = 1.6f, description = "+60% эффективности", isUnlocked = false },
        new UpgradeData { upgradeName = "Керамические", level = 3, price = 4000, powerMultiplier = 2.0f, description = "+100% эффективности", isUnlocked = false },
        new UpgradeData { upgradeName = "Карбоновые", level = 4, price = 8000, powerMultiplier = 2.5f, description = "+150% эффективности", isUnlocked = false }
    };
    
    [Header("Nitro Upgrades")]
    public UpgradeData[] nitroUpgrades = {
        new UpgradeData { upgradeName = "Стандартный", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовый нитро", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенный", level = 1, price = 1200, powerMultiplier = 1.4f, description = "+40% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивный", level = 2, price = 3000, powerMultiplier = 1.8f, description = "+80% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Турбо", level = 3, price = 6000, powerMultiplier = 2.3f, description = "+130% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Максимальный", level = 4, price = 12000, powerMultiplier = 2.8f, description = "+180% мощности", isUnlocked = false }
    };
    
    [Header("Handling Upgrades")]
    public UpgradeData[] handlingUpgrades = {
        new UpgradeData { upgradeName = "Стандартная", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовая управляемость", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенная", level = 1, price = 900, powerMultiplier = 1.25f, description = "+25% управляемости", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивная", level = 2, price = 2200, powerMultiplier = 1.55f, description = "+55% управляемости", isUnlocked = false },
        new UpgradeData { upgradeName = "Гоночная", level = 3, price = 4500, powerMultiplier = 1.9f, description = "+90% управляемости", isUnlocked = false },
        new UpgradeData { upgradeName = "Профессиональная", level = 4, price = 9000, powerMultiplier = 2.4f, description = "+140% управляемости", isUnlocked = false }
    };
    
    #endregion
    
    #region Private Fields
    
    private DataModule _dataModule;
    private Dictionary<int, CarPerformanceData> _carPerformanceData = new Dictionary<int, CarPerformanceData>(); // По индексу машины (0-4)
    
    #endregion
    
    #region Events
    
    public System.Action<int, int> OnEngineUpgraded;    // (carIndex, level)
    public System.Action<int, int> OnBrakeUpgraded;     // (carIndex, level)
    public System.Action<int, int> OnNitroUpgraded;     // (carIndex, level)
    public System.Action<int, int> OnHandlingUpgraded;  // (carIndex, level)
    
    #endregion
    
    #region Initialization
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadAllCarPerformanceData();
        base.Initialize();
        
        Debug.Log($"[{ModuleName}] Инициализирован. Доступно 4 типа улучшений по 5 уровней.");
    }
    
    private void LoadAllCarPerformanceData()
    {
        for (int i = 0; i < 5; i++)
        {
            LoadCarPerformanceData(i);
        }
    }
    
    private void LoadCarPerformanceData(int carIndex)
    {
        string key = $"CarPerformance_{carIndex}";
        
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            _carPerformanceData[carIndex] = JsonUtility.FromJson<CarPerformanceData>(json);
        }
        else
        {
            _carPerformanceData[carIndex] = new CarPerformanceData();
        }
    }
    
    #endregion
    
    #region Upgrade Info
    
    public int GetEngineUpgradeCount() => engineUpgrades.Length;
    public int GetBrakeUpgradeCount() => brakeUpgrades.Length;
    public int GetNitroUpgradeCount() => nitroUpgrades.Length;
    public int GetHandlingUpgradeCount() => handlingUpgrades.Length;
    
    public UpgradeData GetEngineUpgradeData(int level) => GetUpgradeData(engineUpgrades, level);
    public UpgradeData GetBrakeUpgradeData(int level) => GetUpgradeData(brakeUpgrades, level);
    public UpgradeData GetNitroUpgradeData(int level) => GetUpgradeData(nitroUpgrades, level);
    public UpgradeData GetHandlingUpgradeData(int level) => GetUpgradeData(handlingUpgrades, level);
    
    private UpgradeData GetUpgradeData(UpgradeData[] upgrades, int level)
    {
        if (level < 0 || level >= upgrades.Length)
            return null;
        return upgrades[level];
    }
    
    #endregion
    
    #region Unlock Status
    
    public bool IsEngineUpgradeUnlocked(int carIndex, int level)
    {
        return IsUpgradeUnlocked(carIndex, level, data => data.unlockedEngineLevels);
    }
    
    public bool IsBrakeUpgradeUnlocked(int carIndex, int level)
    {
        return IsUpgradeUnlocked(carIndex, level, data => data.unlockedBrakeLevels);
    }
    
    public bool IsNitroUpgradeUnlocked(int carIndex, int level)
    {
        return IsUpgradeUnlocked(carIndex, level, data => data.unlockedNitroLevels);
    }
    
    public bool IsHandlingUpgradeUnlocked(int carIndex, int level)
    {
        return IsUpgradeUnlocked(carIndex, level, data => data.unlockedHandlingLevels);
    }
    
    private bool IsUpgradeUnlocked(int carIndex, int level, System.Func<CarPerformanceData, bool[]> getArray)
    {
        if (!_carPerformanceData.ContainsKey(carIndex))
            return false;
        
        var array = getArray(_carPerformanceData[carIndex]);
        if (level < 0 || level >= array.Length)
            return false;
        
        return array[level];
    }
    
    #endregion
    
    #region Current Levels
    
    public int GetEngineLevel(int carIndex) => _carPerformanceData.ContainsKey(carIndex) ? _carPerformanceData[carIndex].engineLevel : 0;
    public int GetBrakeLevel(int carIndex) => _carPerformanceData.ContainsKey(carIndex) ? _carPerformanceData[carIndex].brakeLevel : 0;
    public int GetNitroLevel(int carIndex) => _carPerformanceData.ContainsKey(carIndex) ? _carPerformanceData[carIndex].nitroLevel : 0;
    public int GetHandlingLevel(int carIndex) => _carPerformanceData.ContainsKey(carIndex) ? _carPerformanceData[carIndex].handlingLevel : 0;
    
    #endregion
    
    #region Purchase System
    
    public bool PurchaseEngineUpgrade(int carIndex, int level)
    {
        return PurchaseUpgrade(carIndex, level, engineUpgrades, 
            data => data.unlockedEngineLevels,
            (data, l) => data.engineLevel = l,
            OnEngineUpgraded, "Двигатель");
    }
    
    public bool PurchaseBrakeUpgrade(int carIndex, int level)
    {
        return PurchaseUpgrade(carIndex, level, brakeUpgrades,
            data => data.unlockedBrakeLevels,
            (data, l) => data.brakeLevel = l,
            OnBrakeUpgraded, "Тормоза");
    }
    
    public bool PurchaseNitroUpgrade(int carIndex, int level)
    {
        return PurchaseUpgrade(carIndex, level, nitroUpgrades,
            data => data.unlockedNitroLevels,
            (data, l) => data.nitroLevel = l,
            OnNitroUpgraded, "Нитро");
    }
    
    public bool PurchaseHandlingUpgrade(int carIndex, int level)
    {
        return PurchaseUpgrade(carIndex, level, handlingUpgrades,
            data => data.unlockedHandlingLevels,
            (data, l) => data.handlingLevel = l,
            OnHandlingUpgraded, "Управляемость");
    }
    
    private bool PurchaseUpgrade(
        int carIndex, 
        int level, 
        UpgradeData[] upgrades,
        System.Func<CarPerformanceData, bool[]> getArray,
        System.Action<CarPerformanceData, int> setLevel,
        System.Action<int, int> onUpgraded,
        string upgradeName)
    {
        var upgradeData = GetUpgradeData(upgrades, level);
        if (upgradeData == null)
        {
            Debug.LogWarning($"[{ModuleName}] Улучшение {upgradeName} уровня {level} не найдено!");
            return false;
        }
        
        if (!_carPerformanceData.ContainsKey(carIndex))
        {
            _carPerformanceData[carIndex] = new CarPerformanceData();
        }
        
        var perfData = _carPerformanceData[carIndex];
        var unlockedArray = getArray(perfData);
        
        // Проверяем, не куплено ли уже
        if (unlockedArray[level])
        {
            Debug.LogWarning($"[{ModuleName}] {upgradeName} уровня {level} уже разблокирован!");
            return false;
        }
        
        // Проверяем монеты
        if (_dataModule.Data.coins < upgradeData.price)
        {
            Debug.LogWarning($"[{ModuleName}] Недостаточно монет! Нужно: {upgradeData.price}, есть: {_dataModule.Data.coins}");
            return false;
        }
        
        // Списываем монеты
        _dataModule.Data.coins -= upgradeData.price;
        _dataModule.SaveData();
        
        // Разблокируем и применяем
        unlockedArray[level] = true;
        setLevel(perfData, level);
        
        SaveCarPerformanceData(carIndex);
        
        // Уведомляем
        onUpgraded?.Invoke(carIndex, level);
        
        Debug.Log($"[{ModuleName}] Куплен {upgradeName} уровня {level} ({upgradeData.upgradeName}) за {upgradeData.price} монет");
        
        return true;
    }
    
    #endregion
    
    #region Save/Load
    
    private void SaveCarPerformanceData(int carIndex)
    {
        if (!_carPerformanceData.ContainsKey(carIndex))
            return;
        
        string key = $"CarPerformance_{carIndex}";
        string json = JsonUtility.ToJson(_carPerformanceData[carIndex]);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    
    public void SaveAllCarPerformanceData()
    {
        foreach (var kvp in _carPerformanceData)
        {
            SaveCarPerformanceData(kvp.Key);
        }
    }
    
    public void ResetCarPerformanceData(int carIndex)
    {
        _carPerformanceData[carIndex] = new CarPerformanceData();
        SaveCarPerformanceData(carIndex);
    }
    
    #endregion
    
    #region Shutdown
    
    public override void Shutdown()
    {
        SaveAllCarPerformanceData();
        base.Shutdown();
    }
    
    #endregion
}

