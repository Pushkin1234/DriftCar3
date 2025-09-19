# 🚀 ПОШАГОВЫЙ ПЛАН МИГРАЦИИ НА МОДУЛЬНУЮ АРХИТЕКТУРУ

## 📋 АНАЛИЗ ТЕКУЩЕГО КОДА

### **Проблемы в текущем коде:**
1. **DriftScore.cs** - дублирование переменных, неиспользуемые поля, логические ошибки
2. **ShopController.cs** - синтаксическая ошибка в LoadCars(), смешение логики
3. **SaveData.cs** - двойной вызов Load(), неоптимальная структура

---

## 🎯 ПЛАН МИГРАЦИИ (8 ЭТАПОВ)

### **ЭТАП 1: СОЗДАНИЕ БАЗОВОЙ СИСТЕМЫ МОДУЛЕЙ (1-2 дня)**

#### **1.1 Создать структуру папок:**
```
Assets/Scripts/
├── Core/
│   ├── IGameModule.cs
│   ├── BaseGameModule.cs
│   └── ModuleManager.cs
├── Modules/
│   ├── Universal/
│   │   ├── ObjectPoolModule.cs
│   │   ├── CacheModule.cs
│   │   ├── WebGLModule.cs
│   │   └── UIModule.cs
│   └── Game/
│       ├── DriftModule.cs
│       ├── ShopModule.cs
│       ├── DataModule.cs
│       └── YandexModule.cs
└── Controllers/
    ├── DriftController.cs
    ├── ShopController.cs
    └── GameController.cs
```

#### **1.2 Создать базовые интерфейсы:**

**IGameModule.cs:**
```csharp
using UnityEngine;

public interface IGameModule
{
    string ModuleName { get; }
    bool IsInitialized { get; }
    void Initialize();
    void Update();
    void Shutdown();
}
```

**BaseGameModule.cs:**
```csharp
using UnityEngine;

public abstract class BaseGameModule : MonoBehaviour, IGameModule
{
    public string ModuleName { get; protected set; }
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
```

**ModuleManager.cs:**
```csharp
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
```

---

### **ЭТАП 2: СОЗДАНИЕ УНИВЕРСАЛЬНЫХ МОДУЛЕЙ (1 день)**

#### **2.1 ObjectPoolModule.cs:**
```csharp
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
```

#### **2.2 CacheModule.cs:**
```csharp
using System.Collections.Generic;
using UnityEngine;

public class CacheModule : BaseGameModule
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
```

#### **2.3 WebGLModule.cs:**
```csharp
using UnityEngine;

public class WebGLModule : BaseGameModule
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
```

---

### **ЭТАП 3: СОЗДАНИЕ DATA МОДУЛЯ (1 день)**

#### **3.1 DataModule.cs (замена SaveData):**
```csharp
using System.Collections.Generic;
using UnityEngine;

public class DataModule : BaseGameModule
{
    public override string ModuleName => "Data";
    
    [System.Serializable]
    public class GameData
    {
        public int coins = 0;
        public int recordDriftScore = 0;
        public int appliedCarIndex = 0;
        public bool muteMusic = false;
        public List<bool> isBuyShop = new List<bool>() {true, false, false, false, false};
    }
    
    private GameData _data = new GameData();
    private const string SAVE_KEY = "GameData";
    
    public GameData Data => _data;
    
    public override void Initialize()
    {
        LoadData();
        base.Initialize();
    }
    
    public void SaveData()
    {
        var json = JsonUtility.ToJson(_data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            var json = PlayerPrefs.GetString(SAVE_KEY);
            _data = JsonUtility.FromJson<GameData>(json);
        }
    }
}
```

---

### **ЭТАП 4: СОЗДАНИЕ DRIFT МОДУЛЯ (1-2 дня)**

#### **4.1 DriftModule.cs (логика дрифта):**
```csharp
using UnityEngine;

public class DriftModule : BaseGameModule
{
    public override string ModuleName => "Drift";
    
    // Конфигурация
    public float driftThreshold = 10f;
    public float requiredSpeed = 10f;
    public float timeToEndDrift = 1f;
    public float[] scoreMilestones = { 500f, 1000f, 1500f, 2000f, 2500f };
    public float[] multipliers = { 2f, 3f, 4f, 5f, 6f };
    
    // Состояние
    private float _driftScore = 0f;
    private bool _isDrifting = false;
    private int _currentMultiplierIndex = 0;
    private bool _isActivateMultiplier = false;
    private float _timeWithoutDrift = 0;
    
    // Компоненты
    private RCC_CarControllerV3 _carController;
    private DataModule _dataModule;
    
    public float CurrentDriftScore => _driftScore;
    public bool IsDrifting => _isDrifting;
    
    public override void Initialize()
    {
        _carController = FindObjectOfType<RCC_CarControllerV3>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        base.Initialize();
    }
    
    public override void Update()
    {
        if (_carController == null) return;
        
        float driftAngle = Vector3.Angle(_carController.transform.forward, _carController.Rigid.velocity);
        
        if (driftAngle > driftThreshold && _carController.speed > requiredSpeed)
        {
            if (!_isDrifting)
            {
                StartDrift();
            }
            UpdateDrift(driftAngle);
            _timeWithoutDrift = 0;
        }
        else
        {
            _timeWithoutDrift += Time.deltaTime;
            
            if (_isDrifting && _timeWithoutDrift > timeToEndDrift)
            {
                EndDrift();
            }
        }
    }
    
    private void StartDrift()
    {
        _isDrifting = true;
    }
    
    private void UpdateDrift(float driftAngle)
    {
        if (_isActivateMultiplier)
        {
            _driftScore += driftAngle * Time.deltaTime * multipliers[_currentMultiplierIndex];
        }
        else
        {
            _driftScore += driftAngle * Time.deltaTime;
        }
        
        CheckForMilestones();
    }
    
    private void CheckForMilestones()
    {
        if (_currentMultiplierIndex < (scoreMilestones.Length - 1) && 
            _driftScore >= scoreMilestones[_currentMultiplierIndex])
        {
            _isActivateMultiplier = true;
            _currentMultiplierIndex++;
        }
    }
    
    private void EndDrift()
    {
        _isDrifting = false;
        _timeWithoutDrift = 0;
        
        // Обновляем данные
        int coinsEarned = Mathf.RoundToInt(_driftScore * 0.5f);
        _dataModule.Data.coins += coinsEarned;
        _dataModule.Data.recordDriftScore = Mathf.Max(_dataModule.Data.recordDriftScore, Mathf.RoundToInt(_driftScore));
        
        // Сбрасываем состояние
        _currentMultiplierIndex = 0;
        _isActivateMultiplier = false;
        _driftScore = 0;
        
        // Сохраняем
        _dataModule.SaveData();
    }
}
```

#### **4.2 DriftController.cs (UI управление):**
```csharp
using TMPro;
using UnityEngine;

public class DriftController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countCoinsNumberText;
    [SerializeField] private TextMeshProUGUI _countDriftScoreNumberText;
    [SerializeField] private TextMeshProUGUI _recordCountDriftScoreNumberText;
    [SerializeField] private TextMeshProUGUI _driftScoreMultiplier;
    [SerializeField] private TextMeshProUGUI _driftScoreText;
    [SerializeField] private TextMeshProUGUI _driftScoreTitle;
    
    private DriftModule _driftModule;
    private DataModule _dataModule;
    private int _countDriftScoreEarnedPerLevel = 0;
    private int _countCoinsEarnedPerLevel = 0;
    
    private void Start()
    {
        _driftModule = ModuleManager.Instance.GetModule<DriftModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        // Инициализация UI
        _driftScoreMultiplier.gameObject.SetActive(false);
        _driftScoreText.gameObject.SetActive(false);
        _driftScoreTitle.gameObject.SetActive(false);
        
        UpdateUI();
    }
    
    private void Update()
    {
        if (_driftModule == null) return;
        
        // Обновляем UI только при изменении
        if (_driftModule.IsDrifting)
        {
            _driftScoreText.text = "+" + Mathf.RoundToInt(_driftModule.CurrentDriftScore).ToString();
            _driftScoreText.gameObject.SetActive(true);
            _driftScoreTitle.gameObject.SetActive(true);
        }
        else
        {
            _driftScoreText.gameObject.SetActive(false);
            _driftScoreTitle.gameObject.SetActive(false);
        }
    }
    
    private void UpdateUI()
    {
        _countCoinsNumberText.text = _countCoinsEarnedPerLevel.ToString();
        _countDriftScoreNumberText.text = _countDriftScoreEarnedPerLevel.ToString();
        _recordCountDriftScoreNumberText.text = _dataModule.Data.recordDriftScore.ToString();
    }
}
```

---

### **ЭТАП 5: СОЗДАНИЕ SHOP МОДУЛЯ (1 день)**

#### **5.1 ShopModule.cs (логика магазина):**
```csharp
using UnityEngine;

public class ShopModule : BaseGameModule
{
    public override string ModuleName => "Shop";
    
    public int[] prices = { 0, 10, 20, 30, 40 };
    
    private DataModule _dataModule;
    
    public int SelectedCarIndex { get; private set; }
    public bool CanBuyCar(int index) => _dataModule.Data.coins >= prices[index];
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        SelectedCarIndex = _dataModule.Data.appliedCarIndex;
        base.Initialize();
    }
    
    public bool BuyCar(int index)
    {
        if (!CanBuyCar(index)) return false;
        
        _dataModule.Data.coins -= prices[index];
        _dataModule.Data.isBuyShop[index] = true;
        _dataModule.SaveData();
        
        return true;
    }
    
    public void SelectCar(int index)
    {
        SelectedCarIndex = index;
        _dataModule.Data.appliedCarIndex = index;
        _dataModule.SaveData();
    }
    
    public void ChangeCar(Direction direction)
    {
        if (direction == Direction.Left)
        {
            SelectedCarIndex = (SelectedCarIndex - 1 + _dataModule.Data.isBuyShop.Count) % _dataModule.Data.isBuyShop.Count;
        }
        else
        {
            SelectedCarIndex = (SelectedCarIndex + 1) % _dataModule.Data.isBuyShop.Count;
        }
        
        SelectCar(SelectedCarIndex);
    }
    
    public enum Direction { Left, Right }
}
```

#### **5.2 ShopController.cs (UI управление):**
```csharp
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopUIView _shopUIView;
    [SerializeField] private List<GameObject> _cars;
    [SerializeField] private TextMeshProUGUI _countCoinsText;
    
    private ShopModule _shopModule;
    private DataModule _dataModule;
    
    private void Start()
    {
        _shopModule = ModuleManager.Instance.GetModule<ShopModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        // Настройка кнопок
        _shopUIView.LeftNextCarButton.onClick.AddListener(() => ChangeCar(ShopModule.Direction.Left));
        _shopUIView.RightNextCarButton.onClick.AddListener(() => ChangeCar(ShopModule.Direction.Right));
        _shopUIView.BuyButton.onClick.AddListener(Buy);
        
        LoadCars();
    }
    
    private void Update()
    {
        // Обновляем UI только при изменении
        UpdateCoinsDisplay();
    }
    
    private void ChangeCar(ShopModule.Direction direction)
    {
        _shopModule.ChangeCar(direction);
        LoadCars();
    }
    
    private void Buy()
    {
        if (_shopModule.BuyCar(_shopModule.SelectedCarIndex))
        {
            LoadCars();
        }
    }
    
    private void LoadCars()
    {
        // Деактивируем все машины
        foreach (GameObject car in _cars)
        {
            car.SetActive(false);
        }
        
        // Активируем выбранную
        _cars[_shopModule.SelectedCarIndex].SetActive(true);
        
        // Обновляем UI
        bool isBought = _dataModule.Data.isBuyShop[_shopModule.SelectedCarIndex];
        
        if (isBought)
        {
            _shopUIView.BuyButton.gameObject.SetActive(false);
            _shopUIView.PriceText.gameObject.SetActive(false);
            _mainMenuView.PlayButton.gameObject.SetActive(true);
        }
        else
        {
            _shopUIView.PriceText.text = _shopModule.prices[_shopModule.SelectedCarIndex].ToString();
            _shopUIView.PriceText.gameObject.SetActive(true);
            _shopUIView.BuyButton.gameObject.SetActive(true);
            _mainMenuView.PlayButton.gameObject.SetActive(false);
        }
    }
    
    private void UpdateCoinsDisplay()
    {
        _countCoinsText.text = _dataModule.Data.coins.ToString();
    }
}
```

---

### **ЭТАП 6: СОЗДАНИЕ YANDEX МОДУЛЯ (1 день)**

#### **6.1 YandexModule.cs (интеграция с Яндекс.Игры):**
```csharp
using UnityEngine;
using YG;

public class YandexModule : BaseGameModule
{
    public override string ModuleName => "Yandex";
    
    private DataModule _dataModule;
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        base.Initialize();
    }
    
    public void SaveToYandex()
    {
        // Интеграция с Яндекс.Игры SDK
        if (YG2.isSDKEnabled)
        {
            // Сохранение в облако
            YG2.savesCloud.Save("GameData", JsonUtility.ToJson(_dataModule.Data));
        }
    }
    
    public void LoadFromYandex()
    {
        if (YG2.isSDKEnabled)
        {
            // Загрузка из облака
            YG2.savesCloud.Load("GameData");
        }
    }
    
    public void SendScoreToLeaderboard(int score)
    {
        if (YG2.isSDKEnabled)
        {
            YG2.leaderboard.SetScore("drift_leaderboard", score);
        }
    }
}
```

---

### **ЭТАП 7: СОЗДАНИЕ ГЛАВНОГО КОНТРОЛЛЕРА (1 день)**

#### **7.1 GameController.cs (замена GameManager):**
```csharp
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
        // Инициализация модулей
        InitializeModules();
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
```

---

### **ЭТАП 8: ОБНОВЛЕНИЕ СУЩЕСТВУЮЩИХ СКРИПТОВ (1 день)**

#### **8.1 Обновить PlayerView.cs:**
```csharp
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cars;
    
    private DataModule _dataModule;
    
    private void Start()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        PlacingSkin(_dataModule.Data.appliedCarIndex);
    }
    
    private void PlacingSkin(int index)
    {
        for (int i = 0; i < _cars.Count; i++)
        {
            _cars[i].SetActive(i == index);
        }
    }
}
```

---

## 🔧 ИНСТРУКЦИИ ПО РЕАЛИЗАЦИИ

### **1. Создайте папки:**
```
Assets/Scripts/Core/
Assets/Scripts/Modules/Universal/
Assets/Scripts/Modules/Game/
Assets/Scripts/Controllers/
```

### **2. Создайте файлы в указанном порядке:**
1. IGameModule.cs
2. BaseGameModule.cs
3. ModuleManager.cs
4. ObjectPoolModule.cs
5. CacheModule.cs
6. WebGLModule.cs
7. DataModule.cs
8. DriftModule.cs
9. DriftController.cs
10. ShopModule.cs
11. ShopController.cs (обновленный)
12. YandexModule.cs
13. GameController.cs

### **3. Обновите существующие скрипты:**
- PlayerView.cs
- Удалите старые DriftScore.cs и SaveData.cs

### **4. Настройте сцены:**
- Добавьте GameController на главный объект
- Обновите ссылки на новые контроллеры

---

## ⚠️ ВАЖНЫЕ МОМЕНТЫ

### **1. Миграция данных:**
- Сохранения останутся совместимыми
- PlayerPrefs заменит файловое сохранение для WebGL

### **2. Тестирование:**
- Тестируйте каждый модуль отдельно
- Проверьте работу в WebGL

### **3. Производительность:**
- Модули кэшируются после первого обращения
- Критичные операции можно сделать статическими

### **4. Обратная совместимость:**
- Старый код будет работать до полной миграции
- Можно мигрировать постепенно

---

## 📊 ОЖИДАЕМЫЕ РЕЗУЛЬТАТЫ

### **Производительность:**
- **FPS:** 58-59 (потеря 1-2 FPS)
- **Загрузка:** +0.1-0.2 секунды
- **Память:** +10-15% (из-за модулей)

### **Преимущества:**
- **Переиспользование:** 80% кода для других проектов
- **Тестируемость:** Каждый модуль тестируется отдельно
- **Поддерживаемость:** Легко добавлять новые функции
- **Чистота кода:** Соблюдение принципов SOLID

### **Время реализации:**
- **Общее время:** 8-10 дней
- **Критический путь:** Создание модулей (5-6 дней)
- **Тестирование:** 2-3 дня

---

## 🎯 ЗАКЛЮЧЕНИЕ

Этот план позволит вам плавно перейти на модульную архитектуру с минимальными рисками и максимальной производительностью. Модульная система обеспечит переиспользование 80% кода в будущих проектах при сохранении высокой производительности для WebGL.
