using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance { get; private set; }
    
    private Dictionary<string, IGameModule> _modules = new Dictionary<string, IGameModule>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public T GetModule<T>() where T : class, IGameModule
    {
        return _modules.Values.OfType<T>().FirstOrDefault();
    }
    
    public void RegisterModule(IGameModule module)
    {
        _modules[module.ModuleName] = module;
    }
}