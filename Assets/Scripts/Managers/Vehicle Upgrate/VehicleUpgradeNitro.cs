using UnityEngine;

/// <summary>
/// Отвечает за применение апгрейда нитро к автомобилю.
/// </summary>
public class VehicleUpgradeNitro : MonoBehaviour
{
    [Header("Level этого апгрейда нитро (0 - базовый, 1 - ... , N - максимальный)")]
    public int NitroLevel = 0;

    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    /// <summary>
    /// Активирует/деактивирует этот апгрейд нитро (например, визуально, если это отображаемо)
    /// </summary>
    public void Activate(bool state) => gameObject.SetActive(state);

    /// <summary>
    /// Сделать этот апгрейд нитро текущим (применить и сохранить).
    /// </summary>
    public void SetAsCurrentNitro()
    {
        if (ModApplier != null)
        {
            ModApplier.SetNitroLevel(NitroLevel);
            ModApplier.SaveLoadout();
        }
    }
}