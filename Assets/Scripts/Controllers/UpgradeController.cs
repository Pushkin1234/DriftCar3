using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button _engineUpgradeButton;
    [SerializeField] private Button _brakeUpgradeButton;
    [SerializeField] private Button _handlingUpgradeButton;
    [SerializeField] private Button _nitroUpgradeButton;
    [SerializeField] private Button _resetUpgradesButton;
    
    [Header("Level Indicators")]
    [SerializeField] private TextMeshProUGUI _engineLevelText;
    [SerializeField] private TextMeshProUGUI _brakeLevelText;
    [SerializeField] private TextMeshProUGUI _handlingLevelText;
    [SerializeField] private TextMeshProUGUI _nitroLevelText;
    
    [Header("Cost Indicators")]
    [SerializeField] private TextMeshProUGUI _engineCostText;
    [SerializeField] private TextMeshProUGUI _brakeCostText;
    [SerializeField] private TextMeshProUGUI _handlingCostText;
    [SerializeField] private TextMeshProUGUI _nitroCostText;
    
    [Header("Progress Bars")]
    [SerializeField] private Slider _engineProgressBar;
    [SerializeField] private Slider _brakeProgressBar;
    [SerializeField] private Slider _handlingProgressBar;
    [SerializeField] private Slider _nitroProgressBar;
    
    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI _engineStatsText;
    [SerializeField] private TextMeshProUGUI _brakeStatsText;
    [SerializeField] private TextMeshProUGUI _handlingStatsText;
    [SerializeField] private TextMeshProUGUI _nitroStatsText;
    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    
    [Header("Configuration")]
    [SerializeField] private string _targetCarModelName = "SportsCar";
    
    private UpgradeModule _upgradeModule;
    private DataModule _dataModule;
    
    private void Start()
    {
        _upgradeModule = ModuleManager.Instance.GetModule<UpgradeModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        InitializeUI();
        UpdateUI();
        
        // Подписываемся на события
        if (_upgradeModule != null)
        {
            _upgradeModule.OnCarUpgraded += OnCarUpgraded;
            _upgradeModule.OnUpgradeAvailabilityChanged += OnUpgradeAvailabilityChanged;
        }
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        if (_upgradeModule != null)
        {
            _upgradeModule.OnCarUpgraded -= OnCarUpgraded;
            _upgradeModule.OnUpgradeAvailabilityChanged -= OnUpgradeAvailabilityChanged;
        }
    }
    
    private void InitializeUI()
    {
        // Настраиваем кнопки прокачки
        if (_engineUpgradeButton != null)
            _engineUpgradeButton.onClick.AddListener(() => UpgradeCharacteristic(UpgradeModule.UpgradeType.Engine));
            
        if (_brakeUpgradeButton != null)
            _brakeUpgradeButton.onClick.AddListener(() => UpgradeCharacteristic(UpgradeModule.UpgradeType.Brake));
            
        if (_handlingUpgradeButton != null)
            _handlingUpgradeButton.onClick.AddListener(() => UpgradeCharacteristic(UpgradeModule.UpgradeType.Handling));
            
        if (_nitroUpgradeButton != null)
            _nitroUpgradeButton.onClick.AddListener(() => UpgradeCharacteristic(UpgradeModule.UpgradeType.Nitro));
            
        // Кнопка сброса
        if (_resetUpgradesButton != null)
            _resetUpgradesButton.onClick.AddListener(ResetAllUpgrades);
            
        // Настраиваем прогресс-бары
        SetupProgressBar(_engineProgressBar);
        SetupProgressBar(_brakeProgressBar);
        SetupProgressBar(_handlingProgressBar);
        SetupProgressBar(_nitroProgressBar);
    }
    
    private void SetupProgressBar(Slider progressBar)
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 5f;
            progressBar.wholeNumbers = true;
        }
    }
    
    private void UpgradeCharacteristic(UpgradeModule.UpgradeType upgradeType)
    {
        if (_upgradeModule == null) return;
        
        bool success = _upgradeModule.UpgradeCar(_targetCarModelName, upgradeType);
        
        if (success)
        {
            Debug.Log($"Successfully upgraded {upgradeType} for {_targetCarModelName}");
        }
        else
        {
            Debug.Log($"Failed to upgrade {upgradeType} for {_targetCarModelName}");
        }
        
        // UI обновится автоматически через события
    }
    
    private void ResetAllUpgrades()
    {
        if (_upgradeModule == null) return;
        
        _upgradeModule.ResetCarUpgrades(_targetCarModelName);
        UpdateUI();
        
        Debug.Log($"Reset all upgrades for {_targetCarModelName}");
    }
    
    private void UpdateUI()
    {
        if (_upgradeModule == null || _dataModule == null) return;
        
        var carUpgrades = _upgradeModule.GetCarUpgrades(_targetCarModelName);
        
        // Обновляем уровни
        UpdateUpgradeLevel(UpgradeModule.UpgradeType.Engine, _engineLevelText, _engineProgressBar);
        UpdateUpgradeLevel(UpgradeModule.UpgradeType.Brake, _brakeLevelText, _brakeProgressBar);
        UpdateUpgradeLevel(UpgradeModule.UpgradeType.Handling, _handlingLevelText, _handlingProgressBar);
        UpdateUpgradeLevel(UpgradeModule.UpgradeType.Nitro, _nitroLevelText, _nitroProgressBar);
        
        // Обновляем стоимости
        UpdateUpgradeCost(UpgradeModule.UpgradeType.Engine, _engineCostText, _engineUpgradeButton);
        UpdateUpgradeCost(UpgradeModule.UpgradeType.Brake, _brakeCostText, _brakeUpgradeButton);
        UpdateUpgradeCost(UpgradeModule.UpgradeType.Handling, _handlingCostText, _handlingUpgradeButton);
        UpdateUpgradeCost(UpgradeModule.UpgradeType.Nitro, _nitroCostText, _nitroUpgradeButton);
        
        // Обновляем статистики
        UpdateStats(UpgradeModule.UpgradeType.Engine, _engineStatsText);
        UpdateStats(UpgradeModule.UpgradeType.Brake, _brakeStatsText);
        UpdateStats(UpgradeModule.UpgradeType.Handling, _handlingStatsText);
        UpdateStats(UpgradeModule.UpgradeType.Nitro, _nitroStatsText);
        
        // Обновляем количество монет
        if (_totalCoinsText != null)
        {
            _totalCoinsText.text = $"Coins: {_dataModule.Data.coins}";
        }
    }
    
    private void UpdateUpgradeLevel(UpgradeModule.UpgradeType upgradeType, TextMeshProUGUI levelText, Slider progressBar)
    {
        var carUpgrades = _upgradeModule.GetCarUpgrades(_targetCarModelName);
        int level = carUpgrades.GetUpgradeLevel(upgradeType);
        
        if (levelText != null)
        {
            levelText.text = $"Level: {level}/{carUpgrades.maxLevel}";
        }
        
        if (progressBar != null)
        {
            progressBar.value = level;
        }
    }
    
    private void UpdateUpgradeCost(UpgradeModule.UpgradeType upgradeType, TextMeshProUGUI costText, Button upgradeButton)
    {
        var carUpgrades = _upgradeModule.GetCarUpgrades(_targetCarModelName);
        int currentLevel = carUpgrades.GetUpgradeLevel(upgradeType);
        bool canUpgrade = _upgradeModule.CanUpgrade(_targetCarModelName, upgradeType);
        bool isMaxLevel = carUpgrades.IsMaxLevel(upgradeType);
        
        if (costText != null)
        {
            if (isMaxLevel)
            {
                costText.text = "MAX";
                costText.color = Color.green;
            }
            else
            {
                int cost = _upgradeModule.GetUpgradeCost(upgradeType, currentLevel);
                costText.text = $"Cost: {cost}";
                costText.color = canUpgrade ? Color.white : Color.red;
            }
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.interactable = canUpgrade && !isMaxLevel;
        }
    }
    
    private void UpdateStats(UpgradeModule.UpgradeType upgradeType, TextMeshProUGUI statsText)
    {
        if (statsText == null) return;
        
        var carUpgrades = _upgradeModule.GetCarUpgrades(_targetCarModelName);
        float currentValue = carUpgrades.GetCurrentValue(upgradeType);
        
        string statName = "";
        string unit = "";
        
        switch (upgradeType)
        {
            case UpgradeModule.UpgradeType.Engine:
                statName = "Engine Torque";
                unit = " Nm";
                break;
            case UpgradeModule.UpgradeType.Brake:
                statName = "Brake Force";
                unit = " Nm";
                break;
            case UpgradeModule.UpgradeType.Handling:
                statName = "Traction";
                unit = "";
                currentValue = Mathf.Round(currentValue * 100f) / 100f; // Округляем до 2 знаков
                break;
            case UpgradeModule.UpgradeType.Nitro:
                statName = "Nitro Power";
                unit = "%";
                currentValue = Mathf.Round(currentValue * 100f); // Конвертируем в проценты
                break;
        }
        
        statsText.text = $"{statName}: {currentValue:F0}{unit}";
    }
    
    private void OnCarUpgraded(string carModelName, UpgradeModule.UpgradeType upgradeType, int newLevel)
    {
        if (carModelName == _targetCarModelName)
        {
            UpdateUI();
        }
    }
    
    private void OnUpgradeAvailabilityChanged(string carModelName, UpgradeModule.UpgradeType upgradeType, bool canUpgrade)
    {
        if (carModelName == _targetCarModelName)
        {
            UpdateUI();
        }
    }
    
    /// <summary>
    /// Устанавливает целевую модель машины для прокачки
    /// </summary>
    public void SetTargetCar(string carModelName)
    {
        _targetCarModelName = carModelName;
        UpdateUI();
    }
    
    /// <summary>
    /// Применяет сохраненные прокачки к заспавненной машине
    /// </summary>
    public void ApplyUpgradesToSpawnedCar(GameObject carObject)
    {
        if (_upgradeModule == null) return;
        
        _upgradeModule.ApplyUpgradesToCar(carObject, _targetCarModelName);
    }
}
