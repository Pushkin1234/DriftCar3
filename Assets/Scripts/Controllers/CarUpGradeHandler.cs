using UnityEngine;

/// <summary>
/// Центральный обработчик кастомизации автомобиля.
/// Аналог RCC_CustomizationApplier, но интегрированный с модульной архитектурой.
/// </summary>
public class CarUpGradeHandler : MonoBehaviour
{
    [Header("Модули кастомизации")]
    [SerializeField] private PaintCustomizationModule _paintModule;
    [SerializeField] private WheelCustomizationModule _wheelModule;
    [SerializeField] private PerformanceUpgradeModule _performanceModule;
    [SerializeField] private SpoilerCustomizationModule _spoilerModule;
    [SerializeField] private DataModule _dataModule;

    [Header("Менеджеры кастомизации")]
    [SerializeField] private PaintManager _paintManager;
    [SerializeField] private WheelsManager _wheelsManager;
    [SerializeField] private UpgradeManager _upgradeManager;
    [SerializeField] private SpoilerManager _spoilerManager;

    [Header("Настройки сохранения")]
    public string saveFileName = "";
    public bool autoLoadLoadout = true;

    // Текущий лоад-аут машины (аналог RCC_CustomizationLoadout)
    [System.Serializable]
    public class CarLoadout
    {
        public Color paintColor = Color.white;
        public int wheelIndex = 0;
        public int spoilerIndex = -1;
        public int engineLevel = 0;
        public int brakeLevel = 0;
        public int nitroLevel = 0;
        public int handlingLevel = 0;
    }

    public CarLoadout loadout = new CarLoadout();
    private int _currentCarIndex = 0;

    #region Initialization

    private void Start()
    {
        InitializeModules();
        
        if (autoLoadLoadout)
            LoadLoadout();

        InitializeManagers();
    }

    private void InitializeModules()
    {
        if (_paintModule == null)
            _paintModule = ModuleManager.Instance?.GetModule<PaintCustomizationModule>();
        if (_wheelModule == null)
            _wheelModule = ModuleManager.Instance?.GetModule<WheelCustomizationModule>();
        if (_performanceModule == null)
            _performanceModule = ModuleManager.Instance?.GetModule<PerformanceUpgradeModule>();
        if (_spoilerModule == null)
            _spoilerModule = ModuleManager.Instance?.GetModule<SpoilerCustomizationModule>();
        if (_dataModule == null)
            _dataModule = ModuleManager.Instance?.GetModule<DataModule>();

        if (_dataModule != null)
            _currentCarIndex = _dataModule.Data.appliedCarIndex;
    }

    private void InitializeManagers()
    {
        // Инициализируем все менеджеры с текущими данными
        if (_paintManager != null)
            _paintManager.Initialize(loadout.paintColor);

        if (_wheelsManager != null)
            _wheelsManager.Initialize(loadout.wheelIndex);

        if (_upgradeManager != null)
            _upgradeManager.Initialize(loadout.engineLevel, loadout.brakeLevel, loadout.handlingLevel, loadout.nitroLevel);

        if (_spoilerManager != null)
            _spoilerManager.Initialize(loadout.spoilerIndex);
    }

    #endregion

    #region Loadout Management

    /// <summary>
    /// Сохранить текущий лоад-аут
    /// </summary>
    public void SaveLoadout()
    {
        if (string.IsNullOrEmpty(saveFileName))
            saveFileName = transform.name;

        PlayerPrefs.SetString(saveFileName, JsonUtility.ToJson(loadout));
        PlayerPrefs.Save();

        // Также сохраняем через модули для централизованного хранения
        SaveToModules();
    }

    /// <summary>
    /// Загрузить сохранённый лоад-аут
    /// </summary>
    public void LoadLoadout()
    {
        if (string.IsNullOrEmpty(saveFileName))
            saveFileName = transform.name;

        loadout = new CarLoadout();

        if (PlayerPrefs.HasKey(saveFileName))
        {
            loadout = JsonUtility.FromJson<CarLoadout>(PlayerPrefs.GetString(saveFileName));
        }
        else
        {
            // Загружаем из модулей, если нет локального сохранения
            LoadFromModules();
        }
    }

    /// <summary>
    /// Сохранить данные в модули
    /// </summary>
    private void SaveToModules()
    {
        if (_paintModule != null)
            _paintModule.PaintCar(_currentCarIndex, loadout.paintColor);

        if (_wheelModule != null)
            _wheelModule.ChangeWheels(_currentCarIndex, loadout.wheelIndex);

        if (_performanceModule != null)
        {
            // Здесь нужно будет добавить методы для установки уровней
            // _performanceModule.SetEngineLevel(_currentCarIndex, loadout.engineLevel);
            // _performanceModule.SetBrakeLevel(_currentCarIndex, loadout.brakeLevel);
            // и т.д.
        }

        if (_spoilerModule != null)
            _spoilerModule.ChangeSpoiler(_currentCarIndex, loadout.spoilerIndex);
    }

    /// <summary>
    /// Загрузить данные из модулей
    /// </summary>
    private void LoadFromModules()
    {
        if (_paintModule != null)
            loadout.paintColor = _paintModule.GetCurrentColor(_currentCarIndex);

        if (_wheelModule != null)
            loadout.wheelIndex = _wheelModule.GetCurrentWheelIndex(_currentCarIndex);

        if (_performanceModule != null)
        {
            loadout.engineLevel = _performanceModule.GetEngineLevel(_currentCarIndex);
            loadout.brakeLevel = _performanceModule.GetBrakeLevel(_currentCarIndex);
            loadout.nitroLevel = _performanceModule.GetNitroLevel(_currentCarIndex);
            loadout.handlingLevel = _performanceModule.GetHandlingLevel(_currentCarIndex);
        }

        if (_spoilerModule != null)
            loadout.spoilerIndex = _spoilerModule.GetCurrentSpoilerIndex(_currentCarIndex);
    }

    #endregion

    #region Public API for Managers

    /// <summary>
    /// Установить цвет покраски
    /// </summary>
    public void SetPaintColor(Color color)
    {
        loadout.paintColor = color;
        if (_paintManager != null)
            _paintManager.Paint(color);
    }

    /// <summary>
    /// Установить индекс колёс
    /// </summary>
    public void SetWheelIndex(int wheelIndex)
    {
        loadout.wheelIndex = wheelIndex;
        if (_wheelsManager != null)
            _wheelsManager.SetCurrentWheels(wheelIndex);
    }

    /// <summary>
    /// Установить уровень двигателя
    /// </summary>
    public void SetEngineLevel(int level)
    {
        loadout.engineLevel = level;
        if (_upgradeManager != null)
            _upgradeManager.SetCurrentEngine(level);
    }

    /// <summary>
    /// Установить уровень тормозов
    /// </summary>
    public void SetBrakeLevel(int level)
    {
        loadout.brakeLevel = level;
        if (_upgradeManager != null)
            _upgradeManager.SetCurrentBrake(level);
    }

    /// <summary>
    /// Установить уровень нитро
    /// </summary>
    public void SetNitroLevel(int level)
    {
        loadout.nitroLevel = level;
        if (_upgradeManager != null)
            _upgradeManager.SetCurrentNitro(level);
    }

    /// <summary>
    /// Установить уровень управляемости
    /// </summary>
    public void SetHandlingLevel(int level)
    {
        loadout.handlingLevel = level;
        if (_upgradeManager != null)
            _upgradeManager.SetCurrentHandling(level);
    }

    /// <summary>
    /// Установить индекс спойлера
    /// </summary>
    public void SetSpoilerIndex(int spoilerIndex)
    {
        loadout.spoilerIndex = spoilerIndex;
        if (_spoilerManager != null)
            _spoilerManager.SetCurrentSpoiler(spoilerIndex);
    }

    #endregion

    #region Utility

    /// <summary>
    /// Получить текущий лоад-аут
    /// </summary>
    public CarLoadout GetLoadout() => loadout;

    /// <summary>
    /// Установить лоад-аут извне
    /// </summary>
    public void SetLoadout(CarLoadout newLoadout)
    {
        loadout = newLoadout;
        InitializeManagers();
    }

    private void Reset()
    {
        saveFileName = transform.name;
    }

    #endregion
}