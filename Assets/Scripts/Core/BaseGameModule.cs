using UnityEngine;

public abstract class BaseGameModule : MonoBehaviour, IGameModule
{
    public virtual string ModuleName { get; protected set; }
    public bool IsInitialized { get; protected set; }
    
    public virtual void Initialize()
    {
        IsInitialized = true;
    }
    
    public virtual void Update() { }
    
    public virtual void Shutdown()
    {
        IsInitialized = false;
    }
}