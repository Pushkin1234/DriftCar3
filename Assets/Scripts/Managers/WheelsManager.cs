using UnityEngine;

/// <summary>
/// Управляет всеми вариантами колёс автомобиля.
/// Активирует только выбранные колёса, остальные скрывает.
/// </summary>
public class WheelsManager : MonoBehaviour
{
    [Header("Все варианты колёс на машине")]
    public VehicleUpgradeWheel[] wheels;

    /// <summary>
    /// Инициализация - установить колёса по данным из модуля
    /// </summary>
    public void Initialize(int selectedWheelIndex)
    {
        if (wheels == null || wheels.Length == 0)
            wheels = GetComponentsInChildren<VehicleUpgradeWheel>(true);

        SetCurrentWheels(selectedWheelIndex);
    }

    /// <summary>
    /// Установить колёса по индексу
    /// </summary>
    public void SetCurrentWheels(int wheelIndex)
    {
        if (wheels == null || wheels.Length == 0)
            return;

        foreach (var wheel in wheels)
        {
            bool shouldBeActive = wheel.WheelIndex == wheelIndex;
            wheel.Activate(shouldBeActive);
        }
    }

    /// <summary>
    /// Выбрать новые колёса (через UI)
    /// </summary>
    public void SelectWheels(int wheelIndex)
    {
        SetCurrentWheels(wheelIndex);

        // Сохранить выбор через CarUpGradeHandler
        if (wheels != null && wheels.Length > 0)
        {
            var modApplier = wheels[0].ModApplier;
            if (modApplier != null)
            {
                modApplier.SetWheelIndex(wheelIndex);
                modApplier.SaveLoadout();
            }
        }
    }
}