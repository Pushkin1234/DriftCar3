using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Модуль кастомизации спойлеров машин.
/// Отвечает ТОЛЬКО за спойлеры - установку, разблокировку, покупку.
/// </summary>
public class SpoilerCustomizationModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "SpoilerCustomization";
    
    #region Data Structures
    
    [System.Serializable]
    public class SpoilerData
    {
        public string spoilerName = "";
        public GameObject spoilerPrefab;
        public Sprite spoilerIcon;
        public int price = 0;
        public float downforceBonus = 0f; // Бонус к прижимной силе
        public bool isUnlocked = false;
    }
    
    [System.Serializable]
    public class CarSpoilerData
    {
        public int selectedSpoilerIndex = -1; // -1 = без спойлера
        public bool[] unlockedSpoilers = new bool[10]; // Разблокированные спойлеры
        
        public CarSpoilerData()
        {
            // "Без спойлера" (индекс 0) разблокирован по умолчанию
            unlockedSpoilers[0] = true;
        }
    }
    
    #endregion
    
    #region Spoiler Configuration
    
    [Header("Spoiler Configuration")]
    [Tooltip("Список доступных спойлеров")]
    public SpoilerData[] availableSpoilers = {
        new SpoilerData { spoilerName = "Без спойлера", price = 0, downforceBonus = 0f, isUnlocked = true },
        new SpoilerData { spoilerName = "Спортивный", price = 1500, downforceBonus = 0.1f, isUnlocked = false },
        new SpoilerData { spoilerName = "GT", price = 3000, downforceBonus = 0.2f, isUnlocked = false },
        new SpoilerData { spoilerName = "Racing", price = 5000, downforceBonus = 0.3f, isUnlocked = false },
        new SpoilerData { spoilerName = "Carbon", price = 8000, downforceBonus = 0.4f, isUnlocked = false },
        new SpoilerData { spoilerName = "Wing", price = 12000, downforceBonus = 0.5f, isUnlocked = false }
    };
    
    #endregion
    
    #region Private Fields
    
    private DataModule _dataModule;
    private Dictionary<int, CarSpoilerData> _carSpoilerData = new Dictionary<int, CarSpoilerData>(); // По индексу машины (0-4)
    
    #endregion
    
    #region Events
    
    /// <summary>
    /// Событие: спойлер изменён (carIndex, spoilerIndex)
    /// </summary>
    public System.Action<int, int> OnSpoilerChanged;
    
    /// <summary>
    /// Событие: спойлер куплен (carIndex, spoilerIndex)
    /// </summary>
    public System.Action<int, int> OnSpoilerPurchased;
    
    #endregion
    
    #region Initialization
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadAllCarSpoilerData();
        base.Initialize();
        
        Debug.Log($"[{ModuleName}] Инициализирован. Доступно {availableSpoilers.Length} спойлеров.");
    }
    
    private void LoadAllCarSpoilerData()
    {
        for (int i = 0; i < 5; i++)
        {
            LoadCarSpoilerData(i);
        }
    }
    
    private void LoadCarSpoilerData(int carIndex)
    {
        string key = $"CarSpoiler_{carIndex}";
        
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            _carSpoilerData[carIndex] = JsonUtility.FromJson<CarSpoilerData>(json);
        }
        else
        {
            _carSpoilerData[carIndex] = new CarSpoilerData();
        }
    }
    
    #endregion
    
    #region Spoiler Management
    
    /// <summary>
    /// Получить количество доступных спойлеров
    /// </summary>
    public int GetSpoilerCount()
    {
        return availableSpoilers.Length;
    }
    
    /// <summary>
    /// Получить данные о спойлере по индексу
    /// </summary>
    public SpoilerData GetSpoilerData(int spoilerIndex)
    {
        if (spoilerIndex < 0 || spoilerIndex >= availableSpoilers.Length)
            return null;
        
        return availableSpoilers[spoilerIndex];
    }
    
    /// <summary>
    /// Проверить, разблокирован ли спойлер для машины
    /// </summary>
    public bool IsSpoilerUnlocked(int carIndex, int spoilerIndex)
    {
        if (!_carSpoilerData.ContainsKey(carIndex))
            return false;
        
        if (spoilerIndex < 0 || spoilerIndex >= _carSpoilerData[carIndex].unlockedSpoilers.Length)
            return false;
        
        return _carSpoilerData[carIndex].unlockedSpoilers[spoilerIndex];
    }
    
    /// <summary>
    /// Получить индекс текущего спойлера на машине
    /// </summary>
    public int GetCurrentSpoilerIndex(int carIndex)
    {
        if (!_carSpoilerData.ContainsKey(carIndex))
            return -1; // Нет спойлера
        
        return _carSpoilerData[carIndex].selectedSpoilerIndex;
    }
    
    /// <summary>
    /// Получить бонус прижимной силы от текущего спойлера
    /// </summary>
    public float GetCurrentDownforceBonus(int carIndex)
    {
        int spoilerIndex = GetCurrentSpoilerIndex(carIndex);
        if (spoilerIndex < 0)
            return 0f;
        
        var spoilerData = GetSpoilerData(spoilerIndex);
        return spoilerData != null ? spoilerData.downforceBonus : 0f;
    }
    
    #endregion
    
    #region Spoiler Operations
    
    /// <summary>
    /// Установить спойлер на машину (-1 = убрать спойлер)
    /// </summary>
    public void ChangeSpoiler(int carIndex, int spoilerIndex)
    {
        // -1 = убрать спойлер (всегда разрешено)
        if (spoilerIndex != -1)
        {
            if (spoilerIndex < 0 || spoilerIndex >= availableSpoilers.Length)
            {
                Debug.LogWarning($"[{ModuleName}] Неверный индекс спойлера: {spoilerIndex}");
                return;
            }
            
            if (!IsSpoilerUnlocked(carIndex, spoilerIndex))
            {
                Debug.LogWarning($"[{ModuleName}] Спойлер {spoilerIndex} ещё не разблокирован для машины {carIndex}!");
                return;
            }
        }
        
        if (!_carSpoilerData.ContainsKey(carIndex))
        {
            _carSpoilerData[carIndex] = new CarSpoilerData();
        }
        
        _carSpoilerData[carIndex].selectedSpoilerIndex = spoilerIndex;
        
        // Сохраняем
        SaveCarSpoilerData(carIndex);
        
        // Уведомляем
        OnSpoilerChanged?.Invoke(carIndex, spoilerIndex);
        
        string spoilerName = spoilerIndex >= 0 ? availableSpoilers[spoilerIndex].spoilerName : "Убран";
        Debug.Log($"[{ModuleName}] Спойлер машины {carIndex} изменён на: {spoilerName}");
    }
    
    #endregion
    
    #region Purchase System
    
    /// <summary>
    /// Купить спойлер для машины
    /// </summary>
    public bool PurchaseSpoiler(int carIndex, int spoilerIndex)
    {
        var spoilerData = GetSpoilerData(spoilerIndex);
        if (spoilerData == null)
        {
            Debug.LogWarning($"[{ModuleName}] Спойлер с индексом {spoilerIndex} не найден!");
            return false;
        }
        
        // Проверяем, не куплен ли уже
        if (IsSpoilerUnlocked(carIndex, spoilerIndex))
        {
            Debug.LogWarning($"[{ModuleName}] Спойлер {spoilerData.spoilerName} уже разблокирован для машины {carIndex}!");
            return false;
        }
        
        // Проверяем монеты
        if (_dataModule.Data.coins < spoilerData.price)
        {
            Debug.LogWarning($"[{ModuleName}] Недостаточно монет! Нужно: {spoilerData.price}, есть: {_dataModule.Data.coins}");
            return false;
        }
        
        // Списываем монеты
        _dataModule.Data.coins -= spoilerData.price;
        _dataModule.SaveData();
        
        // Разблокируем спойлер
        if (!_carSpoilerData.ContainsKey(carIndex))
        {
            _carSpoilerData[carIndex] = new CarSpoilerData();
        }
        
        _carSpoilerData[carIndex].unlockedSpoilers[spoilerIndex] = true;
        
        // Сохраняем
        SaveCarSpoilerData(carIndex);
        
        // Уведомляем
        OnSpoilerPurchased?.Invoke(carIndex, spoilerIndex);
        
        Debug.Log($"[{ModuleName}] Куплен спойлер {spoilerData.spoilerName} для машины {carIndex} за {spoilerData.price} монет");
        
        return true;
    }
    
    /// <summary>
    /// Разблокировать спойлер без покупки (для наград, достижений)
    /// </summary>
    public void UnlockSpoiler(int carIndex, int spoilerIndex)
    {
        if (!_carSpoilerData.ContainsKey(carIndex))
        {
            _carSpoilerData[carIndex] = new CarSpoilerData();
        }
        
        _carSpoilerData[carIndex].unlockedSpoilers[spoilerIndex] = true;
        SaveCarSpoilerData(carIndex);
        
        Debug.Log($"[{ModuleName}] Разблокирован спойлер {spoilerIndex} для машины {carIndex}");
    }
    
    #endregion
    
    #region Save/Load
    
    private void SaveCarSpoilerData(int carIndex)
    {
        if (!_carSpoilerData.ContainsKey(carIndex))
            return;
        
        string key = $"CarSpoiler_{carIndex}";
        string json = JsonUtility.ToJson(_carSpoilerData[carIndex]);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Сохранить данные всех машин
    /// </summary>
    public void SaveAllCarSpoilerData()
    {
        foreach (var kvp in _carSpoilerData)
        {
            SaveCarSpoilerData(kvp.Key);
        }
        
        Debug.Log($"[{ModuleName}] Сохранены данные спойлеров для {_carSpoilerData.Count} машин");
    }
    
    /// <summary>
    /// Сбросить данные спойлеров машины
    /// </summary>
    public void ResetCarSpoilerData(int carIndex)
    {
        _carSpoilerData[carIndex] = new CarSpoilerData();
        SaveCarSpoilerData(carIndex);
        
        Debug.Log($"[{ModuleName}] Сброшены данные спойлеров для машины {carIndex}");
    }
    
    #endregion
    
    #region Shutdown
    
    public override void Shutdown()
    {
        SaveAllCarSpoilerData();
        base.Shutdown();
    }
    
    #endregion
}

