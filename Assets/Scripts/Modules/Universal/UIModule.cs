using UnityEngine;

public class UIModule : BaseGameModule
{
    public override string ModuleName => "UI";
    
    private float _lastUpdateTime;
    private const float UPDATE_INTERVAL = 0.1f; // 10 FPS для UI
    
    public override void Update()
    {
        if (Time.time - _lastUpdateTime > UPDATE_INTERVAL)
        {
            UpdateUI();
            _lastUpdateTime = Time.time;
        }
    }
    
    private void UpdateUI() { /* Умные обновления UI */ }
}