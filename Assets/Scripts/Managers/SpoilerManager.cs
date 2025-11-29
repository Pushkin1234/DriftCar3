using UnityEngine;

/// <summary>
/// Менеджер всех вариантов спойлеров автомобиля. 
/// Включает только тот спойлер, который выбран в CarUpGradeHandler.
/// </summary>
public class SpoilerManager : MonoBehaviour
{
    [Header("Дочерние объекты-спойлеры")]
    public VehcileUpgradeSpoiler[] spoilers;

    /// <summary>
    /// Инициализировать активацию спойлера по данным машины.
    /// </summary>
    public void Initialize(int selectedSpoilerIndex)
    {
        if (spoilers == null || spoilers.Length == 0)
            spoilers = GetComponentsInChildren<VehcileUpgradeSpoiler>(true);

        SetCurrentSpoiler(selectedSpoilerIndex);
    }
    
    /// <summary>
    /// Активировать один нужный спойлер, остальные скрыть.
    /// </summary>
    public void SetCurrentSpoiler(int index)
    {
        if (spoilers == null || spoilers.Length == 0)
            return;

        foreach (var sp in spoilers)
        {
            bool shouldBeActive = sp.SpoilerIndex == index;
            sp.Activate(shouldBeActive);
        }
    }

    /// <summary>
    /// Быстрая установка указанного спойлера как текущего (например, через UI).
    /// </summary>
    public void SelectSpoiler(int index)
    {
        SetCurrentSpoiler(index);

        // Сообщить CarUpGradeHandler, что выбран новый спойлер:
        if (spoilers != null && spoilers.Length > 0)
        {
            var modApplier = spoilers[0].ModApplier;
            if (modApplier != null)
            {
                modApplier.SetSpoilerIndex(index);
                modApplier.SaveLoadout(); // По аналогии с Paint
            }
        }
    }
}