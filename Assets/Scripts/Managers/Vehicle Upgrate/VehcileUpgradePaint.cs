using UnityEngine;

/// <summary>
/// Скрипт для покраски частей автомобиля с поддержкой кастомизации.
/// Должен находиться на дочернем объекте машины и обращаться к CarUpGradeHandler родителя.
/// </summary>
public class VehcileUpgadrePaint : MonoBehaviour
{
    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    [Header("Target renderer/material index")]
    public MeshRenderer BodyRenderer;
    public int MaterialIndex = 0;

    /// <summary>
    /// Покрасить машину заданным цветом и сохранить изменения
    /// </summary>
    public void UpdatePaint(Color newColor)
    {
        if (!BodyRenderer)
        {
            Debug.LogError("[VehcileUpgratePaint] Body renderer is not set!");
            return;
        }
        if (BodyRenderer.materials.Length <= MaterialIndex)
        {
            Debug.LogError("[VehcileUpgratePaint] Material index out of range!");
            return;
        }

        BodyRenderer.materials[MaterialIndex].color = newColor;

        if (ModApplier != null)
        {
            ModApplier.SetPaintColor(newColor);
            ModApplier.SaveLoadout(); // или другой метод сохранения
        }
    }
}