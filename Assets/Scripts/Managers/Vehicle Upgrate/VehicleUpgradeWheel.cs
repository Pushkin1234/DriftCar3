using UnityEngine;

/// <summary>
/// Управляет одним вариантом колёс на автомобиле.
/// </summary>
public class VehicleUpgradeWheel : MonoBehaviour
{
    [Header("Индекс этих колёс в списке")]
    public int WheelIndex = 0;

    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    /// <summary>
    /// Активировать/деактивировать эти колёса
    /// </summary>
    public void Activate(bool state) => gameObject.SetActive(state);

    /// <summary>
    /// Установить эти колёса как текущие
    /// </summary>
    public void SetAsCurrentWheels()
    {
        if (ModApplier != null)
        {
            ModApplier.SetWheelIndex(WheelIndex);
            ModApplier.SaveLoadout();
        }
    }
}