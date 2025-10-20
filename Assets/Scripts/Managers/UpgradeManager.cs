using UnityEngine;

/// <summary>
/// Управляет всеми апгрейдами (двигатель, тормоза, нитро, управляемость) и их активным состоянием.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [Header("Апгрейды двигателя")]
    public VehicleUpgradeEngine[] engineUpgrades;

    [Header("Апгрейды тормозов")]
    public VehicleUpgradeBrake[] brakeUpgrades;

    [Header("Апгрейды нитро")]
    public VehicleUpgradeNitro[] nitroUpgrades;

    [Header("Апгрейды управляемости")]
    public VehicleUpgradeHandling[] handlingUpgrades;

    /// <summary>
    /// Установить нужные апгрейды по текущему лоад-ауту
    /// </summary>
    public void Initialize(int engineLevel, int brakeLevel, int nitroLevel, int handlingLevel)
    {
        SetCurrentEngine(engineLevel);
        SetCurrentBrake(brakeLevel);
        SetCurrentNitro(nitroLevel);
        SetCurrentHandling(handlingLevel);
    }

    #region Engine Management

    public void SetCurrentEngine(int level)
    {
        if (engineUpgrades == null || engineUpgrades.Length == 0)
            engineUpgrades = GetComponentsInChildren<VehicleUpgradeEngine>(true);

        foreach (var up in engineUpgrades)
            up.Activate(up.EngineLevel == level);
    }

    public void SelectEngineUpgrade(int level)
    {
        SetCurrentEngine(level);

        if (engineUpgrades != null && engineUpgrades.Length > 0)
        {
            var modApplier = engineUpgrades[0].ModApplier;
            if (modApplier != null)
            {
                modApplier.SetEngineLevel(level);
                modApplier.SaveLoadout();
            }
        }
    }

    #endregion

    #region Brake Management

    public void SetCurrentBrake(int level)
    {
        if (brakeUpgrades == null || brakeUpgrades.Length == 0)
            brakeUpgrades = GetComponentsInChildren<VehicleUpgradeBrake>(true);

        foreach (var up in brakeUpgrades)
            up.Activate(up.BrakeLevel == level);
    }

    public void SelectBrakeUpgrade(int level)
    {
        SetCurrentBrake(level);

        if (brakeUpgrades != null && brakeUpgrades.Length > 0)
        {
            var modApplier = brakeUpgrades[0].ModApplier;
            if (modApplier != null)
            {
                modApplier.SetBrakeLevel(level);
                modApplier.SaveLoadout();
            }
        }
    }

    #endregion

    #region Nitro Management

    public void SetCurrentNitro(int level)
    {
        if (nitroUpgrades == null || nitroUpgrades.Length == 0)
            nitroUpgrades = GetComponentsInChildren<VehicleUpgradeNitro>(true);

        foreach (var up in nitroUpgrades)
            up.Activate(up.NitroLevel == level);
    }

    public void SelectNitroUpgrade(int level)
    {
        SetCurrentNitro(level);

        if (nitroUpgrades != null && nitroUpgrades.Length > 0)
        {
            var modApplier = nitroUpgrades[0].ModApplier;
            if (modApplier != null)
            {
                modApplier.SetNitroLevel(level);
                modApplier.SaveLoadout();
            }
        }
    }

    #endregion

    #region Handling Management

    public void SetCurrentHandling(int level)
    {
        if (handlingUpgrades == null || handlingUpgrades.Length == 0)
            handlingUpgrades = GetComponentsInChildren<VehicleUpgradeHandling>(true);

        foreach (var up in handlingUpgrades)
            up.Activate(up.HandlingLevel == level);
    }

    public void SelectHandlingUpgrade(int level)
    {
        SetCurrentHandling(level);

        if (handlingUpgrades != null && handlingUpgrades.Length > 0)
        {
            var modApplier = handlingUpgrades[0].ModApplier;
            if (modApplier != null)
            {
                modApplier.SetHandlingLevel(level);
                modApplier.SaveLoadout();
            }
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Получить все доступные уровни для конкретного типа апгрейда
    /// </summary>
    public int[] GetAvailableLevels(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Engine:
                if (engineUpgrades == null) return new int[0];
                int[] engineLevels = new int[engineUpgrades.Length];
                for (int i = 0; i < engineUpgrades.Length; i++)
                    engineLevels[i] = engineUpgrades[i].EngineLevel;
                return engineLevels;

            case UpgradeType.Brake:
                if (brakeUpgrades == null) return new int[0];
                int[] brakeLevels = new int[brakeUpgrades.Length];
                for (int i = 0; i < brakeUpgrades.Length; i++)
                    brakeLevels[i] = brakeUpgrades[i].BrakeLevel;
                return brakeLevels;

            case UpgradeType.Nitro:
                if (nitroUpgrades == null) return new int[0];
                int[] nitroLevels = new int[nitroUpgrades.Length];
                for (int i = 0; i < nitroUpgrades.Length; i++)
                    nitroLevels[i] = nitroUpgrades[i].NitroLevel;
                return nitroLevels;

            case UpgradeType.Handling:
                if (handlingUpgrades == null) return new int[0];
                int[] handlingLevels = new int[handlingUpgrades.Length];
                for (int i = 0; i < handlingUpgrades.Length; i++)
                    handlingLevels[i] = handlingUpgrades[i].HandlingLevel;
                return handlingLevels;

            default:
                return new int[0];
        }
    }

    /// <summary>
    /// Получить максимальный уровень для типа апгрейда
    /// </summary>
    public int GetMaxLevel(UpgradeType upgradeType)
    {
        int[] levels = GetAvailableLevels(upgradeType);
        if (levels.Length == 0) return 0;
        
        int max = levels[0];
        foreach (int level in levels)
            if (level > max) max = level;
        
        return max;
    }

    #endregion
}

/// <summary>
/// Типы апгрейдов для удобства работы
/// </summary>
public enum UpgradeType
{
    Engine,
    Brake,
    Nitro,
    Handling
}