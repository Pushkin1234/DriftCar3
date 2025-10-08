using System.Collections.Generic;
using UnityEngine;

public class CustomizationModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "Customization";
    
    [System.Serializable]
    public class CarCustomization
    {
        public Color paintColor = Color.white;
        public int selectedWheelIndex = 0;
        public int selectedSpoilerIndex = -1;
        public string carModelName = "";
        public bool[] unlockedColors = new bool[8]; // Массив разблокированных цветов
        
        // Улучшения характеристик
        public int engineLevel = 0;
        public int brakeLevel = 0;
        public int nitroLevel = 0;
        public int handlingLevel = 0;
        
        // Разблокированные улучшения
        public bool[] unlockedEngineLevels = new bool[5]; // 5 уровней двигателя
        public bool[] unlockedBrakeLevels = new bool[5]; // 5 уровней тормозов
        public bool[] unlockedNitroLevels = new bool[5]; // 5 уровней нитро
        public bool[] unlockedHandlingLevels = new bool[5]; // 5 уровней управления
        public bool[] unlockedSpoilers = new bool[10]; // 10 спойлеров
    }
    
    [System.Serializable]
    public class WheelData
    {
        public string wheelName = "";
        public GameObject wheelPrefab;
        public Sprite wheelIcon;
        public int price = 0;
        public bool isUnlocked = false;
    }
    
    private DataModule _dataModule;
    private Dictionary<string, CarCustomization> _carCustomizations = new Dictionary<string, CarCustomization>();
    private Dictionary<int, CarCustomization> _carCustomizationsByIndex = new Dictionary<int, CarCustomization>(); // Кастомизация по индексу машины
    
    [System.Serializable]
    public class ColorData
    {
        public Color color = Color.white;
        public string colorName = "";
        public int price = 0;
        public bool isUnlocked = false;
    }
    
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
    public class SpoilerData
    {
        public string spoilerName = "";
        public GameObject spoilerPrefab;
        public Sprite spoilerIcon;
        public int price = 0;
        public float downforceBonus = 0f; // Бонус к прижимной силе
        public bool isUnlocked = false;
    }
    
    // Доступные цвета для кастомизации
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
    
    // Доступные колеса для кастомизации
    public WheelData[] availableWheels = new WheelData[0];
    
    // Доступные улучшения двигателя
    public UpgradeData[] engineUpgrades = {
        new UpgradeData { upgradeName = "Стандартный", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовый двигатель", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенный", level = 1, price = 1000, powerMultiplier = 1.2f, description = "+20% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивный", level = 2, price = 2500, powerMultiplier = 1.5f, description = "+50% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Турбо", level = 3, price = 5000, powerMultiplier = 1.8f, description = "+80% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Максимальный", level = 4, price = 10000, powerMultiplier = 2.2f, description = "+120% мощности", isUnlocked = false }
    };
    
    // Доступные улучшения тормозов
    public UpgradeData[] brakeUpgrades = {
        new UpgradeData { upgradeName = "Стандартные", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовые тормоза", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенные", level = 1, price = 800, powerMultiplier = 1.3f, description = "+30% эффективности", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивные", level = 2, price = 2000, powerMultiplier = 1.6f, description = "+60% эффективности", isUnlocked = false },
        new UpgradeData { upgradeName = "Керамические", level = 3, price = 4000, powerMultiplier = 2.0f, description = "+100% эффективности", isUnlocked = false },
        new UpgradeData { upgradeName = "Карбоновые", level = 4, price = 8000, powerMultiplier = 2.5f, description = "+150% эффективности", isUnlocked = false }
    };
    
    // Доступные улучшения нитро
    public UpgradeData[] nitroUpgrades = {
        new UpgradeData { upgradeName = "Стандартный", level = 0, price = 0, powerMultiplier = 1.0f, description = "Базовый нитро", isUnlocked = true },
        new UpgradeData { upgradeName = "Улучшенный", level = 1, price = 1200, powerMultiplier = 1.4f, description = "+40% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Спортивный", level = 2, price = 3000, powerMultiplier = 1.8f, description = "+80% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Турбо", level = 3, price = 6000, powerMultiplier = 2.3f, description = "+130% мощности", isUnlocked = false },
        new UpgradeData { upgradeName = "Максимальный", level = 4, price = 12000, powerMultiplier = 2.8f, description = "+180% мощности", isUnlocked = false }
    };
    
    // Доступные спойлеры
    public SpoilerData[] availableSpoilers = {
        new SpoilerData { spoilerName = "Без спойлера", price = 0, downforceBonus = 0f, isUnlocked = true },
        new SpoilerData { spoilerName = "Спортивный", price = 1500, downforceBonus = 0.1f, isUnlocked = false },
        new SpoilerData { spoilerName = "GT", price = 3000, downforceBonus = 0.2f, isUnlocked = false },
        new SpoilerData { spoilerName = "Racing", price = 5000, downforceBonus = 0.3f, isUnlocked = false },
        new SpoilerData { spoilerName = "Carbon", price = 8000, downforceBonus = 0.4f, isUnlocked = false },
        new SpoilerData { spoilerName = "Wing", price = 12000, downforceBonus = 0.5f, isUnlocked = false }
    };
    
    // События для уведомления других систем
    public System.Action<string, Color> OnCarPainted;
    public System.Action<string, int> OnWheelsChanged;
    public System.Action<string, int, Color> OnColorSelected; // carModelName, colorIndex, color
    public System.Action<string, int> OnColorPurchased; // carModelName, colorIndex
    public System.Action<string, int> OnColorUnlocked; // carModelName, colorIndex
    
    // События для улучшений
    public System.Action<string, int> OnEngineUpgraded; // carModelName, level
    public System.Action<string, int> OnBrakeUpgraded; // carModelName, level
    public System.Action<string, int> OnNitroUpgraded; // carModelName, level
    public System.Action<string, int> OnHandlingUpgraded; // carModelName, level
    public System.Action<string, int> OnSpoilerChanged; // carModelName, spoilerIndex
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadCustomizations();
        LoadAllCarCustomizations(); // Загружаем кастомизацию для всех 5 машин
        base.Initialize();
    }
    
    /// <summary>
    /// Красит машину определенного типа
    /// </summary>
    public void PaintCar(string carModelName, Color color)
    {
        if (!_carCustomizations.ContainsKey(carModelName))
        {
            _carCustomizations[carModelName] = new CarCustomization();
            _carCustomizations[carModelName].carModelName = carModelName;
        }
        
        _carCustomizations[carModelName].paintColor = color;
        
        // Применяем изменения к активной машине
        ApplyPaintToActiveCar(carModelName, color);
        
        // Уведомляем подписчиков
        OnCarPainted?.Invoke(carModelName, color);
        
        // Сохраняем изменения
        SaveCustomizations();
    }
    
    /// <summary>
    /// Меняет колеса машины
    /// </summary>
    public void ChangeWheels(string carModelName, int wheelIndex)
    {
        // Проверяем валидность индекса
        if (wheelIndex < 0 || wheelIndex >= availableWheels.Length)
        {
            Debug.LogError($"Invalid wheel index: {wheelIndex}. Available wheels: {availableWheels.Length}");
            return;
        }
        
        // Проверяем разблокированность колес
        if (!availableWheels[wheelIndex].isUnlocked)
        {
            Debug.LogWarning($"Wheel {availableWheels[wheelIndex].wheelName} is not unlocked yet!");
            return;
        }
        
        if (!_carCustomizations.ContainsKey(carModelName))
        {
            _carCustomizations[carModelName] = new CarCustomization();
            _carCustomizations[carModelName].carModelName = carModelName;
        }
        
        _carCustomizations[carModelName].selectedWheelIndex = wheelIndex;
        
        // Применяем изменения к активной машине
        ApplyWheelsToActiveCar(carModelName, wheelIndex);
        
        // Уведомляем подписчиков
        OnWheelsChanged?.Invoke(carModelName, wheelIndex);
        
        // Сохраняем изменения
        SaveCustomizations();
    }
    
    /// <summary>
    /// Получает кастомизацию для определенной машины
    /// </summary>
    public CarCustomization GetCarCustomization(string carModelName)
    {
        if (_carCustomizations.ContainsKey(carModelName))
            return _carCustomizations[carModelName];
            
        // Возвращаем стандартную кастомизацию
        var defaultCustomization = new CarCustomization();
        defaultCustomization.carModelName = carModelName;
        return defaultCustomization;
    }
    
    /// <summary>
    /// Применяет кастомизацию к машине при спавне
    /// </summary>
    public void ApplyCustomizationToCar(GameObject carObject, string carModelName)
    {
        var customization = GetCarCustomization(carModelName);
        
        // Применяем цвет
        ApplyPaintToCarObject(carObject, customization.paintColor);
        
        // Применяем колеса (если есть система смены колес)
        ApplyWheelsToCarObject(carObject, customization.selectedWheelIndex);
    }
    
    private void ApplyPaintToActiveCar(string carModelName, Color color)
    {
        // Находим активную машину
        var activeVehicle = FindObjectOfType<RCC_CarControllerV3>();
        if (activeVehicle != null && activeVehicle.name.Contains(carModelName))
        {
            ApplyPaintToCarObject(activeVehicle.gameObject, color);
        }
    }
    
    private void ApplyPaintToCarObject(GameObject carObject, Color color)
    {
        // Ищем RCC_CustomizationApplier
        var customizationApplier = carObject.GetComponent<RCC_CustomizationApplier>();
        if (customizationApplier != null && customizationApplier.PaintManager != null)
        {
            customizationApplier.PaintManager.Paint(color);
        }
        else
        {
            // Альтернативный способ - ищем MeshRenderer напрямую
            var renderers = carObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.name.ToLower().Contains("body") || 
                    renderer.name.ToLower().Contains("car") ||
                    renderer.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
                {
                    if (renderer.materials.Length > 0)
                    {
                        renderer.materials[0].color = color;
                    }
                }
            }
        }
    }
    
    private void ApplyWheelsToActiveCar(string carModelName, int wheelIndex)
    {
        var activeVehicle = FindObjectOfType<RCC_CarControllerV3>();
        if (activeVehicle != null && activeVehicle.name.Contains(carModelName))
        {
            ApplyWheelsToCarObject(activeVehicle.gameObject, wheelIndex);
        }
    }
    
    private void ApplyWheelsToCarObject(GameObject carObject, int wheelIndex)
    {
        // Проверяем валидность индекса
        if (wheelIndex < 0 || wheelIndex >= availableWheels.Length)
            return;
            
        var wheelData = availableWheels[wheelIndex];
        if (wheelData.wheelPrefab == null)
        {
            Debug.LogError($"Wheel prefab is null for index {wheelIndex}");
            return;
        }
        
        // Способ 1: Интеграция с RCC системой смены колес
        var customizationApplier = carObject.GetComponent<RCC_CustomizationApplier>();
        if (customizationApplier != null && customizationApplier.WheelManager != null)
        {
            customizationApplier.WheelManager.UpdateWheel(wheelIndex);
            return;
        }
        
        // Способ 2: Прямая работа с RCC_CarControllerV3
        var carController = carObject.GetComponent<RCC_CarControllerV3>();
        if (carController != null)
        {
            ChangeWheelsDirectly(carController, wheelData.wheelPrefab, true);
            return;
        }
        
        // Способ 3: Альтернативный метод для машин без RCC
        ChangeWheelsAlternative(carObject, wheelData.wheelPrefab);
    }
    
    /// <summary>
    /// Прямая смена колес через RCC API
    /// </summary>
    private void ChangeWheelsDirectly(RCC_CarControllerV3 vehicle, GameObject wheelPrefab, bool applyRadius)
    {
        // Используем статический метод RCC_Customization
        // RCC_Customization.ChangeWheels(vehicle, wheelPrefab, applyRadius);
        
        // Альтернативная реализация на случай если RCC_Customization недоступен
        for (int i = 0; i < vehicle.AllWheelColliders.Length; i++)
        {
            var wheelCollider = vehicle.AllWheelColliders[i];
            
            // Отключаем старые модели колес
            if (wheelCollider.wheelModel.GetComponent<MeshRenderer>())
                wheelCollider.wheelModel.GetComponent<MeshRenderer>().enabled = false;
                
            foreach (Transform t in wheelCollider.wheelModel.GetComponentsInChildren<Transform>())
            {
                if (t != wheelCollider.wheelModel)
                    t.gameObject.SetActive(false);
            }
            
            // Создаем новое колесо
            GameObject newWheel = Instantiate(wheelPrefab, 
                wheelCollider.wheelModel.position, 
                wheelCollider.wheelModel.rotation, 
                wheelCollider.wheelModel);
            
            // Зеркалируем правые колеса
            if (wheelCollider.wheelModel.localPosition.x > 0f)
            {
                var scale = newWheel.transform.localScale;
                newWheel.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
            
            // Применяем радиус если нужно
            if (applyRadius)
            {
                var bounds = GetWheelBounds(wheelPrefab);
                wheelCollider.WheelCollider.radius = bounds;
            }
        }
    }
    
    /// <summary>
    /// Альтернативный метод смены колес для машин без RCC
    /// </summary>
    private void ChangeWheelsAlternative(GameObject carObject, GameObject wheelPrefab)
    {
        // Ищем все объекты с именем содержащим "wheel"
        var wheelObjects = new List<Transform>();
        FindWheelObjects(carObject.transform, wheelObjects);
        
        foreach (var wheelTransform in wheelObjects)
        {
            // Отключаем старые модели
            var renderer = wheelTransform.GetComponent<MeshRenderer>();
            if (renderer) renderer.enabled = false;
            
            foreach (Transform child in wheelTransform)
                child.gameObject.SetActive(false);
            
            // Создаем новое колесо
            GameObject newWheel = Instantiate(wheelPrefab, 
                wheelTransform.position, 
                wheelTransform.rotation, 
                wheelTransform);
            
            // Зеркалируем правые колеса
            if (wheelTransform.localPosition.x > 0f)
            {
                var scale = newWheel.transform.localScale;
                newWheel.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
        }
    }
    
    /// <summary>
    /// Рекурсивный поиск объектов колес
    /// </summary>
    private void FindWheelObjects(Transform parent, List<Transform> wheelObjects)
    {
        if (parent.name.ToLower().Contains("wheel"))
        {
            wheelObjects.Add(parent);
        }
        
        foreach (Transform child in parent)
        {
            FindWheelObjects(child, wheelObjects);
        }
    }
    
    /// <summary>
    /// Получает размер колеса для расчета радиуса
    /// </summary>
    private float GetWheelBounds(GameObject wheelPrefab)
    {
        var renderers = wheelPrefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0.3f; // Стандартный радиус
        
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        
        return Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 0.5f;
    }
    
    private void SaveCustomizations()
    {
        // Сохраняем через DataModule
        if (_dataModule != null)
        {
            var json = JsonUtility.ToJson(_carCustomizations);
            PlayerPrefs.SetString("CarCustomizations", json);
            _dataModule.SaveData();
        }
    }
    
    private void LoadCustomizations()
    {
        if (PlayerPrefs.HasKey("CarCustomizations"))
        {
            try
            {
                var json = PlayerPrefs.GetString("CarCustomizations");
                _carCustomizations = JsonUtility.FromJson<Dictionary<string, CarCustomization>>(json) 
                                   ?? new Dictionary<string, CarCustomization>();
            }
            catch
            {
                _carCustomizations = new Dictionary<string, CarCustomization>();
            }
        }
    }
    
    /// <summary>
    /// Получает индекс цвета в массиве доступных цветов
    /// </summary>
    public int GetColorIndex(Color color)
    {
        for (int i = 0; i < availableColors.Length; i++)
        {
            if (ColorUtility.Equals(availableColors[i], color))
                return i;
        }
        return 0; // Возвращаем белый по умолчанию
    }
    
    /// <summary>
    /// Получает данные колеса по индексу
    /// </summary>
    public WheelData GetWheelData(int wheelIndex)
    {
        if (wheelIndex >= 0 && wheelIndex < availableWheels.Length)
            return availableWheels[wheelIndex];
        return null;
    }
    
    /// <summary>
    /// Разблокирует колесо
    /// </summary>
    public bool UnlockWheel(int wheelIndex)
    {
        if (wheelIndex < 0 || wheelIndex >= availableWheels.Length)
            return false;
            
        var wheelData = availableWheels[wheelIndex];
        
        // Проверяем достаточно ли монет
        if (_dataModule != null && _dataModule.Data.coins >= wheelData.price)
        {
            _dataModule.Data.coins -= wheelData.price;
            wheelData.isUnlocked = true;
            SaveCustomizations();
            _dataModule.SaveData();
            
            Debug.Log($"Wheel {wheelData.wheelName} unlocked for {wheelData.price} coins!");
            return true;
        }
        
        Debug.LogWarning($"Not enough coins to unlock {wheelData.wheelName}. Required: {wheelData.price}, Available: {_dataModule?.Data.coins ?? 0}");
        return false;
    }
    
    /// <summary>
    /// Проверяет разблокировано ли колесо
    /// </summary>
    public bool IsWheelUnlocked(int wheelIndex)
    {
        if (wheelIndex < 0 || wheelIndex >= availableWheels.Length)
            return false;
        return availableWheels[wheelIndex].isUnlocked;
    }
    
    /// <summary>
    /// Получает количество доступных колес
    /// </summary>
    public int GetWheelCount()
    {
        return availableWheels.Length;
    }
    
    #region Color Management Methods
    
    /// <summary>
    /// Выбирает цвет для машины (предварительный просмотр)
    /// </summary>
    public void SelectColor(string carModelName, int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= availableColors.Length)
        {
            Debug.LogError($"Invalid color index: {colorIndex}");
            return;
        }
        
        var colorData = availableColors[colorIndex];
        
        // Применяем цвет к машине для предварительного просмотра
        PaintCar(carModelName, colorData.color);
        
        // Уведомляем о выборе цвета
        OnColorSelected?.Invoke(carModelName, colorIndex, colorData.color);
        
        Debug.Log($"Color {colorData.colorName} selected for {carModelName}");
    }
    
    /// <summary>
    /// Покупает цвет для машины
    /// </summary>
    public bool PurchaseColor(string carModelName, int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= availableColors.Length)
        {
            Debug.LogError($"Invalid color index: {colorIndex}");
            return false;
        }
        
        var colorData = availableColors[colorIndex];
        
        // Проверяем достаточно ли монет
        if (_dataModule != null && _dataModule.Data.coins >= colorData.price)
        {
            _dataModule.Data.coins -= colorData.price;
            colorData.isUnlocked = true;
            
            // Сохраняем разблокированный цвет для конкретной машины
            if (!_carCustomizations.ContainsKey(carModelName))
            {
                _carCustomizations[carModelName] = new CarCustomization();
                _carCustomizations[carModelName].carModelName = carModelName;
            }
            
            // Инициализируем массив если нужно
            if (_carCustomizations[carModelName].unlockedColors == null)
            {
                _carCustomizations[carModelName].unlockedColors = new bool[availableColors.Length];
            }
            
            _carCustomizations[carModelName].unlockedColors[colorIndex] = true;
            
            SaveCustomizations();
            _dataModule.SaveData();
            
            // Уведомляем о покупке
            OnColorPurchased?.Invoke(carModelName, colorIndex);
            
            Debug.Log($"Color {colorData.colorName} purchased for {colorData.price} coins!");
            return true;
        }
        
        Debug.LogWarning($"Not enough coins to purchase {colorData.colorName}. Required: {colorData.price}, Available: {_dataModule?.Data.coins ?? 0}");
        return false;
    }
    
    /// <summary>
    /// Проверяет разблокирован ли цвет для конкретной машины
    /// </summary>
    public bool IsColorUnlocked(string carModelName, int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= availableColors.Length)
            return false;
            
        // Базовые цвета разблокированы по умолчанию
        if (availableColors[colorIndex].isUnlocked)
            return true;
            
        // Проверяем разблокировку для конкретной машины
        if (_carCustomizations.ContainsKey(carModelName))
        {
            var customization = _carCustomizations[carModelName];
            if (customization.unlockedColors != null && colorIndex < customization.unlockedColors.Length)
            {
                return customization.unlockedColors[colorIndex];
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Получает данные о цвете по индексу
    /// </summary>
    public ColorData GetColorData(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= availableColors.Length)
            return null;
        return availableColors[colorIndex];
    }
    
    /// <summary>
    /// Получает количество доступных цветов
    /// </summary>
    public int GetColorCount()
    {
        return availableColors.Length;
    }
    
    /// <summary>
    /// Разблокирует цвет для конкретной машины (для тестирования или наград)
    /// </summary>
    public void UnlockColor(string carModelName, int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= availableColors.Length)
            return;
            
        if (!_carCustomizations.ContainsKey(carModelName))
        {
            _carCustomizations[carModelName] = new CarCustomization();
            _carCustomizations[carModelName].carModelName = carModelName;
        }
        
        if (_carCustomizations[carModelName].unlockedColors == null)
        {
            _carCustomizations[carModelName].unlockedColors = new bool[availableColors.Length];
        }
        
        _carCustomizations[carModelName].unlockedColors[colorIndex] = true;
        SaveCustomizations();
        
        OnColorUnlocked?.Invoke(carModelName, colorIndex);
        
        Debug.Log($"Color {availableColors[colorIndex].colorName} unlocked for {carModelName}");
    }
    
    #endregion
    
    #region Upgrade Management Methods
    
    /// <summary>
    /// Покупает улучшение двигателя
    /// </summary>
    public bool PurchaseEngineUpgrade(string carModelName, int level)
    {
        if (level < 0 || level >= engineUpgrades.Length)
            return false;
            
        var upgrade = engineUpgrades[level];
        
        if (_dataModule != null && _dataModule.Data.coins >= upgrade.price)
        {
            _dataModule.Data.coins -= upgrade.price;
            
            // Сохраняем улучшение для машины
            if (!_carCustomizations.ContainsKey(carModelName))
            {
                _carCustomizations[carModelName] = new CarCustomization();
                _carCustomizations[carModelName].carModelName = carModelName;
            }
            
            var customization = _carCustomizations[carModelName];
            customization.engineLevel = level;
            
            if (customization.unlockedEngineLevels == null)
                customization.unlockedEngineLevels = new bool[engineUpgrades.Length];
                
            customization.unlockedEngineLevels[level] = true;
            
            SaveCustomizations();
            _dataModule.SaveData();
            
            OnEngineUpgraded?.Invoke(carModelName, level);
            
            Debug.Log($"Engine upgrade level {level} purchased for {upgrade.price} coins!");
            return true;
        }
        
        Debug.LogWarning($"Not enough coins to purchase engine upgrade level {level}. Required: {upgrade.price}, Available: {_dataModule?.Data.coins ?? 0}");
        return false;
    }
    
    /// <summary>
    /// Покупает улучшение тормозов
    /// </summary>
    public bool PurchaseBrakeUpgrade(string carModelName, int level)
    {
        if (level < 0 || level >= brakeUpgrades.Length)
            return false;
            
        var upgrade = brakeUpgrades[level];
        
        if (_dataModule != null && _dataModule.Data.coins >= upgrade.price)
        {
            _dataModule.Data.coins -= upgrade.price;
            
            if (!_carCustomizations.ContainsKey(carModelName))
            {
                _carCustomizations[carModelName] = new CarCustomization();
                _carCustomizations[carModelName].carModelName = carModelName;
            }
            
            var customization = _carCustomizations[carModelName];
            customization.brakeLevel = level;
            
            if (customization.unlockedBrakeLevels == null)
                customization.unlockedBrakeLevels = new bool[brakeUpgrades.Length];
                
            customization.unlockedBrakeLevels[level] = true;
            
            SaveCustomizations();
            _dataModule.SaveData();
            
            OnBrakeUpgraded?.Invoke(carModelName, level);
            
            Debug.Log($"Brake upgrade level {level} purchased for {upgrade.price} coins!");
            return true;
        }
        
        Debug.LogWarning($"Not enough coins to purchase brake upgrade level {level}. Required: {upgrade.price}, Available: {_dataModule?.Data.coins ?? 0}");
        return false;
    }
    
    /// <summary>
    /// Покупает улучшение нитро
    /// </summary>
    public bool PurchaseNitroUpgrade(string carModelName, int level)
    {
        if (level < 0 || level >= nitroUpgrades.Length)
            return false;
            
        var upgrade = nitroUpgrades[level];
        
        if (_dataModule != null && _dataModule.Data.coins >= upgrade.price)
        {
            _dataModule.Data.coins -= upgrade.price;
            
            if (!_carCustomizations.ContainsKey(carModelName))
            {
                _carCustomizations[carModelName] = new CarCustomization();
                _carCustomizations[carModelName].carModelName = carModelName;
            }
            
            var customization = _carCustomizations[carModelName];
            customization.nitroLevel = level;
            
            if (customization.unlockedNitroLevels == null)
                customization.unlockedNitroLevels = new bool[nitroUpgrades.Length];
                
            customization.unlockedNitroLevels[level] = true;
            
            SaveCustomizations();
            _dataModule.SaveData();
            
            OnNitroUpgraded?.Invoke(carModelName, level);
            
            Debug.Log($"Nitro upgrade level {level} purchased for {upgrade.price} coins!");
            return true;
        }
        
        Debug.LogWarning($"Not enough coins to purchase nitro upgrade level {level}. Required: {upgrade.price}, Available: {_dataModule?.Data.coins ?? 0}");
        return false;
    }
    
    /// <summary>
    /// Покупает спойлер
    /// </summary>
    public bool PurchaseSpoiler(string carModelName, int spoilerIndex)
    {
        if (spoilerIndex < 0 || spoilerIndex >= availableSpoilers.Length)
            return false;
            
        var spoiler = availableSpoilers[spoilerIndex];
        
        if (_dataModule != null && _dataModule.Data.coins >= spoiler.price)
        {
            _dataModule.Data.coins -= spoiler.price;
            
            if (!_carCustomizations.ContainsKey(carModelName))
            {
                _carCustomizations[carModelName] = new CarCustomization();
                _carCustomizations[carModelName].carModelName = carModelName;
            }
            
            var customization = _carCustomizations[carModelName];
            customization.selectedSpoilerIndex = spoilerIndex;
            
            if (customization.unlockedSpoilers == null)
                customization.unlockedSpoilers = new bool[availableSpoilers.Length];
                
            customization.unlockedSpoilers[spoilerIndex] = true;
            
            SaveCustomizations();
            _dataModule.SaveData();
            
            OnSpoilerChanged?.Invoke(carModelName, spoilerIndex);
            
            Debug.Log($"Spoiler {spoiler.spoilerName} purchased for {spoiler.price} coins!");
            return true;
        }
        
        Debug.LogWarning($"Not enough coins to purchase spoiler {spoiler.spoilerName}. Required: {spoiler.price}, Available: {_dataModule?.Data.coins ?? 0}");
        return false;
    }
    
    /// <summary>
    /// Проверяет разблокировано ли улучшение двигателя
    /// </summary>
    public bool IsEngineUpgradeUnlocked(string carModelName, int level)
    {
        if (level < 0 || level >= engineUpgrades.Length)
            return false;
            
        if (engineUpgrades[level].isUnlocked)
            return true;
            
        if (_carCustomizations.ContainsKey(carModelName))
        {
            var customization = _carCustomizations[carModelName];
            if (customization.unlockedEngineLevels != null && level < customization.unlockedEngineLevels.Length)
            {
                return customization.unlockedEngineLevels[level];
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Проверяет разблокировано ли улучшение тормозов
    /// </summary>
    public bool IsBrakeUpgradeUnlocked(string carModelName, int level)
    {
        if (level < 0 || level >= brakeUpgrades.Length)
            return false;
            
        if (brakeUpgrades[level].isUnlocked)
            return true;
            
        if (_carCustomizations.ContainsKey(carModelName))
        {
            var customization = _carCustomizations[carModelName];
            if (customization.unlockedBrakeLevels != null && level < customization.unlockedBrakeLevels.Length)
            {
                return customization.unlockedBrakeLevels[level];
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Проверяет разблокировано ли улучшение нитро
    /// </summary>
    public bool IsNitroUpgradeUnlocked(string carModelName, int level)
    {
        if (level < 0 || level >= nitroUpgrades.Length)
            return false;
            
        if (nitroUpgrades[level].isUnlocked)
            return true;
            
        if (_carCustomizations.ContainsKey(carModelName))
        {
            var customization = _carCustomizations[carModelName];
            if (customization.unlockedNitroLevels != null && level < customization.unlockedNitroLevels.Length)
            {
                return customization.unlockedNitroLevels[level];
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Проверяет разблокирован ли спойлер
    /// </summary>
    public bool IsSpoilerUnlocked(string carModelName, int spoilerIndex)
    {
        if (spoilerIndex < 0 || spoilerIndex >= availableSpoilers.Length)
            return false;
            
        if (availableSpoilers[spoilerIndex].isUnlocked)
            return true;
            
        if (_carCustomizations.ContainsKey(carModelName))
        {
            var customization = _carCustomizations[carModelName];
            if (customization.unlockedSpoilers != null && spoilerIndex < customization.unlockedSpoilers.Length)
            {
                return customization.unlockedSpoilers[spoilerIndex];
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Получает данные улучшения двигателя
    /// </summary>
    public UpgradeData GetEngineUpgradeData(int level)
    {
        if (level < 0 || level >= engineUpgrades.Length)
            return null;
        return engineUpgrades[level];
    }
    
    /// <summary>
    /// Получает данные улучшения тормозов
    /// </summary>
    public UpgradeData GetBrakeUpgradeData(int level)
    {
        if (level < 0 || level >= brakeUpgrades.Length)
            return null;
        return brakeUpgrades[level];
    }
    
    /// <summary>
    /// Получает данные улучшения нитро
    /// </summary>
    public UpgradeData GetNitroUpgradeData(int level)
    {
        if (level < 0 || level >= nitroUpgrades.Length)
            return null;
        return nitroUpgrades[level];
    }
    
    /// <summary>
    /// Получает данные спойлера
    /// </summary>
    public SpoilerData GetSpoilerData(int spoilerIndex)
    {
        if (spoilerIndex < 0 || spoilerIndex >= availableSpoilers.Length)
            return null;
        return availableSpoilers[spoilerIndex];
    }
    
    /// <summary>
    /// Получает количество уровней улучшений
    /// </summary>
    public int GetEngineUpgradeCount() => engineUpgrades.Length;
    public int GetBrakeUpgradeCount() => brakeUpgrades.Length;
    public int GetNitroUpgradeCount() => nitroUpgrades.Length;
    public int GetSpoilerCount() => availableSpoilers.Length;
    
    #endregion
    
    #region Car Index Management Methods
    
    /// <summary>
    /// Получает кастомизацию машины по индексу
    /// </summary>
    public CarCustomization GetCarCustomizationByIndex(int carIndex)
    {
        if (!_carCustomizationsByIndex.ContainsKey(carIndex))
        {
            _carCustomizationsByIndex[carIndex] = new CarCustomization();
            _carCustomizationsByIndex[carIndex].carModelName = $"Car_{carIndex}";
        }
        return _carCustomizationsByIndex[carIndex];
    }
    
    /// <summary>
    /// Применяет кастомизацию к машине по индексу.
    /// ВНИМАНИЕ: Используйте CarCustomizationApplier для применения к GameObject!
    /// Этот метод для обратной совместимости.
    /// </summary>
    [System.Obsolete("Use CarCustomizationApplier.Instance.ApplyCustomization() instead")]
    public void ApplyCustomizationToCarByIndex(GameObject carObject, int carIndex)
    {
        var customization = GetCarCustomizationByIndex(carIndex);
        CarCustomizationApplier.Instance.ApplyCustomization(carObject, customization, this);
        Debug.Log($"[CustomizationModule] Applied customization to car index {carIndex} via CarCustomizationApplier");
    }
    
    /// <summary>
    /// Сохраняет кастомизацию машины по индексу
    /// </summary>
    public void SaveCarCustomizationByIndex(int carIndex)
    {
        if (!_carCustomizationsByIndex.ContainsKey(carIndex))
            return;
            
        var customization = _carCustomizationsByIndex[carIndex];
        
        // Сохраняем в DataModule
        SaveCustomizationToDataModule(carIndex, customization);
        _dataModule.SaveData();
        
        Debug.Log($"[CustomizationModule] Saved customization for car index {carIndex}");
    }
    
    /// <summary>
    /// Загружает кастомизацию машины по индексу
    /// </summary>
    public void LoadCarCustomizationByIndex(int carIndex)
    {
        var customization = LoadCustomizationFromDataModule(carIndex);
        if (customization != null)
        {
            _carCustomizationsByIndex[carIndex] = customization;
            Debug.Log($"[CustomizationModule] Loaded customization for car index {carIndex}");
        }
    }
    
    /// <summary>
    /// Сохраняет кастомизацию в DataModule
    /// </summary>
    private void SaveCustomizationToDataModule(int carIndex, CarCustomization customization)
    {
        if (_dataModule == null) return;
        
        var data = new DataModule.CarCustomizationData
        {
            carIndex = carIndex,
            colorR = customization.paintColor.r.ToString(),
            colorG = customization.paintColor.g.ToString(),
            colorB = customization.paintColor.b.ToString(),
            colorA = customization.paintColor.a.ToString(),
            selectedWheelIndex = customization.selectedWheelIndex,
            selectedSpoilerIndex = customization.selectedSpoilerIndex,
            engineLevel = customization.engineLevel,
            brakeLevel = customization.brakeLevel,
            nitroLevel = customization.nitroLevel,
            handlingLevel = customization.handlingLevel,
            unlockedColorsJson = JsonUtility.ToJson(new BoolArrayWrapper { values = customization.unlockedColors }),
            unlockedEngineLevelsJson = JsonUtility.ToJson(new BoolArrayWrapper { values = customization.unlockedEngineLevels }),
            unlockedBrakeLevelsJson = JsonUtility.ToJson(new BoolArrayWrapper { values = customization.unlockedBrakeLevels }),
            unlockedNitroLevelsJson = JsonUtility.ToJson(new BoolArrayWrapper { values = customization.unlockedNitroLevels }),
            unlockedHandlingLevelsJson = JsonUtility.ToJson(new BoolArrayWrapper { values = customization.unlockedHandlingLevels }),
            unlockedSpoilersJson = JsonUtility.ToJson(new BoolArrayWrapper { values = customization.unlockedSpoilers })
        };
        
        // Сохраняем в PlayerPrefs по ключу
        string key = $"CarCustomization_{carIndex}";
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
    }
    
    /// <summary>
    /// Загружает кастомизацию из DataModule
    /// </summary>
    private CarCustomization LoadCustomizationFromDataModule(int carIndex)
    {
        string key = $"CarCustomization_{carIndex}";
        if (!PlayerPrefs.HasKey(key))
            return null;
            
        string json = PlayerPrefs.GetString(key);
        var data = JsonUtility.FromJson<DataModule.CarCustomizationData>(json);
        
        var customization = new CarCustomization
        {
            carModelName = $"Car_{carIndex}",
            paintColor = new Color(
                float.Parse(data.colorR),
                float.Parse(data.colorG),
                float.Parse(data.colorB),
                float.Parse(data.colorA)
            ),
            selectedWheelIndex = data.selectedWheelIndex,
            selectedSpoilerIndex = data.selectedSpoilerIndex,
            engineLevel = data.engineLevel,
            brakeLevel = data.brakeLevel,
            nitroLevel = data.nitroLevel,
            handlingLevel = data.handlingLevel
        };
        
        // Загружаем массивы разблокировок
        if (!string.IsNullOrEmpty(data.unlockedColorsJson))
        {
            var wrapper = JsonUtility.FromJson<BoolArrayWrapper>(data.unlockedColorsJson);
            customization.unlockedColors = wrapper.values;
        }
        
        if (!string.IsNullOrEmpty(data.unlockedEngineLevelsJson))
        {
            var wrapper = JsonUtility.FromJson<BoolArrayWrapper>(data.unlockedEngineLevelsJson);
            customization.unlockedEngineLevels = wrapper.values;
        }
        
        if (!string.IsNullOrEmpty(data.unlockedBrakeLevelsJson))
        {
            var wrapper = JsonUtility.FromJson<BoolArrayWrapper>(data.unlockedBrakeLevelsJson);
            customization.unlockedBrakeLevels = wrapper.values;
        }
        
        if (!string.IsNullOrEmpty(data.unlockedNitroLevelsJson))
        {
            var wrapper = JsonUtility.FromJson<BoolArrayWrapper>(data.unlockedNitroLevelsJson);
            customization.unlockedNitroLevels = wrapper.values;
        }
        
        if (!string.IsNullOrEmpty(data.unlockedHandlingLevelsJson))
        {
            var wrapper = JsonUtility.FromJson<BoolArrayWrapper>(data.unlockedHandlingLevelsJson);
            customization.unlockedHandlingLevels = wrapper.values;
        }
        
        if (!string.IsNullOrEmpty(data.unlockedSpoilersJson))
        {
            var wrapper = JsonUtility.FromJson<BoolArrayWrapper>(data.unlockedSpoilersJson);
            customization.unlockedSpoilers = wrapper.values;
        }
        
        return customization;
    }
    
    [System.Serializable]
    private class BoolArrayWrapper
    {
        public bool[] values;
    }
    
    /// <summary>
    /// Загружает все кастомизации при инициализации
    /// </summary>
    public void LoadAllCarCustomizations()
    {
        for (int i = 0; i < 5; i++) // 5 машин в магазине
        {
            LoadCarCustomizationByIndex(i);
        }
    }
    
    #endregion
}
