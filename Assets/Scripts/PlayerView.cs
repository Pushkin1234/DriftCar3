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
        if (carIndex < 0 || carIndex >= _cars.Count)
        {
            Debug.LogWarning($"[PlayerView] Неверный индекс машины: {carIndex}");
            return;
        }

        GameObject activeCar = _cars[carIndex];
        if (activeCar == null || !activeCar.activeSelf)
        {
            Debug.LogWarning($"[PlayerView] Машина с индексом {carIndex} не активна!");
            return;
        }

        // Загружаем и применяем сохраненный цвет
        Color savedColor = LoadColorFromPlayerPrefs(carIndex);
        ApplyPaintColor(activeCar, savedColor);

        Debug.Log($"[PlayerView] Применен цвет {savedColor} для машины {carIndex}");
    }

    /// <summary>
    /// Загрузить цвет из PlayerPrefs
    /// </summary>
    private Color LoadColorFromPlayerPrefs(int carIndex)
    {
        string keyR = $"CarColor_{carIndex}_R";
        string keyG = $"CarColor_{carIndex}_G";
        string keyB = $"CarColor_{carIndex}_B";
        string keyA = $"CarColor_{carIndex}_A";

        // Если цвет сохранен - загружаем, иначе возвращаем белый по умолчанию
        if (PlayerPrefs.HasKey(keyR))
        {
            float r = PlayerPrefs.GetFloat(keyR, 1f);
            float g = PlayerPrefs.GetFloat(keyG, 1f);
            float b = PlayerPrefs.GetFloat(keyB, 1f);
            float a = PlayerPrefs.GetFloat(keyA, 1f);
            return new Color(r, g, b, a);
        }

        // Цвет не сохранен - возвращаем белый по умолчанию
        return Color.white;
    }

    /// <summary>
    /// Применить цвет покраски к машине
    /// </summary>
    private void ApplyPaintColor(GameObject car, Color color)
    {
        PaintManager paintManager = car.GetComponentInChildren<PaintManager>();
        if (paintManager != null)
        {
            paintManager.Paint(color);
        }
        else
        {
            Debug.LogWarning($"[PlayerView] PaintManager не найден на машине {car.name}");
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
