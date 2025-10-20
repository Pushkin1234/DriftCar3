# 🔄 Миграция с CustomizationModule на модульную систему

## 📋 Сравнение API

### ПОКРАСКА (Paint)

#### Было (CustomizationModule):
```csharp
CustomizationModule _customization;

// Покрасить машину
_customization.PaintCar(carName, color);

// Купить цвет
_customization.PurchaseColor(carName, colorIndex);

// Проверить разблокирован ли цвет
bool unlocked = _customization.IsColorUnlocked(carName, colorIndex);
```

#### Стало (PaintCustomizationModule):
```csharp
PaintCustomizationModule _paintModule;

// Покрасить машину
_paintModule.PaintCar(carIndex, color);

// Купить цвет
_paintModule.PurchaseColor(carIndex, colorIndex);

// Проверить разблокирован ли цвет
bool unlocked = _paintModule.IsColorUnlocked(carIndex, colorIndex);
```

**Изменения:**
- ✅ `carName` → `carIndex` (0-4 вместо строки)
- ✅ Отдельный модуль для покраски
- ✅ Те же методы, но в отдельном модуле

---

### КОЛЁСА (Wheels)

#### Было:
```csharp
// Сменить колёса
_customization.ChangeWheels(carName, wheelIndex);

// Купить колёса
_customization.PurchaseWheel(carName, wheelIndex);
```

#### Стало:
```csharp
WheelCustomizationModule _wheelModule;

// Сменить колёса
_wheelModule.ChangeWheels(carIndex, wheelIndex);

// Купить колёса (теперь для всех машин сразу!)
_wheelModule.PurchaseWheel(wheelIndex);
```

**Изменения:**
- ✅ `carName` → `carIndex`
- ✅ Отдельный модуль для колёс
- ⚠️ **Колёса покупаются для ВСЕХ машин**, а не для одной!

---

### УЛУЧШЕНИЯ (Performance)

#### Было:
```csharp
// Прокачать двигатель
_customization.UpgradeEngine(carName, level);

// Прокачать тормоза
_customization.UpgradeBrake(carName, level);

// Прокачать нитро
_customization.UpgradeNitro(carName, level);

// Прокачать управляемость
_customization.UpgradeHandling(carName, level);
```

#### Стало:
```csharp
PerformanceUpgradeModule _performanceModule;

// Купить улучшение двигателя
_performanceModule.PurchaseEngineUpgrade(carIndex, level);

// Купить улучшение тормозов
_performanceModule.PurchaseBrakeUpgrade(carIndex, level);

// Купить улучшение нитро
_performanceModule.PurchaseNitroUpgrade(carIndex, level);

// Купить улучшение управляемости
_performanceModule.PurchaseHandlingUpgrade(carIndex, level);
```

**Изменения:**
- ✅ `carName` → `carIndex`
- ✅ Отдельный модуль для улучшений
- ✅ `Upgrade` → `Purchase` (более явное название)

---

### СПОЙЛЕРЫ (Spoilers)

#### Было:
```csharp
// Установить спойлер
_customization.ChangeSpoiler(carName, spoilerIndex);

// Купить спойлер
_customization.PurchaseSpoiler(carName, spoilerIndex);
```

#### Стало:
```csharp
SpoilerCustomizationModule _spoilerModule;

// Установить спойлер
_spoilerModule.ChangeSpoiler(carIndex, spoilerIndex);

// Купить спойлер
_spoilerModule.PurchaseSpoiler(carIndex, spoilerIndex);
```

**Изменения:**
- ✅ `carName` → `carIndex`
- ✅ Отдельный модуль для спойлеров
- ✅ Те же методы

---

## 🔧 Пример миграции CustomizationController

### Было (старый код):

```csharp
public class CustomizationController : MonoBehaviour
{
    private CustomizationModule _customizationModule;
    private string _currentCarName = "Car1";
    
    private void Start()
    {
        _customizationModule = ModuleManager.Instance.GetModule<CustomizationModule>();
    }
    
    private void HandleColorPurchase(int colorIndex)
    {
        bool success = _customizationModule.PurchaseColor(_currentCarName, colorIndex);
        if (success)
        {
            var colorData = _customizationModule.GetColorData(colorIndex);
            _customizationModule.PaintCar(_currentCarName, colorData.color);
        }
    }
    
    private void HandleWheelPurchase(int wheelIndex)
    {
        bool success = _customizationModule.PurchaseWheel(_currentCarName, wheelIndex);
        if (success)
        {
            _customizationModule.ChangeWheels(_currentCarName, wheelIndex);
        }
    }
}
```

### Стало (новый код):

```csharp
public class CustomizationController : MonoBehaviour
{
    // 4 отдельных модуля
    private PaintCustomizationModule _paintModule;
    private WheelCustomizationModule _wheelModule;
    private PerformanceUpgradeModule _performanceModule;
    private SpoilerCustomizationModule _spoilerModule;
    
    private int _currentCarIndex = 0; // 0-4 вместо строки
    
    private void Start()
    {
        // Получаем 4 модуля
        _paintModule = ModuleManager.Instance.GetModule<PaintCustomizationModule>();
        _wheelModule = ModuleManager.Instance.GetModule<WheelCustomizationModule>();
        _performanceModule = ModuleManager.Instance.GetModule<PerformanceUpgradeModule>();
        _spoilerModule = ModuleManager.Instance.GetModule<SpoilerCustomizationModule>();
    }
    
    private void HandleColorPurchase(int colorIndex)
    {
        bool success = _paintModule.PurchaseColor(_currentCarIndex, colorIndex);
        if (success)
        {
            var colorData = _paintModule.GetColorData(colorIndex);
            _paintModule.PaintCar(_currentCarIndex, colorData.color);
        }
    }
    
    private void HandleWheelPurchase(int wheelIndex)
    {
        // Теперь колёса покупаются для всех машин
        bool success = _wheelModule.PurchaseWheel(wheelIndex);
        if (success)
        {
            // Но применяем к текущей машине
            _wheelModule.ChangeWheels(_currentCarIndex, wheelIndex);
        }
    }
}
```

---

## 📊 Таблица замены методов

| Старый метод | Новый метод | Модуль |
|--------------|-------------|--------|
| `PaintCar(carName, color)` | `PaintCar(carIndex, color)` | PaintCustomizationModule |
| `PurchaseColor(carName, colorIndex)` | `PurchaseColor(carIndex, colorIndex)` | PaintCustomizationModule |
| `IsColorUnlocked(carName, colorIndex)` | `IsColorUnlocked(carIndex, colorIndex)` | PaintCustomizationModule |
| `ChangeWheels(carName, wheelIndex)` | `ChangeWheels(carIndex, wheelIndex)` | WheelCustomizationModule |
| `PurchaseWheel(carName, wheelIndex)` | `PurchaseWheel(wheelIndex)` ⚠️ | WheelCustomizationModule |
| `IsWheelUnlocked(carName, wheelIndex)` | `IsWheelUnlocked(wheelIndex)` ⚠️ | WheelCustomizationModule |
| `UpgradeEngine(carName, level)` | `PurchaseEngineUpgrade(carIndex, level)` | PerformanceUpgradeModule |
| `UpgradeBrake(carName, level)` | `PurchaseBrakeUpgrade(carIndex, level)` | PerformanceUpgradeModule |
| `UpgradeNitro(carName, level)` | `PurchaseNitroUpgrade(carIndex, level)` | PerformanceUpgradeModule |
| `UpgradeHandling(carName, level)` | `PurchaseHandlingUpgrade(carIndex, level)` | PerformanceUpgradeModule |
| `ChangeSpoiler(carName, spoilerIndex)` | `ChangeSpoiler(carIndex, spoilerIndex)` | SpoilerCustomizationModule |
| `PurchaseSpoiler(carName, spoilerIndex)` | `PurchaseSpoiler(carIndex, spoilerIndex)` | SpoilerCustomizationModule |

⚠️ **Важно:** Методы для колёс теперь **НЕ принимают** `carIndex` при покупке/проверке разблокировки!

---

## 🔄 Конвертация carName → carIndex

### Было:
```csharp
string carName = "Car1";
```

### Стало:
```csharp
int carIndex = 0; // 0 = Car1, 1 = Car2, и т.д.
```

### Маппинг:

| Старое название | Новый индекс |
|----------------|--------------|
| "Car1" | 0 |
| "Car2" | 1 |
| "Car3" | 2 |
| "Car4" | 3 |
| "Car5" | 4 |

### Функция-помощник (если нужна обратная совместимость):

```csharp
private int ConvertCarNameToIndex(string carName)
{
    switch (carName)
    {
        case "Car1": return 0;
        case "Car2": return 1;
        case "Car3": return 2;
        case "Car4": return 3;
        case "Car5": return 4;
        default: return 0;
    }
}

private string ConvertCarIndexToName(int carIndex)
{
    return $"Car{carIndex + 1}";
}
```

---

## 📦 Чек-лист миграции

### 1. Подготовка:
- [ ] Создать резервную копию проекта
- [ ] Прочитать документацию модульной системы
- [ ] Понять изменения в API

### 2. Добавить модули:
- [ ] Добавить `PaintCustomizationModule` на GameController
- [ ] Добавить `WheelCustomizationModule` на GameController
- [ ] Добавить `PerformanceUpgradeModule` на GameController
- [ ] Добавить `SpoilerCustomizationModule` на GameController

### 3. Обновить CustomizationController:
- [ ] Заменить `CustomizationModule` на 4 новых модуля
- [ ] Изменить `string carName` на `int carIndex`
- [ ] Обновить все вызовы методов
- [ ] Обновить подписки на события

### 4. Обновить CustomizationView:
- [ ] Проверить события UI
- [ ] Обновить передачу параметров

### 5. Настроить в Inspector:
- [ ] Настроить `WheelCustomizationModule` (префабы, иконки)
- [ ] Настроить `SpoilerCustomizationModule` (если нужно)

### 6. Тестирование:
- [ ] Проверить покупку цветов
- [ ] Проверить смену колёс
- [ ] Проверить прокачку улучшений
- [ ] Проверить спойлеры
- [ ] Проверить сохранение/загрузку

### 7. Очистка:
- [ ] Удалить старый `CustomizationModule.cs` (опционально)
- [ ] Удалить неиспользуемый код

---

## ⚠️ Важные отличия

### 1. Колёса теперь общие для всех машин:

#### Было:
```csharp
// Покупали колёса отдельно для каждой машины
_customization.PurchaseWheel("Car1", 2); // Купили для Car1
_customization.PurchaseWheel("Car2", 2); // Нужно купить ещё раз для Car2
```

#### Стало:
```csharp
// Колёса покупаются один раз для всех машин
_wheelModule.PurchaseWheel(2); // Разблокировано для всех 5 машин!

// Но применяем к конкретной машине
_wheelModule.ChangeWheels(0, 2); // Применили к Car1
_wheelModule.ChangeWheels(1, 2); // Применили к Car2
```

### 2. Проверка разблокировки колёс:

#### Было:
```csharp
bool unlocked = _customization.IsWheelUnlocked("Car1", wheelIndex);
```

#### Стало:
```csharp
bool unlocked = _wheelModule.IsWheelUnlocked(wheelIndex); // БЕЗ carIndex!
```

---

## 💡 Примеры типичных сценариев

### Сценарий 1: Покупка и применение цвета

#### Было:
```csharp
private void BuyColor(string carName, int colorIndex)
{
    if (_customization.PurchaseColor(carName, colorIndex))
    {
        var colorData = _customization.GetColorData(colorIndex);
        _customization.PaintCar(carName, colorData.color);
    }
}
```

#### Стало:
```csharp
private void BuyColor(int carIndex, int colorIndex)
{
    if (_paintModule.PurchaseColor(carIndex, colorIndex))
    {
        var colorData = _paintModule.GetColorData(colorIndex);
        _paintModule.PaintCar(carIndex, colorData.color);
    }
}
```

### Сценарий 2: Полная прокачка машины

#### Было:
```csharp
private void FullyUpgrade(string carName)
{
    _customization.UpgradeEngine(carName, 4);
    _customization.UpgradeBrake(carName, 4);
    _customization.UpgradeNitro(carName, 4);
    _customization.UpgradeHandling(carName, 4);
}
```

#### Стало:
```csharp
private void FullyUpgrade(int carIndex)
{
    for (int level = 1; level <= 4; level++)
    {
        _performanceModule.PurchaseEngineUpgrade(carIndex, level);
        _performanceModule.PurchaseBrakeUpgrade(carIndex, level);
        _performanceModule.PurchaseNitroUpgrade(carIndex, level);
        _performanceModule.PurchaseHandlingUpgrade(carIndex, level);
    }
}
```

### Сценарий 3: Получение информации о машине

#### Было:
```csharp
private void ShowCarInfo(string carName)
{
    var customization = _customization.GetCarCustomization(carName);
    Debug.Log($"Цвет: {customization.paintColor}");
    Debug.Log($"Колёса: {customization.selectedWheelIndex}");
    Debug.Log($"Спойлер: {customization.selectedSpoilerIndex}");
}
```

#### Стало:
```csharp
private void ShowCarInfo(int carIndex)
{
    Color color = _paintModule.GetCurrentColor(carIndex);
    int wheelIndex = _wheelModule.GetCurrentWheelIndex(carIndex);
    int spoilerIndex = _spoilerModule.GetCurrentSpoilerIndex(carIndex);
    
    Debug.Log($"Цвет: {color}");
    Debug.Log($"Колёса: {wheelIndex}");
    Debug.Log($"Спойлер: {spoilerIndex}");
}
```

---

## ✅ Проверка после миграции

Запустите игру и убедитесь:

1. ✅ В консоли появились сообщения инициализации 4 модулей
2. ✅ Можно покупать и применять цвета
3. ✅ Можно покупать и менять колёса
4. ✅ Можно прокачивать характеристики
5. ✅ Можно покупать и менять спойлеры
6. ✅ Сохранение работает корректно
7. ✅ Загрузка восстанавливает все настройки

---

**Готово! Миграция завершена! 🎉**

