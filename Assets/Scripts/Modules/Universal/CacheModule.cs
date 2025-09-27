using System.Collections.Generic;
using UnityEngine;

public class CacheModule : BaseGameModule, IPersistentModule
{
    public override string ModuleName => "Cache";
    
    private Dictionary<string, object> _cache = new Dictionary<string, object>();
    
    public T GetCached<T>(string key) where T : class
    {
        return _cache.ContainsKey(key) ? _cache[key] as T : null;
    }
    
    public void SetCached<T>(string key, T value)
    {
        _cache[key] = value;
    }
}