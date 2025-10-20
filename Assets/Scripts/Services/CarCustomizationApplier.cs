using UnityEngine;

/// <summary>
/// Сервис для применения кастомизации к GameObject машин.
/// Отделен от CustomizationModule - отвечает ТОЛЬКО за визуальное применение изменений.
/// </summary>
public class CarCustomizationApplier : MonoBehaviour
{
    private static CarCustomizationApplier _instance;
    public static CarCustomizationApplier Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("CarCustomizationApplier");
                _instance = go.AddComponent<CarCustomizationApplier>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Применяет кастомизацию к машине
    /// </summary>
    public void ApplyCustomization(GameObject carObject, int carIndex)
    {
        if (carObject == null) return;
        
        var paintModule = ModuleManager.Instance?.GetModule<PaintCustomizationModule>();
        var wheelModule = ModuleManager.Instance?.GetModule<WheelCustomizationModule>();
        var spoilerModule = ModuleManager.Instance?.GetModule<SpoilerCustomizationModule>();
        var performanceModule = ModuleManager.Instance?.GetModule<PerformanceUpgradeModule>();
        
        if (paintModule != null)
            ApplyPaint(carObject, paintModule.GetCurrentColor(carIndex));
        
        if (wheelModule != null)
            ApplyWheels(carObject, wheelModule.GetCurrentWheelIndex(carIndex), wheelModule);
        
        if (spoilerModule != null)
            ApplySpoiler(carObject, spoilerModule.GetCurrentSpoilerIndex(carIndex), spoilerModule);
        
        if (performanceModule != null)
            ApplyUpgrades(carObject, carIndex, performanceModule);
        
        Debug.Log($"[CarCustomizationApplier] Applied customization to {carObject.name}");
    }
    
    #region Paint Application
    
    private void ApplyPaint(GameObject carObject, Color color)
    {
        // Способ 1: Через RCC_CustomizationApplier
        var customizationApplier = carObject.GetComponent<RCC_CustomizationApplier>();
        if (customizationApplier != null && customizationApplier.PaintManager != null)
        {
            customizationApplier.PaintManager.Paint(color);
            return;
        }
        
        // Способ 2: Напрямую через MeshRenderer
        var renderers = carObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            if (IsCarBodyRenderer(renderer))
            {
                foreach (var material in renderer.materials)
                {
                    material.color = color;
                }
            }
        }
    }
    
    private bool IsCarBodyRenderer(MeshRenderer renderer)
    {
        string name = renderer.name.ToLower();
        return name.Contains("body") || 
               name.Contains("car") || 
               renderer.gameObject.layer == LayerMask.NameToLayer("Vehicle");
    }
    
    #endregion
    
    #region Wheels Application
    
    private void ApplyWheels(GameObject carObject, int wheelIndex, WheelCustomizationModule module)
    {
        var wheelData = module.GetWheelData(wheelIndex);
        if (wheelData == null || wheelData.wheelPrefab == null) return;
        
        // Способ 1: Через RCC систему
        var customizationApplier = carObject.GetComponent<RCC_CustomizationApplier>();
        if (customizationApplier != null && customizationApplier.WheelManager != null)
        {
            customizationApplier.WheelManager.UpdateWheel(wheelIndex);
            return;
        }
        
        // Способ 2: Через RCC_CarControllerV3
        var carController = carObject.GetComponent<RCC_CarControllerV3>();
        if (carController != null)
        {
            ApplyWheelsToRCC(carController, wheelData.wheelPrefab);
        }
    }
    
    private void ApplyWheelsToRCC(RCC_CarControllerV3 vehicle, GameObject wheelPrefab)
    {
        for (int i = 0; i < vehicle.AllWheelColliders.Length; i++)
        {
            var wheelCollider = vehicle.AllWheelColliders[i];
            
            // Находим визуальное представление колеса
            Transform wheelModel = wheelCollider.transform.Find("WheelModel");
            if (wheelModel != null)
            {
                // Удаляем старую модель
                foreach (Transform child in wheelModel)
                {
                    Destroy(child.gameObject);
                }
                
                // Создаем новую модель
                GameObject newWheel = Instantiate(wheelPrefab, wheelModel);
                newWheel.transform.localPosition = Vector3.zero;
                newWheel.transform.localRotation = Quaternion.identity;
            }
        }
    }
    
    #endregion
    
    #region Spoiler Application
    
    private void ApplySpoiler(GameObject carObject, int spoilerIndex, SpoilerCustomizationModule module)
    {
        if (spoilerIndex < 0) return;
        
        var spoilerData = module.GetSpoilerData(spoilerIndex);
        if (spoilerData == null) return;
        
        // Находим точку крепления спойлера
        Transform spoilerPoint = carObject.transform.Find("SpoilerPoint");
        if (spoilerPoint == null)
        {
            // Создаем точку крепления если не существует
            spoilerPoint = new GameObject("SpoilerPoint").transform;
            spoilerPoint.SetParent(carObject.transform);
            spoilerPoint.localPosition = new Vector3(0, 0.5f, -1.5f); // Примерная позиция
        }
        
        // Удаляем старый спойлер
        foreach (Transform child in spoilerPoint)
        {
            Destroy(child.gameObject);
        }
        
        // Устанавливаем новый спойлер
        if (spoilerData.spoilerPrefab != null)
        {
            GameObject spoiler = Instantiate(spoilerData.spoilerPrefab, spoilerPoint);
            spoiler.transform.localPosition = Vector3.zero;
            spoiler.transform.localRotation = Quaternion.identity;
        }
    }
    
    #endregion
    
    #region Upgrades Application
    
    private void ApplyUpgrades(GameObject carObject, int carIndex, PerformanceUpgradeModule module)
    {
        var carController = carObject.GetComponent<RCC_CarControllerV3>();
        if (carController == null) return;
        
        // Получаем текущие уровни улучшений
        int engineLevel = module.GetEngineLevel(carIndex);
        int brakeLevel = module.GetBrakeLevel(carIndex);
        int nitroLevel = module.GetNitroLevel(carIndex);
        
        // Получаем базовые значения
        float baseEngineTorque = carController.maxEngineTorque;
        float baseBrakeTorque = carController.brakeTorque;
        
        // Применяем улучшения двигателя
        if (engineLevel > 0)
        {
            var engineUpgrade = module.GetEngineUpgradeData(engineLevel);
            if (engineUpgrade != null)
            {
                carController.maxEngineTorque = baseEngineTorque * engineUpgrade.powerMultiplier;
            }
        }
        
        // Применяем улучшения тормозов
        if (brakeLevel > 0)
        {
            var brakeUpgrade = module.GetBrakeUpgradeData(brakeLevel);
            if (brakeUpgrade != null)
            {
                carController.brakeTorque = baseBrakeTorque * brakeUpgrade.powerMultiplier;
            }
        }
        
        // Применяем улучшения нитро
        var nitroModule = carObject.GetComponent<NitroModule>();
        if (nitroModule != null && nitroLevel > 0)
        {
            var nitroUpgrade = module.GetNitroUpgradeData(nitroLevel);
            // NitroModule сам должен применить улучшение
        }
        
        Debug.Log($"[CarCustomizationApplier] Applied upgrades: Engine={engineLevel}, Brake={brakeLevel}, Nitro={nitroLevel}");
    }
    
    #endregion
}
