using UnityEngine;

/// <summary>
/// Управляет установкой и видимостью конкретного спойлера на автомобиле.
/// </summary>
public class VehcileUpgradeSpoiler : MonoBehaviour
{
    [Header("Indicates what index этот спойлер занимает в списке")]
    public int SpoilerIndex = -1;

    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    /// <summary>
    /// Включить или выключить этот спойлер (активация объекта).
    /// </summary>
    public void Activate(bool state) => gameObject.SetActive(state);

    /// <summary>
    /// Вызвать установку этого спойлера на машину.
    /// </summary>
    public void SetAsCurrentSpoiler()
    {
        if (ModApplier != null)
        {
            ModApplier.SetSpoilerIndex(SpoilerIndex);
            ModApplier.SaveLoadout(); // Опционально, если сразу хотим сохранять выбор
        }
    }
}