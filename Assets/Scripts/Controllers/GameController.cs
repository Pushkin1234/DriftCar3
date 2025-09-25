using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
        // Сначала создаем ModuleManager если его нет
        EnsureModuleManagerExists();
        
        // Затем инициализируем модули
        InitializeModules();
    }
    
    private void EnsureModuleManagerExists()
    {
        if (ModuleManager.Instance == null)
        {
            // Создаем ModuleManager на этом же объекте
            gameObject.AddComponent<ModuleManager>();
        }
    }
    
    private void InitializeModules()
    {
        // Создаем универсальные модули
        var objectPoolModule = gameObject.AddComponent<ObjectPoolModule>();
        var cacheModule = gameObject.AddComponent<CacheModule>();
        var webglModule = gameObject.AddComponent<WebGLModule>();
        
        // Создаем игровые модули
        var dataModule = gameObject.AddComponent<DataModule>();
        var driftModule = gameObject.AddComponent<DriftModule>();
        var shopModule = gameObject.AddComponent<ShopModule>();
        var yandexModule = gameObject.AddComponent<YandexModule>();
        
        // Регистрируем модули
        ModuleManager.Instance.RegisterModule(objectPoolModule);
        ModuleManager.Instance.RegisterModule(cacheModule);
        ModuleManager.Instance.RegisterModule(webglModule);
        ModuleManager.Instance.RegisterModule(dataModule);
        ModuleManager.Instance.RegisterModule(driftModule);
        ModuleManager.Instance.RegisterModule(shopModule);
        ModuleManager.Instance.RegisterModule(yandexModule);
        
        // Инициализируем модули
        objectPoolModule.Initialize();
        cacheModule.Initialize();
        webglModule.Initialize();
        dataModule.Initialize();
        driftModule.Initialize();
        shopModule.Initialize();
        yandexModule.Initialize();
    }
}