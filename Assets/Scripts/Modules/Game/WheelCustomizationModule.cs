using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Модуль кастомизации колёс машин.
/// Отвечает ТОЛЬКО за колёса - смену, разблокировку, покупку.
/// </summary>
public class WheelCustomizationModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "WheelCustomization";
    
    #region Data Structures
    
    [System.Serializable]
    public class WheelData
    {
        public string wheelName = "";
        public GameObject wheelPrefab;
        public Sprite wheelIcon;
        public int price = 0;
        public bool isUnlocked = false;
    }
    
    [System.Serializable]
    public class CarWheelData
    {
        public int selectedWheelIndex = 0; // Текущие колёса
        public bool[] unlockedWheels = new bool[10]; // Разблокированные колёса (общие для всех машин)
        
        public CarWheelData()
        {
            // Первые колёса разблокированы по умолчанию
            unlockedWheels[0] = true;
        }
    }
    
    #endregion
    
    #region Available Wheels Configuration
    
    [Header("Wheel Configuration")]
    [Tooltip("Список доступных колёс для кастомизации")]
    public WheelData[] availableWheels = new WheelData[0];
    
    #endregion
    
    #region Private Fields
    
    private DataModule _dataModule;
    private Dictionary<int, CarWheelData> _carWheelData = new Dictionary<int, CarWheelData>(); // По индексу машины (0-4)
    
    #endregion
    
    #region Events
    
    /// <summary>
    /// Событие: колёса изменены (carIndex, wheelIndex)
    /// </summary>
    public System.Action<int, int> OnWheelsChanged;
    
    /// <summary>
    /// Событие: колёса куплены (wheelIndex)
    /// </summary>
    public System.Action<int> OnWheelsPurchased;
    
    #endregion
    
    #region Initialization
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadAllCarWheelData();
        base.Initialize();
        
        Debug.Log($"[{ModuleName}] Инициализирован. Доступно {availableWheels.Length} типов колёс.");
    }
    
    private void LoadAllCarWheelData()
    {
        // Загружаем данные колёс для всех 5 машин
        for (int i = 0; i < 5; i++)
        {
            LoadCarWheelData(i);
        }
    }
    
    private void LoadCarWheelData(int carIndex)
    {
        string key = $"CarWheels_{carIndex}";
        
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            _carWheelData[carIndex] = JsonUtility.FromJson<CarWheelData>(json);
        }
        else
        {
            // Создаём данные по умолчанию
            _carWheelData[carIndex] = new CarWheelData();
        }
    }
    
    #endregion
    
    #region Wheel Management
    
    /// <summary>
    /// Получить количество доступных колёс
    /// </summary>
    public int GetWheelCount()
    {
        return availableWheels.Length;
    }
    
    /// <summary>
    /// Получить данные о колёсах по индексу
    /// </summary>
    public WheelData GetWheelData(int wheelIndex)
    {
        if (wheelIndex < 0 || wheelIndex >= availableWheels.Length)
            return null;
        
        return availableWheels[wheelIndex];
    }
    
    /// <summary>
    /// Проверить, разблокированы ли колёса (общие для всех машин)
    /// </summary>
    public bool IsWheelUnlocked(int wheelIndex)
    {
        // Колёса общие для всех машин, проверяем через первую машину
        if (!_carWheelData.ContainsKey(0))
            return false;
        
        if (wheelIndex < 0 || wheelIndex >= _carWheelData[0].unlockedWheels.Length)
            return false;
        
        return _carWheelData[0].unlockedWheels[wheelIndex];
    }
    
    /// <summary>
    /// Получить индекс текущих колёс на машине
    /// </summary>
    public int GetCurrentWheelIndex(int carIndex)
    {
        if (!_carWheelData.ContainsKey(carIndex))
            return 0;
        
        return _carWheelData[carIndex].selectedWheelIndex;
    }
    
    #endregion
    
    #region Wheel Operations
    
    /// <summary>
    /// Сменить колёса на машине
    /// </summary>
    public void ChangeWheels(int carIndex, int wheelIndex)
    {
        if (wheelIndex < 0 || wheelIndex >= availableWheels.Length)
        {
            Debug.LogWarning($"[{ModuleName}] Неверный индекс колёс: {wheelIndex}");
            return;
        }
        
        if (!IsWheelUnlocked(wheelIndex))
        {
            Debug.LogWarning($"[{ModuleName}] Колёса {wheelIndex} ещё не разблокированы!");
            return;
        }
        
        if (!_carWheelData.ContainsKey(carIndex))
        {
            _carWheelData[carIndex] = new CarWheelData();
        }
        
        _carWheelData[carIndex].selectedWheelIndex = wheelIndex;
        
        // Сохраняем
        SaveCarWheelData(carIndex);
        
        // Уведомляем
        OnWheelsChanged?.Invoke(carIndex, wheelIndex);
        
        var wheelData = availableWheels[wheelIndex];
        Debug.Log($"[{ModuleName}] Колёса машины {carIndex} изменены на: {wheelData.wheelName}");
    }
    
    #endregion
    
    #region Purchase System
    
    /// <summary>
    /// Купить колёса (разблокируются для всех машин)
    /// </summary>
    public bool PurchaseWheel(int wheelIndex)
    {
        var wheelData = GetWheelData(wheelIndex);
        if (wheelData == null)
        {
            Debug.LogWarning($"[{ModuleName}] Колёса с индексом {wheelIndex} не найдены!");
            return false;
        }
        
        // Проверяем, не куплены ли уже
        if (IsWheelUnlocked(wheelIndex))
        {
            Debug.LogWarning($"[{ModuleName}] Колёса {wheelData.wheelName} уже разблокированы!");
            return false;
        }
        
        // Проверяем монеты
        if (_dataModule.Data.coins < wheelData.price)
        {
            Debug.LogWarning($"[{ModuleName}] Недостаточно монет! Нужно: {wheelData.price}, есть: {_dataModule.Data.coins}");
            return false;
        }
        
        // Списываем монеты
        _dataModule.Data.coins -= wheelData.price;
        _dataModule.SaveData();
        
        // Разблокируем колёса (для всех машин)
        for (int i = 0; i < 5; i++)
        {
            if (!_carWheelData.ContainsKey(i))
            {
                _carWheelData[i] = new CarWheelData();
            }
            
            _carWheelData[i].unlockedWheels[wheelIndex] = true;
            SaveCarWheelData(i);
        }
        
        // Уведомляем
        OnWheelsPurchased?.Invoke(wheelIndex);
        
        Debug.Log($"[{ModuleName}] Куплены колёса {wheelData.wheelName} за {wheelData.price} монет");
        
        return true;
    }
    
    /// <summary>
    /// Разблокировать колёса без покупки (для наград, достижений)
    /// </summary>
    public void UnlockWheel(int wheelIndex)
    {
        for (int i = 0; i < 5; i++)
        {
            if (!_carWheelData.ContainsKey(i))
            {
                _carWheelData[i] = new CarWheelData();
            }
            
            _carWheelData[i].unlockedWheels[wheelIndex] = true;
            SaveCarWheelData(i);
        }
        
        Debug.Log($"[{ModuleName}] Разблокированы колёса {wheelIndex}");
    }
    
    #endregion
    
    #region Save/Load
    
    private void SaveCarWheelData(int carIndex)
    {
        if (!_carWheelData.ContainsKey(carIndex))
            return;
        
        string key = $"CarWheels_{carIndex}";
        string json = JsonUtility.ToJson(_carWheelData[carIndex]);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Сохранить данные всех машин
    /// </summary>
    public void SaveAllCarWheelData()
    {
        foreach (var kvp in _carWheelData)
        {
            SaveCarWheelData(kvp.Key);
        }
        
        Debug.Log($"[{ModuleName}] Сохранены данные колёс для {_carWheelData.Count} машин");
    }
    
    /// <summary>
    /// Сбросить данные колёс машины
    /// </summary>
    public void ResetCarWheelData(int carIndex)
    {
        _carWheelData[carIndex] = new CarWheelData();
        SaveCarWheelData(carIndex);
        
        Debug.Log($"[{ModuleName}] Сброшены данные колёс для машины {carIndex}");
    }
    
    #endregion
    
    #region Shutdown
    
    public override void Shutdown()
    {
        SaveAllCarWheelData();
        base.Shutdown();
    }
    
    #endregion
}

