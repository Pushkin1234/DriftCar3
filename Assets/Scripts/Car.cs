using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Менеджеры кастомизации")]
    [SerializeField] private PaintManager _paintManager;
    [SerializeField] private WheelsManager _wheelsManager;
    [SerializeField] private UpgradeManager _upgradeManager;
    [SerializeField] private SpoilerManager _spoilerManager;

    private void Awake()
    {
        // Автоматически находим менеджеры, если они не назначены в инспекторе
        FindManagersIfNeeded();
    }

    /// <summary>
    /// Найти менеджеры в дочерних объектах, если они не назначены
    /// </summary>
    private void FindManagersIfNeeded()
    {
        if (_paintManager == null)
            _paintManager = GetComponentInChildren<PaintManager>();

        if (_wheelsManager == null)
            _wheelsManager = GetComponentInChildren<WheelsManager>();

        if (_upgradeManager == null)
            _upgradeManager = GetComponentInChildren<UpgradeManager>();

        if (_spoilerManager == null)
            _spoilerManager = GetComponentInChildren<SpoilerManager>();
    }

    /// <summary>
    /// Инициализировать все менеджеры с данными из лоад-аута
    /// </summary>
    public void InitializeManagers(CarUpGradeHandler.CarLoadout loadout)
    {
        FindManagersIfNeeded();

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

    #region Getters для менеджеров

    /// <summary>
    /// Получить менеджер покраски
    /// </summary>
    public PaintManager GetPaintManager()
    {
        if (_paintManager == null)
            _paintManager = GetComponentInChildren<PaintManager>();
        return _paintManager;
    }

    /// <summary>
    /// Получить менеджер колёс
    /// </summary>
    public WheelsManager GetWheelsManager()
    {
        if (_wheelsManager == null)
            _wheelsManager = GetComponentInChildren<WheelsManager>();
        return _wheelsManager;
    }

    /// <summary>
    /// Получить менеджер апгрейдов
    /// </summary>
    public UpgradeManager GetUpgradeManager()
    {
        if (_upgradeManager == null)
            _upgradeManager = GetComponentInChildren<UpgradeManager>();
        return _upgradeManager;
    }

    /// <summary>
    /// Получить менеджер спойлеров
    /// </summary>
    public SpoilerManager GetSpoilerManager()
    {
        if (_spoilerManager == null)
            _spoilerManager = GetComponentInChildren<SpoilerManager>();
        return _spoilerManager;
    }

    #endregion
}