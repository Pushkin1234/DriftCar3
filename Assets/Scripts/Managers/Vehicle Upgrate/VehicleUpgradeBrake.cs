using UnityEngine;

public class VehicleUpgradeBrake : MonoBehaviour
{
    public int BrakeLevel = 0;

    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    public void Activate(bool state) => gameObject.SetActive(state);

    public void SetAsCurrentBrake()
    {
        if (ModApplier != null)
        {
            ModApplier.SetBrakeLevel(BrakeLevel);
            ModApplier.SaveLoadout();
        }
    }
}