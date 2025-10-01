using System.Collections.Generic;
using UnityEngine;

public class UpgradeModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "Upgrade";
    
    [System.Serializable]
    public class CarUpgrades
    {
        public string carModelName = "";
        public int engineLevel = 0;
        public int brakeLevel = 0;
        public int handlingLevel = 0;
        public int NitroLevel = 0;
        public int maxLevel = 5;
        
        // Базовые характеристики (устанавливаются при первой инициализации)
        public float baseEngineTorque = 400f;
        public float baseBrakeTorque = 2500f;
        public float baseHandlingStrength = 0.2f;
        public float baseNitroStrength = 0.1f;
        
        // Максимальные характеристики
        public float maxEngineTorque = 800f;
        public float maxBrakeTorque = 4500f;
        public float maxHandlingStrength = 0.6f;
        public float maxNitroStrength = 0.5f;
        
        public bool IsMaxLevel(UpgradeType upgradeType)
        {
            return GetUpgradeLevel(upgradeType) >= maxLevel;
        }
        
        public int GetUpgradeLevel(UpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case UpgradeType.Engine: return engineLevel;
                case UpgradeType.Brake: return brakeLevel;
                case UpgradeType.Handling: return handlingLevel;
                case UpgradeType.Nitro: return NitroLevel;
                default: return 0;
            }
        }
        
        public void SetUpgradeLevel(UpgradeType upgradeType, int level)
        {
            level = Mathf.Clamp(level, 0, maxLevel);
            
            switch (upgradeType)
            {
                case UpgradeType.Engine: engineLevel = level; break;
                case UpgradeType.Brake: brakeLevel = level; break;
                case UpgradeType.Handling: handlingLevel = level; break;
                case UpgradeType.Nitro: NitroLevel = level; break;
            }
        }
        
        public float GetCurrentValue(UpgradeType upgradeType)
        {
            float progress = GetUpgradeLevel(upgradeType) / (float)maxLevel;
            
            switch (upgradeType)
            {
                case UpgradeType.Engine: 
                    return Mathf.Lerp(baseEngineTorque, maxEngineTorque, progress);
                case UpgradeType.Brake: 
                    return Mathf.Lerp(baseBrakeTorque, maxBrakeTorque, progress);
                case UpgradeType.Handling: 
                    return Mathf.Lerp(baseHandlingStrength, maxHandlingStrength, progress);
                case UpgradeType.Nitro: 
                    return Mathf.Lerp(baseNitroStrength, maxNitroStrength, progress);
                default: return 0f;
            }
        }
    }
    
    public enum UpgradeType
    {
        Engine,
        Brake,
        Handling,
        Nitro
    }
    
    [System.Serializable]
    public class UpgradeCost
    {
        public UpgradeType upgradeType;
        public int[] levelCosts = { 100, 200, 400, 800, 1600 }; // Стоимость для каждого уровня
        
        public int GetCostForLevel(int level)
        {
            if (level < 0 || level >= levelCosts.Length)
                return int.MaxValue; // Недоступно для прокачки
                
            return levelCosts[level];
        }
    }
    
    private DataModule _dataModule;
    private Dictionary<string, CarUpgrades> _carUpgrades = new Dictionary<string, CarUpgrades>();
    
    [Header("Upgrade Configuration")]
    public UpgradeCost[] upgradeCosts = {
        new UpgradeCost { upgradeType = UpgradeType.Engine, levelCosts = new int[] { 100, 200, 400, 800, 1600 } },
        new UpgradeCost { upgradeType = UpgradeType.Brake, levelCosts = new int[] { 80, 160, 320, 640, 1280 } },
        new UpgradeCost { upgradeType = UpgradeType.Handling, levelCosts = new int[] { 120, 240, 480, 960, 1920 } },
        new UpgradeCost { upgradeType = UpgradeType.Nitro, levelCosts = new int[] { 150, 300, 600, 1200, 2400 } }
    };
    
    // События для уведомления других систем
    public System.Action<string, UpgradeType, int> OnCarUpgraded;
    public System.Action<string, UpgradeType, bool> OnUpgradeAvailabilityChanged;
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadUpgrades();
        base.Initialize();
    }
    
    /// <summary>
    /// Прокачивает характеристику машины
    /// </summary>
    public bool UpgradeCar(string carModelName, UpgradeType upgradeType)
    {
        if (!_carUpgrades.ContainsKey(carModelName))
        {
            InitializeCarUpgrades(carModelName);
        }
        
        var carUpgrade = _carUpgrades[carModelName];
        int currentLevel = carUpgrade.GetUpgradeLevel(upgradeType);
        
        // Проверяем максимальный уровень
        if (carUpgrade.IsMaxLevel(upgradeType))
        {
            Debug.LogWarning($"Car {carModelName} {upgradeType} is already at max level!");
            return false;
        }
        
        // Проверяем стоимость
        int cost = GetUpgradeCost(upgradeType, currentLevel);
        if (_dataModule.Data.coins < cost)
        {
            Debug.LogWarning($"Not enough coins to upgrade {carModelName} {upgradeType}. Need: {cost}, Have: {_dataModule.Data.coins}");
            return false;
        }
        
        // Выполняем прокачку
        _dataModule.Data.coins -= cost;
        carUpgrade.SetUpgradeLevel(upgradeType, currentLevel + 1);
        
        // Применяем изменения к активной машине
        ApplyUpgradeToActiveCar(carModelName, upgradeType);
        
        // Уведомляем подписчиков
        OnCarUpgraded?.Invoke(carModelName, upgradeType, carUpgrade.GetUpgradeLevel(upgradeType));
        
        // Проверяем доступность дальнейших улучшений
        CheckUpgradeAvailability(carModelName, upgradeType);
        
        // Сохраняем изменения
        SaveUpgrades();
        _dataModule.SaveData();
        
        Debug.Log($"Upgraded {carModelName} {upgradeType} to level {carUpgrade.GetUpgradeLevel(upgradeType)}");
        return true;
    }
    
    /// <summary>
    /// Получает стоимость прокачки
    /// </summary>
    public int GetUpgradeCost(UpgradeType upgradeType, int currentLevel)
    {
        foreach (var cost in upgradeCosts)
        {
            if (cost.upgradeType == upgradeType)
            {
                return cost.GetCostForLevel(currentLevel);
            }
        }
        return int.MaxValue;
    }
    
    /// <summary>
    /// Проверяет, можно ли прокачать характеристику
    /// </summary>
    public bool CanUpgrade(string carModelName, UpgradeType upgradeType)
    {
        if (!_carUpgrades.ContainsKey(carModelName))
            return true; // Можно начать прокачку
            
        var carUpgrade = _carUpgrades[carModelName];
        if (carUpgrade.IsMaxLevel(upgradeType))
            return false;
            
        int cost = GetUpgradeCost(upgradeType, carUpgrade.GetUpgradeLevel(upgradeType));
        return _dataModule.Data.coins >= cost;
    }
    
    /// <summary>
    /// Получает прокачки для определенной машины
    /// </summary>
    public CarUpgrades GetCarUpgrades(string carModelName)
    {
        if (!_carUpgrades.ContainsKey(carModelName))
        {
            InitializeCarUpgrades(carModelName);
        }
        return _carUpgrades[carModelName];
    }
    
    /// <summary>
    /// Применяет прокачки к машине при спавне
    /// </summary>
    public void ApplyUpgradesToCar(GameObject carObject, string carModelName)
    {
        var upgrades = GetCarUpgrades(carModelName);
        
        // Применяем через RCC систему
        var customizationApplier = carObject.GetComponent<RCC_CustomizationApplier>();
        if (customizationApplier != null && customizationApplier.UpgradeManager != null)
        {
            // Устанавливаем уровни в RCC систему
            customizationApplier.loadout.engineLevel = upgrades.engineLevel;
            customizationApplier.loadout.brakeLevel = upgrades.brakeLevel;
            customizationApplier.loadout.handlingLevel = upgrades.handlingLevel;
            // Примечание: RCC не поддерживает nitroLevel из коробки
            
            // Инициализируем RCC систему
            customizationApplier.UpgradeManager.Initialize();
        }
        else
        {
            // Альтернативный способ - применяем напрямую к CarController
            var carController = carObject.GetComponent<RCC_CarControllerV3>();
            if (carController != null)
            {
                carController.maxEngineTorque = upgrades.GetCurrentValue(UpgradeType.Engine);
                carController.brakeTorque = upgrades.GetCurrentValue(UpgradeType.Brake);
                carController.tractionHelperStrength = upgrades.GetCurrentValue(UpgradeType.Handling);
            }
            
            // Применяем нитро через кастомную систему
            var nitroSystem = carObject.GetComponent<NitroModule>();
            if (nitroSystem != null)
            {
                nitroSystem.SetCarModel(carModelName);
            }
        }
    }
    
    private void InitializeCarUpgrades(string carModelName)
    {
        var newUpgrade = new CarUpgrades();
        newUpgrade.carModelName = carModelName;
        
        // Получаем базовые характеристики из RCC (если доступно)
        var activeVehicle = FindObjectOfType<RCC_CarControllerV3>();
        if (activeVehicle != null && activeVehicle.name.Contains(carModelName))
        {
            newUpgrade.baseEngineTorque = activeVehicle.maxEngineTorque;
            newUpgrade.baseBrakeTorque = activeVehicle.brakeTorque;
            newUpgrade.baseHandlingStrength = activeVehicle.tractionHelperStrength;
        }
        
        _carUpgrades[carModelName] = newUpgrade;
    }
    
    private void ApplyUpgradeToActiveCar(string carModelName, UpgradeType upgradeType)
    {
        var activeVehicle = FindObjectOfType<RCC_CarControllerV3>();
        if (activeVehicle != null && activeVehicle.name.Contains(carModelName))
        {
            ApplyUpgradesToCar(activeVehicle.gameObject, carModelName);
        }
    }
    
    private void CheckUpgradeAvailability(string carModelName, UpgradeType upgradeType)
    {
        bool canUpgrade = CanUpgrade(carModelName, upgradeType);
        OnUpgradeAvailabilityChanged?.Invoke(carModelName, upgradeType, canUpgrade);
    }
    
    private void SaveUpgrades()
    {
        if (_dataModule != null)
        {
            var json = JsonUtility.ToJson(_carUpgrades);
            PlayerPrefs.SetString("CarUpgrades", json);
        }
    }
    
    private void LoadUpgrades()
    {
        if (PlayerPrefs.HasKey("CarUpgrades"))
        {
            try
            {
                var json = PlayerPrefs.GetString("CarUpgrades");
                _carUpgrades = JsonUtility.FromJson<Dictionary<string, CarUpgrades>>(json) 
                             ?? new Dictionary<string, CarUpgrades>();
            }
            catch
            {
                _carUpgrades = new Dictionary<string, CarUpgrades>();
            }
        }
    }
    
    /// <summary>
    /// Сбрасывает все прокачки машины (для тестирования)
    /// </summary>
    public void ResetCarUpgrades(string carModelName)
    {
        if (_carUpgrades.ContainsKey(carModelName))
        {
            _carUpgrades[carModelName] = new CarUpgrades { carModelName = carModelName };
            SaveUpgrades();
            
            // Применяем сброс к активной машине
            ApplyUpgradeToActiveCar(carModelName, UpgradeType.Engine);
        }
    }
}
