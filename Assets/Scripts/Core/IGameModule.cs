using UnityEngine;

public interface IGameModule
{
    string ModuleName { get; }
    bool IsInitialized { get; }
    void Initialize();
    void Update();
    void Shutdown();
}