using UnityEngine;

public class VehicleUpgradeHandling : MonoBehaviour
{
    public int HandlingLevel = 0;

    private CarUpGradeHandler _modApplier;
    public CarUpGradeHandler ModApplier {
        get {
            if (_modApplier == null)
                _modApplier = GetComponentInParent<CarUpGradeHandler>();
            return _modApplier;
        }
    }

    public void Activate(bool state) => gameObject.SetActive(state);

    public void SetAsCurrentHandling()
    {
        if (ModApplier != null)
        {
            ModApplier.SetHandlingLevel(HandlingLevel);
            ModApplier.SaveLoadout();
        }
    }
}