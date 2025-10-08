using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private bool needsDriftModule = true;
    [SerializeField] private bool needsShopModule = false;
    [SerializeField] private bool needsObjectPoolModule = true;
    [SerializeField] private bool needsUIModule = false;
    [SerializeField] private bool needsCustomizationModule = false;
    
    private void Awake()
    {
        // Сначала создаем ModuleManager если его нет
        EnsureModuleManagerExists();
        
        // Очищаем локальные модули от предыдущей сцены
        CleanupLocalModules();
        
        // Инициализируем модули для этой сцены
        InitializeModules();
    }
    
    private void EnsureModuleManagerExists()
    {
        if (ModuleManager.Instance == null)
        {
            // Создаем ModuleManager на отдельном объекте
            GameObject moduleManagerGO = new GameObject("ModuleManager");
            moduleManagerGO.AddComponent<ModuleManager>();
        }
    }
    
    private void CleanupLocalModules()
    {
        // Удаляем локальные модули, которые должны пересоздаваться
        ModuleManager.Instance.CleanupLocalModules();
    }
    
    private void InitializeModules()
    {
        // Инициализируем глобальные модули (создаются только один раз)
        InitializeGlobalModules();
        
        // Создаем локальные модули для этой сцены
        InitializeLocalModules();
    }
    
    private void InitializeGlobalModules()
    {
        // DataModule - глобальный (сохраняет прогресс игрока)
        if (!ModuleManager.Instance.HasModule<DataModule>())
        {
            var dataModule = ModuleManager.Instance.gameObject.AddComponent<DataModule>();
            ModuleManager.Instance.RegisterModule(dataModule);
            dataModule.Initialize();
            Debug.Log("Created DataModule (Global)");
        }
        
        // YandexModule - глобальный (сессия пользователя)
        if (!ModuleManager.Instance.HasModule<YandexModule>())
        {
            var yandexModule = ModuleManager.Instance.gameObject.AddComponent<YandexModule>();
            ModuleManager.Instance.RegisterModule(yandexModule);
            yandexModule.Initialize();
            Debug.Log("Created YandexModule (Global)");
        }
        
        // CacheModule - глобальный (кэш данных)
        if (!ModuleManager.Instance.HasModule<CacheModule>())
        {
            var cacheModule = ModuleManager.Instance.gameObject.AddComponent<CacheModule>();
            ModuleManager.Instance.RegisterModule(cacheModule);
            cacheModule.Initialize();
            Debug.Log("Created CacheModule (Global)");
        }
        
        // WebGLModule - глобальный (настройки платформы)
        if (!ModuleManager.Instance.HasModule<WebGLModule>())
        {
            var webglModule = ModuleManager.Instance.gameObject.AddComponent<WebGLModule>();
            ModuleManager.Instance.RegisterModule(webglModule);
            webglModule.Initialize();
            Debug.Log("Created WebGLModule (Global)");
        }
        
        // CustomizationModule - глобальный (настройки кастомизации машин)
        if (needsCustomizationModule && !ModuleManager.Instance.HasModule<CustomizationModule>())
        {
            var customizationModule = ModuleManager.Instance.gameObject.AddComponent<CustomizationModule>();
            ModuleManager.Instance.RegisterModule(customizationModule);
            customizationModule.Initialize();
            Debug.Log("Created CustomizationModule (Global)");
        }
        
        // UpgradeModule - глобальный (прокачка характеристик машин)
        if (!ModuleManager.Instance.HasModule<UpgradeModule>())
        {
            var upgradeModule = ModuleManager.Instance.gameObject.AddComponent<UpgradeModule>();
            ModuleManager.Instance.RegisterModule(upgradeModule);
            upgradeModule.Initialize();
            Debug.Log("Created UpgradeModule (Global)");
        }
    }
    
    private void InitializeLocalModules()
    {
        // DriftModule - локальный (состояние дрифта текущего уровня)
        if (needsDriftModule)
        {
            var driftModule = gameObject.AddComponent<DriftModule>();
            ModuleManager.Instance.RegisterModule(driftModule);
            driftModule.Initialize();
            Debug.Log("Created DriftModule (Local)");
        }
        
        // ShopModule - локальный (состояние UI магазина)
        if (needsShopModule)
        {
            var shopModule = gameObject.AddComponent<ShopModule>();
            ModuleManager.Instance.RegisterModule(shopModule);
            shopModule.Initialize();
            Debug.Log("Created ShopModule (Local)");
        }
        
        // ObjectPoolModule - локальный (пул объектов уровня)
        if (needsObjectPoolModule)
        {
            var objectPoolModule = gameObject.AddComponent<ObjectPoolModule>();
            ModuleManager.Instance.RegisterModule(objectPoolModule);
            objectPoolModule.Initialize();
            Debug.Log("Created ObjectPoolModule (Local)");
        }
    }
    
    private void OnDestroy()
    {
        // Дополнительная очистка при уничтожении GameController
        Debug.Log($"GameController destroyed in scene: {gameObject.scene.name}");
    }
}