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
    
    /// <summary>
    /// Проверяет, существует ли модуль определенного типа
    /// </summary>
    public bool HasModule<T>() where T : class, IGameModule
    {
        return GetModule<T>() != null;
    }
    
    /// <summary>
    /// Проверяет, является ли модуль персистентным (глобальным)
    /// </summary>
    public bool IsModulePersistent<T>() where T : class, IGameModule
    {
        var module = GetModule<T>();
        return module is IPersistentModule;
    }
    
    /// <summary>
    /// Удаляет локальные (непостоянные) модули при смене сцены
    /// </summary>
    public void CleanupLocalModules()
    {
        var localModules = _modules.Values.Where(m => !(m is IPersistentModule)).ToList();
        
        foreach (var module in localModules)
        {
            _modules.Remove(module.ModuleName);
            
            // Уничтожаем компонент если это MonoBehaviour
            if (module is MonoBehaviour monoBehaviour)
            {
                Destroy(monoBehaviour);
            }
        }
    }
    
    /// <summary>
    /// Получает все зарегистрированные модули (для отладки)
    /// </summary>
    public Dictionary<string, IGameModule> GetAllModules()
    {
        return new Dictionary<string, IGameModule>(_modules);
    }
}