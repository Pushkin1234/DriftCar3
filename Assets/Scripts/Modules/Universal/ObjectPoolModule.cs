using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolModule : BaseGameModule
{
    public override string ModuleName => "ObjectPool";
    
    private Dictionary<System.Type, object> _pools = new Dictionary<System.Type, object>();
    
    public ObjectPool<T> GetPool<T>() where T : MonoBehaviour
    {
        if (!_pools.ContainsKey(typeof(T)))
            _pools[typeof(T)] = new ObjectPool<T>();
        return _pools[typeof(T)] as ObjectPool<T>;
    }
}

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _pool = new Queue<T>();
    private T _prefab;
    
    public T Get()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();
        return Object.Instantiate(_prefab);
    }
    
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}