using UnityEngine;

public class WebGLModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "WebGL";
    
    public override void Initialize()
    {
        OptimizeWebGLSettings();
        base.Initialize();
    }
    
    private void OptimizeWebGLSettings()
    {
        QualitySettings.SetQualityLevel(2);
        QualitySettings.shadowResolution = ShadowResolution.Low;
        QualitySettings.shadowDistance = 50f;
        QualitySettings.lodBias = 1.5f;
    }
}