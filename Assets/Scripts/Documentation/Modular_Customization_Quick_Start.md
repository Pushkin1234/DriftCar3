# ⚡ Быстрый старт - Модульная кастомизация

## 🚀 Установка

### Шаг 1: Добавить модули на GameController

```
GameController (GameObject)
├─ ModuleManager
├─ DataModule
├─ PaintCustomizationModule      ← Add Component
├─ WheelCustomizationModule       ← Add Component  
├─ PerformanceUpgradeModule       ← Add Component
└─ SpoilerCustomizationModule     ← Add Component
```

### Шаг 2: Настроить модули

**WheelCustomizationModule:**
- Создайте массив Available Wheels в Inspector
- Назначьте префабы колёс, иконки, цены

**SpoilerCustomizationModule:**
- Создайте массив Available Spoilers
- Назначьте префабы спойлеров, иконки, цены

---

## 💻 Использование в коде

### Получение модулей:

```csharp
// В CustomizationController
private PaintCustomizationModule _paintModule;
private WheelCustomizationModule _wheelModule;
private PerformanceUpgradeModule _performanceModule;
private SpoilerCustomizationModule _spoilerModule;

private void Start()
{
    _paintModule = ModuleManager.Instance.GetModule<PaintCustomizationModule>();
    _wheelModule = ModuleManager.Instance.GetModule<WheelCustomizationModule>();
    _performanceModule = ModuleManager.Instance.GetModule<PerformanceUpgradeModule>();
    _spoilerModule = ModuleManager.Instance.GetModule<SpoilerCustomizationModule>();
}
```

---

## 🎨 1. Покраска (Paint Module)

### Основные методы:

```csharp
// Получить количество цветов
int count = _paintModule.GetColorCount(); // 8

// Получить данные о цвете
var colorData = _paintModule.GetColorData(colorIndex);
// colorData.color, colorData.colorName, colorData.price

// Проверить разблокирован ли цвет
bool unlocked = _paintModule.IsColorUnlocked(carIndex, colorIndex);

// Покрасить машину
_paintModule.PaintCar(carIndex, Color.red);

// Купить цвет
bool success = _paintModule.PurchaseColor(carIndex, colorIndex);
```

### Пример: Купить и применить цвет

```csharp
private void BuyColor(int carIndex, int colorIndex)
{
    bool purchased = _paintModule.PurchaseColor(carIndex, colorIndex);
    
    if (purchased)
    {
        var colorData = _paintModule.GetColorData(colorIndex);
        _paintModule.PaintCar(carIndex, colorData.color);
    }
}
```

---

## 🛞 2. Колёса (Wheel Module)

### Основные методы:

```csharp
// Получить количество колёс
int count = _wheelModule.GetWheelCount();

// Получить данные о колёсах
var wheelData = _wheelModule.GetWheelData(wheelIndex);
// wheelData.wheelName, wheelData.wheelPrefab, wheelData.price

// Проверить разблокированы ли (общие для всех машин!)
bool unlocked = _wheelModule.IsWheelUnlocked(wheelIndex);

// Сменить колёса
_wheelModule.ChangeWheels(carIndex, wheelIndex);

// Купить колёса (для всех машин)
bool success = _wheelModule.PurchaseWheel(wheelIndex);
```

### Пример: Купить и установить колёса

```csharp
private void BuyWheels(int carIndex, int wheelIndex)
{
    bool purchased = _wheelModule.PurchaseWheel(wheelIndex);
    
    if (purchased)
    {
        // Применяем к текущей машине
        _wheelModule.ChangeWheels(carIndex, wheelIndex);
    }
}
```

---

## ⚡ 3. Улучшения (Performance Module)

### Основные методы:

```csharp
// ДВИГАТЕЛЬ
int levels = _performanceModule.GetEngineUpgradeCount(); // 5
var upgrade = _performanceModule.GetEngineUpgradeData(level);
bool unlocked = _performanceModule.IsEngineUpgradeUnlocked(carIndex, level);
int currentLevel = _performanceModule.GetEngineLevel(carIndex);
bool success = _performanceModule.PurchaseEngineUpgrade(carIndex, level);

// ТОРМОЗА
var upgrade = _performanceModule.GetBrakeUpgradeData(level);
bool unlocked = _performanceModule.IsBrakeUpgradeUnlocked(carIndex, level);
int currentLevel = _performanceModule.GetBrakeLevel(carIndex);
bool success = _performanceModule.PurchaseBrakeUpgrade(carIndex, level);

// НИТРО
var upgrade = _performanceModule.GetNitroUpgradeData(level);
bool unlocked = _performanceModule.IsNitroUpgradeUnlocked(carIndex, level);
int currentLevel = _performanceModule.GetNitroLevel(carIndex);
bool success = _performanceModule.PurchaseNitroUpgrade(carIndex, level);

// УПРАВЛЯЕМОСТЬ
var upgrade = _performanceModule.GetHandlingUpgradeData(level);
bool unlocked = _performanceModule.IsHandlingUpgradeUnlocked(carIndex, level);
int currentLevel = _performanceModule.GetHandlingLevel(carIndex);
bool success = _performanceModule.PurchaseHandlingUpgrade(carIndex, level);
```

### Пример: Прокачать двигатель

```csharp
private void UpgradeEngine(int carIndex, int level)
{
    var upgradeData = _performanceModule.GetEngineUpgradeData(level);
    
    Debug.Log($"Покупаем {upgradeData.upgradeName} за {upgradeData.price}");
    
    bool purchased = _performanceModule.PurchaseEngineUpgrade(carIndex, level);
    
    if (purchased)
    {
        Debug.Log($"Мощность: x{upgradeData.powerMultiplier}");
    }
}
```

---

## 🏎️ 4. Спойлеры (Spoiler Module)

### Основные методы:

```csharp
// Получить количество спойлеров
int count = _spoilerModule.GetSpoilerCount(); // 6

// Получить данные о спойлере
var spoilerData = _spoilerModule.GetSpoilerData(spoilerIndex);
// spoilerData.spoilerName, spoilerData.spoilerPrefab, spoilerData.price, spoilerData.downforceBonus

// Проверить разблокирован ли
bool unlocked = _spoilerModule.IsSpoilerUnlocked(carIndex, spoilerIndex);

// Получить текущий спойлер
int current = _spoilerModule.GetCurrentSpoilerIndex(carIndex); // -1 = без спойлера

// Получить бонус прижимной силы
float bonus = _spoilerModule.GetCurrentDownforceBonus(carIndex);

// Установить спойлер (-1 = убрать)
_spoilerModule.ChangeSpoiler(carIndex, spoilerIndex);

// Купить спойлер
bool success = _spoilerModule.PurchaseSpoiler(carIndex, spoilerIndex);
```

### Пример: Купить и установить спойлер

```csharp
private void BuySpoiler(int carIndex, int spoilerIndex)
{
    bool purchased = _spoilerModule.PurchaseSpoiler(carIndex, spoilerIndex);
    
    if (purchased)
    {
        _spoilerModule.ChangeSpoiler(carIndex, spoilerIndex);
        
        var spoilerData = _spoilerModule.GetSpoilerData(spoilerIndex);
        Debug.Log($"Прижимная сила: +{spoilerData.downforceBonus}");
    }
}
```

---

## 📊 Сравнительная таблица

| Модуль | Купить | Применить | Разблокировка |
|--------|--------|-----------|---------------|
| Paint | `PurchaseColor(car, color)` | `PaintCar(car, color)` | Для каждой машины |
| Wheel | `PurchaseWheel(wheel)` | `ChangeWheels(car, wheel)` | Для ВСЕХ машин |
| Performance | `PurchaseEngineUpgrade(car, lvl)` | Автоматически | Для каждой машины |
| Spoiler | `PurchaseSpoiler(car, spoiler)` | `ChangeSpoiler(car, spoiler)` | Для каждой машины |

---

## 🎮 События модулей

### Подписка на события:

```csharp
private void Start()
{
    // Paint Module
    _paintModule.OnCarPainted += (carIndex, color) => 
    {
        Debug.Log($"Машина {carIndex} покрашена в {color}");
    };
    
    _paintModule.OnColorPurchased += (carIndex, colorIndex) =>
    {
        Debug.Log($"Куплен цвет {colorIndex} для машины {carIndex}");
    };
    
    // Wheel Module
    _wheelModule.OnWheelsChanged += (carIndex, wheelIndex) =>
    {
        Debug.Log($"Колёса машины {carIndex} изменены на {wheelIndex}");
    };
    
    // Performance Module
    _performanceModule.OnEngineUpgraded += (carIndex, level) =>
    {
        Debug.Log($"Двигатель машины {carIndex} улучшен до уровня {level}");
    };
    
    // Spoiler Module
    _spoilerModule.OnSpoilerChanged += (carIndex, spoilerIndex) =>
    {
        Debug.Log($"Спойлер машины {carIndex} изменён на {spoilerIndex}");
    };
}
```

---

## 💡 Полезные паттерны

### Паттерн 1: Получить всю информацию о машине

```csharp
public void ShowCarInfo(int carIndex)
{
    Debug.Log($"=== Машина {carIndex} ===");
    
    // Цвет
    Color color = _paintModule.GetCurrentColor(carIndex);
    Debug.Log($"Цвет: {color}");
    
    // Колёса
    int wheelIndex = _wheelModule.GetCurrentWheelIndex(carIndex);
    var wheelData = _wheelModule.GetWheelData(wheelIndex);
    Debug.Log($"Колёса: {wheelData.wheelName}");
    
    // Улучшения
    int engineLevel = _performanceModule.GetEngineLevel(carIndex);
    int brakeLevel = _performanceModule.GetBrakeLevel(carIndex);
    Debug.Log($"Двигатель: {engineLevel}, Тормоза: {brakeLevel}");
    
    // Спойлер
    int spoilerIndex = _spoilerModule.GetCurrentSpoilerIndex(carIndex);
    if (spoilerIndex >= 0)
    {
        var spoilerData = _spoilerModule.GetSpoilerData(spoilerIndex);
        Debug.Log($"Спойлер: {spoilerData.spoilerName}");
    }
}
```

### Паттерн 2: Сбросить всю кастомизацию

```csharp
public void ResetCarCustomization(int carIndex)
{
    // Сбрасываем покраску (белый цвет)
    _paintModule.PaintCar(carIndex, Color.white);
    
    // Стандартные колёса
    _wheelModule.ChangeWheels(carIndex, 0);
    
    // Убираем спойлер
    _spoilerModule.ChangeSpoiler(carIndex, -1);
    
    // Сбрасываем данные улучшений
    _performanceModule.ResetCarPerformanceData(carIndex);
}
```

### Паттерн 3: Проверка перед покупкой

```csharp
public bool CanPurchase(int price)
{
    var dataModule = ModuleManager.Instance.GetModule<DataModule>();
    return dataModule.Data.coins >= price;
}

public void TryPurchaseColor(int carIndex, int colorIndex)
{
    var colorData = _paintModule.GetColorData(colorIndex);
    
    if (!CanPurchase(colorData.price))
    {
        Debug.Log("Недостаточно монет!");
        return;
    }
    
    bool success = _paintModule.PurchaseColor(carIndex, colorIndex);
    
    if (success)
    {
        _paintModule.PaintCar(carIndex, colorData.color);
    }
}
```

---

## ⚙️ Настройка данных в Inspector

### PaintCustomizationModule
✅ Уже настроен, данные в коде (8 цветов)

### WheelCustomizationModule
```
Available Wheels (Size: 5)
├─ Element 0:
│   ├─ Wheel Name: "Стандартные"
│   ├─ Wheel Prefab: [Префаб]
│   ├─ Wheel Icon: [Sprite]
│   ├─ Price: 0
│   └─ Is Unlocked: ✓
├─ Element 1:
│   ├─ Wheel Name: "Спортивные"
│   ├─ Price: 1000
│   └─ Is Unlocked: ☐
...
```

### PerformanceUpgradeModule
✅ Уже настроен, данные в коде (4 типа по 5 уровней)

### SpoilerCustomizationModule
✅ Уже настроен, данные в коде (6 спойлеров)

Можно переопределить в Inspector если нужно!

---

## 🐛 Отладка

### Проверка модулей:

```csharp
private void CheckModules()
{
    if (_paintModule == null)
        Debug.LogError("PaintModule not found!");
    else
        Debug.Log($"PaintModule: {_paintModule.GetColorCount()} colors");
    
    if (_wheelModule == null)
        Debug.LogError("WheelModule not found!");
    else
        Debug.Log($"WheelModule: {_wheelModule.GetWheelCount()} wheels");
    
    if (_performanceModule == null)
        Debug.LogError("PerformanceModule not found!");
    else
        Debug.Log($"PerformanceModule: OK");
    
    if (_spoilerModule == null)
        Debug.LogError("SpoilerModule not found!");
    else
        Debug.Log($"SpoilerModule: {_spoilerModule.GetSpoilerCount()} spoilers");
}
```

---

**Готово! Теперь вы можете использовать модульную систему кастомизации! 🚗💨**

