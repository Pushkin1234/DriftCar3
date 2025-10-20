using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Менеджеры кастомизации")]
    [SerializeField] private PaintManager _paintManager;
    [SerializeField] private WheelsManager _wheelsManager;
    [SerializeField] private UpgradeManager _upgradeManager;
    [SerializeField] private SpoilerManager _spoilerManager;

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
}