using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управляет отображением машин на сцене Level с применением сохраненной кастомизации.
/// </summary>
public class PlayerView : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cars;

    private DataModule _dataModule;
    private CustomizationModule _customizationModule;

    private void Awake()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        _customizationModule = ModuleManager.Instance.GetModule<CustomizationModule>();
    }

    private void Start()
    {
        if (_dataModule != null && _dataModule.IsInitialized)
        {
            PlacingSkin(_dataModule.Data.appliedCarIndex);
            ApplyCustomization(_dataModule.Data.appliedCarIndex);
        }
        else
        {
            Debug.Log("DataModule not initialized");
        }
    }

    private void PlacingSkin(int index)
    {
        foreach (var car in _cars)
        {
            car.SetActive(false);
        }
        if (index >= 0 && index < _cars.Count)
        {
            _cars[index].SetActive(true);
        }
    }
    
    /// <summary>
    /// Применяет сохраненную кастомизацию к активной машине
    /// </summary>
    private void ApplyCustomization(int carIndex)
    {
        if (_customizationModule == null || carIndex < 0 || carIndex >= _cars.Count)
            return;
            
        var activeCar = _cars[carIndex];
        if (activeCar != null && activeCar.activeSelf)
        {
            // Получаем кастомизацию из Module
            var customization = _customizationModule.GetCarCustomizationByIndex(carIndex);
            
            // Применяем через сервис
            CarCustomizationApplier.Instance.ApplyCustomization(activeCar, customization, _customizationModule);
            
            Debug.Log($"[PlayerView] Applied customization to car index {carIndex} on Level scene");
        }
    }
    
    /// <summary>
    /// Обновляет машину с новой кастомизацией (вызывается при изменении машины в магазине)
    /// </summary>
    public void RefreshCarCustomization()
    {
        if (_dataModule != null)
        {
            PlacingSkin(_dataModule.Data.appliedCarIndex);
            ApplyCustomization(_dataModule.Data.appliedCarIndex);
        }
    }
}
