using System;
using UnityEngine;

/// <summary>
/// Управляет всеми частями кузова для покраски на машине.
/// Является простым менеджером для VehcileUpgratePaint-модулей.
/// </summary>
public class PaintManager : MonoBehaviour
{
    public VehcileUpgadrePaint[] paints; // Все paint-модули, которые можно красить

    /// <summary>
    /// Инициализация — покрасить все части в цвет из лоад-аута машины.
    /// </summary>
    public void Initialize(Color? initialColor = null)
    {
        if (paints == null || paints.Length == 0)
            paints = GetComponentsInChildren<VehcileUpgadrePaint>();

        // Можно явно задать цвет (например, из save)
        if (initialColor.HasValue)
            Paint(initialColor.Value);
    }

    /// <summary>
    /// Покрасить все части сразу.
    /// </summary>
    public void Paint(Color color)
    {
        if (paints == null || paints.Length == 0) return;
        foreach (var paintModule in paints)
            paintModule.UpdatePaint(color);
    }
}