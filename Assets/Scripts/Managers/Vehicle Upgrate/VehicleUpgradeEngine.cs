using UnityEngine;

/// <summary>
/// Отвечает за применение апгрейда двигателя к автомобилю.
/// </summary>
public class VehicleUpgradeEngine : MonoBehaviour
{
    [Header("Level этого апгрейда двигателя (0 - базовый, 1 - ... , N - максимальный)")]
    public int EngineLevel = 0;

    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    /// <summary>
    /// Активирует/деактивирует этот апгрейд (например, визуально, если это отображаемо)
    /// </summary>
    public void Activate(bool state) => gameObject.SetActive(state);

    /// <summary>
    /// Сделать этот апгрейд двигателя текущим (применить и сохранить).
    /// </summary>
    public void SetAsCurrentEngine()
    {
        if (ModApplier != null)
        {
            ModApplier.SetEngineLevel(EngineLevel);
            ModApplier.SaveLoadout();
        }
    }
}