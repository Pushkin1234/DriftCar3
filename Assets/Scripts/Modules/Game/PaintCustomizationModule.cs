using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Модуль кастомизации покраски машин.
/// Отвечает ТОЛЬКО за покраску - цвета, разблокировку, покупку.
/// </summary>
public class PaintCustomizationModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "PaintCustomization";
    
    #region Data Structures
    
    [System.Serializable]
    public class ColorData
    {
        public Color color = Color.white;
        public string colorName = "";
        public int price = 0;
        public bool isUnlocked = false;
    }
    
    [System.Serializable]
    public class CarPaintData
    {
        public Color currentColor = Color.white;
        public bool[] unlockedColors = new bool[8]; // Разблокированные цвета для этой машины
        
        public CarPaintData()
        {
            // Белый цвет разблокирован по умолчанию
            unlockedColors[0] = true;
        }
    }
    
    #endregion
    
    #region Available Colors Configuration
    
    public ColorData[] availableColors = {
        new ColorData { color = Color.white, colorName = "Белый", price = 0, isUnlocked = true },
        new ColorData { color = Color.black, colorName = "Черный", price = 500, isUnlocked = false },
        new ColorData { color = Color.red, colorName = "Красный", price = 300, isUnlocked = false },
        new ColorData { color = Color.blue, colorName = "Синий", price = 400, isUnlocked = false },
        new ColorData { color = Color.green, colorName = "Зеленый", price = 350, isUnlocked = false },
        new ColorData { color = Color.yellow, colorName = "Желтый", price = 600, isUnlocked = false },
        new ColorData { color = Color.cyan, colorName = "Голубой", price = 450, isUnlocked = false },
        new ColorData { color = Color.magenta, colorName = "Фиолетовый", price = 700, isUnlocked = false }
    };
    
    #endregion
    
    #region Private Fields
    
    private DataModule _dataModule;
    private Dictionary<int, CarPaintData> _carPaintData = new Dictionary<int, CarPaintData>(); // По индексу машины (0-4)
    
    #endregion
    
    #region Events
    
    /// <summary>
    /// Событие: машина покрашена (carIndex, color)
    /// </summary>
    public System.Action<int, Color> OnCarPainted;
    
    /// <summary>
    /// Событие: цвет выбран для предпросмотра (carIndex, colorIndex, color)
    /// </summary>
    public System.Action<int, int, Color> OnColorSelected;
    
    /// <summary>
    /// Событие: цвет куплен (carIndex, colorIndex)
    /// </summary>
    public System.Action<int, int> OnColorPurchased;
    
    #endregion
    
    #region Initialization
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadAllCarPaintData();
        base.Initialize();
        
        Debug.Log($"[{ModuleName}] Инициализирован. Загружено данных для {_carPaintData.Count} машин.");
    }
    
    private void LoadAllCarPaintData()
    {
        // Загружаем данные покраски для всех 5 машин
        for (int i = 0; i < 5; i++)
        {
            LoadCarPaintData(i);
        }
    }
    
    private void LoadCarPaintData(int carIndex)
    {
        string key = $"CarPaint_{carIndex}";
        
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            _carPaintData[carIndex] = JsonUtility.FromJson<CarPaintData>(json);
        }
        else
        {
            // Создаём данные по умолчанию
            _carPaintData[carIndex] = new CarPaintData();
        }
    }
    
    #endregion
    
    #region Color Management
    
    /// <summary>
    /// Получить количество доступных цветов
    /// </summary>
    public int GetColorCount()
    {
        return availableColors.Length;
    }
    
    /// <summary>
    /// Получить данные о цвете по индексу
    /// </summary>
    public ColorData GetColorData(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= availableColors.Length)
            return null;
        
        return availableColors[colorIndex];
    }
    
    /// <summary>
    /// Проверить, разблокирован ли цвет для машины
    /// </summary>
    public bool IsColorUnlocked(int carIndex, int colorIndex)
    {
        if (!_carPaintData.ContainsKey(carIndex))
            return false;
        
        if (colorIndex < 0 || colorIndex >= _carPaintData[carIndex].unlockedColors.Length)
            return false;
        
        return _carPaintData[carIndex].unlockedColors[colorIndex];
    }
    
    /// <summary>
    /// Получить текущий цвет машины
    /// </summary>
    public Color GetCurrentColor(int carIndex)
    {
        if (!_carPaintData.ContainsKey(carIndex))
            return Color.white;
        
        return _carPaintData[carIndex].currentColor;
    }
    
    #endregion
    
    #region Paint Operations
    
    /// <summary>
    /// Покрасить машину (применить цвет)
    /// </summary>
    public void PaintCar(int carIndex, Color color)
    {
        if (!_carPaintData.ContainsKey(carIndex))
        {
            _carPaintData[carIndex] = new CarPaintData();
        }
        
        _carPaintData[carIndex].currentColor = color;
        
        // Сохраняем
        SaveCarPaintData(carIndex);
        
        // Уведомляем
        OnCarPainted?.Invoke(carIndex, color);
        
        Debug.Log($"[{ModuleName}] Машина {carIndex} покрашена в цвет {color}");
    }
    
    /// <summary>
    /// Выбрать цвет для предварительного просмотра
    /// </summary>
    public void SelectColor(int carIndex, int colorIndex)
    {
        var colorData = GetColorData(colorIndex);
        if (colorData == null)
            return;
        
        // Уведомляем (не применяем сразу, только показываем)
        OnColorSelected?.Invoke(carIndex, colorIndex, colorData.color);
        
        Debug.Log($"[{ModuleName}] Выбран цвет {colorData.colorName} для предпросмотра на машине {carIndex}");
    }
    
    #endregion
    
    #region Purchase System
    
    /// <summary>
    /// Купить цвет для машины
    /// </summary>
    public bool PurchaseColor(int carIndex, int colorIndex)
    {
        var colorData = GetColorData(colorIndex);
        if (colorData == null)
        {
            Debug.LogWarning($"[{ModuleName}] Цвет с индексом {colorIndex} не найден!");
            return false;
        }
        
        // Проверяем, не куплен ли уже
        if (IsColorUnlocked(carIndex, colorIndex))
        {
            Debug.LogWarning($"[{ModuleName}] Цвет {colorData.colorName} уже разблокирован для машины {carIndex}!");
            return false;
        }
        
        // Проверяем монеты
        if (_dataModule.Data.coins < colorData.price)
        {
            Debug.LogWarning($"[{ModuleName}] Недостаточно монет! Нужно: {colorData.price}, есть: {_dataModule.Data.coins}");
            return false;
        }
        
        // Списываем монеты
        _dataModule.Data.coins -= colorData.price;
        _dataModule.SaveData();
        
        // Разблокируем цвет
        if (!_carPaintData.ContainsKey(carIndex))
        {
            _carPaintData[carIndex] = new CarPaintData();
        }
        
        _carPaintData[carIndex].unlockedColors[colorIndex] = true;
        
        // Сохраняем
        SaveCarPaintData(carIndex);
        
        // Уведомляем
        OnColorPurchased?.Invoke(carIndex, colorIndex);
        
        Debug.Log($"[{ModuleName}] Куплен цвет {colorData.colorName} для машины {carIndex} за {colorData.price} монет");
        
        return true;
    }
    
    /// <summary>
    /// Разблокировать цвет без покупки (для наград, достижений и т.д.)
    /// </summary>
    public void UnlockColor(int carIndex, int colorIndex)
    {
        if (!_carPaintData.ContainsKey(carIndex))
        {
            _carPaintData[carIndex] = new CarPaintData();
        }
        
        _carPaintData[carIndex].unlockedColors[colorIndex] = true;
        SaveCarPaintData(carIndex);
        
        Debug.Log($"[{ModuleName}] Разблокирован цвет {colorIndex} для машины {carIndex}");
    }
    
    #endregion
    
    #region Save/Load
    
    private void SaveCarPaintData(int carIndex)
    {
        if (!_carPaintData.ContainsKey(carIndex))
            return;
        
        string key = $"CarPaint_{carIndex}";
        string json = JsonUtility.ToJson(_carPaintData[carIndex]);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Сохранить данные всех машин
    /// </summary>
    public void SaveAllCarPaintData()
    {
        foreach (var kvp in _carPaintData)
        {
            SaveCarPaintData(kvp.Key);
        }
        
        Debug.Log($"[{ModuleName}] Сохранены данные покраски для {_carPaintData.Count} машин");
    }
    
    /// <summary>
    /// Сбросить данные покраски машины
    /// </summary>
    public void ResetCarPaintData(int carIndex)
    {
        _carPaintData[carIndex] = new CarPaintData();
        SaveCarPaintData(carIndex);
        
        Debug.Log($"[{ModuleName}] Сброшены данные покраски для машины {carIndex}");
    }
    
    #endregion
    
    #region Shutdown
    
    public override void Shutdown()
    {
        SaveAllCarPaintData();
        base.Shutdown();
    }
    
    #endregion
}

