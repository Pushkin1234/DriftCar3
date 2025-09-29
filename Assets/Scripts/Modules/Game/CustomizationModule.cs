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
    
    private DataModule _dataModule;
    private Dictionary<string, CarCustomization> _carCustomizations = new Dictionary<string, CarCustomization>();
    
    // Доступные цвета для кастомизации
    public Color[] availableColors = {
        Color.white, Color.black, Color.red, Color.blue, 
        Color.green, Color.yellow, Color.cyan, Color.magenta
    };
    
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
        // Интеграция с RCC системой смены колес
        var customizationApplier = carObject.GetComponent<RCC_CustomizationApplier>();
        if (customizationApplier != null && customizationApplier.WheelManager != null)
        {
            // Используем RCC систему смены колес
            // customizationApplier.WheelManager.ChangeWheels(wheelIndex);
        }
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
}
