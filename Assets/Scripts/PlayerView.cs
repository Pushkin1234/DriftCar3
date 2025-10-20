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
    private PaintCustomizationModule _paintModule;
    private WheelCustomizationModule _wheelModule;
    private PerformanceUpgradeModule _performanceModule;
    private SpoilerCustomizationModule _spoilerModule;

    private void Awake()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        _paintModule = ModuleManager.Instance.GetModule<PaintCustomizationModule>();
        _wheelModule = ModuleManager.Instance.GetModule<WheelCustomizationModule>();
        _performanceModule = ModuleManager.Instance.GetModule<PerformanceUpgradeModule>();
        _spoilerModule = ModuleManager.Instance.GetModule<SpoilerCustomizationModule>();
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
