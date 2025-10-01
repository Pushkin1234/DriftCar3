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
    
    // Доступные цвета для кастомизации
    public Color[] availableColors = {
        Color.white, Color.black, Color.red, Color.blue, 
        Color.green, Color.yellow, Color.cyan, Color.magenta
    };
    
    // Доступные колеса для кастомизации
    public WheelData[] availableWheels = new WheelData[0];
    
    // События для уведомления других систем
    public System.Action<string, Color> OnCarPainted;
    public System.Action<string, int> OnWheelsChanged;
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        LoadCustomizations();
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
}
