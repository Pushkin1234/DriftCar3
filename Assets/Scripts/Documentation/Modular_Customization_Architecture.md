# 🏗️ Модульная архитектура кастомизации машин

## 📋 Обзор

Система кастомизации разделена на **4 независимых модуля**, каждый отвечает за свою часть:

```
┌──────────────────────────────────────────┐
│         CustomizationController          │
│      (координирует все модули)           │
└──────────────────┬───────────────────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
    ┌────▼────┐         ┌───▼────┐
    │ Paint   │         │ Wheel  │
    │ Module  │         │ Module │
    └─────────┘         └────────┘
         │                   │
    ┌────▼────┐         ┌───▼────┐
    │Performance│        │Spoiler │
    │ Module  │          │ Module │
    └─────────┘          └────────┘
```

---

## 🎯 Модули

### 1. **PaintCustomizationModule** - Покраска
**Ответственность:** ТОЛЬКО покраска машин

**Данные:**
- Доступные цвета (8 цветов)
- Текущий цвет машины
- Разблокированные цвета для каждой машины

**Методы:**
```csharp
int GetColorCount()
ColorData GetColorData(int colorIndex)
bool IsColorUnlocked(int carIndex, int colorIndex)
Color GetCurrentColor(int carIndex)
void PaintCar(int carIndex, Color color)
void SelectColor(int carIndex, int colorIndex)
bool PurchaseColor(int carIndex, int colorIndex)
void UnlockColor(int carIndex, int colorIndex)
```

**События:**
- `OnCarPainted(int carIndex, Color color)`
- `OnColorSelected(int carIndex, int colorIndex, Color color)`
- `OnColorPurchased(int carIndex, int colorIndex)`

**Сохранение:** `CarPaint_{carIndex}` в PlayerPrefs

---

### 2. **WheelCustomizationModule** - Колёса
**Ответственность:** ТОЛЬКО смена колёс

**Данные:**
- Доступные колёса (настраиваются в Inspector)
- Текущие колёса на машине
- Разблокированные колёса (общие для всех машин)

**Методы:**
```csharp
int GetWheelCount()
WheelData GetWheelData(int wheelIndex)
bool IsWheelUnlocked(int wheelIndex)
int GetCurrentWheelIndex(int carIndex)
void ChangeWheels(int carIndex, int wheelIndex)
bool PurchaseWheel(int wheelIndex)
void UnlockWheel(int wheelIndex)
```

**События:**
- `OnWheelsChanged(int carIndex, int wheelIndex)`
- `OnWheelsPurchased(int wheelIndex)`

**Сохранение:** `CarWheels_{carIndex}` в PlayerPrefs

**Особенность:** Колёса разблокируются для ВСЕХ машин сразу

---

### 3. **PerformanceUpgradeModule** - Улучшения
**Ответственность:** ТОЛЬКО прокачка характеристик

**Данные:**
- 4 типа улучшений по 5 уровней каждый:
  - Двигатель (Engine)
  - Тормоза (Brake)
  - Нитро (Nitro)
  - Управляемость (Handling)
- Текущие уровни для каждой машины
- Разблокированные уровни

**Методы:**
```csharp
// Информация
UpgradeData GetEngineUpgradeData(int level)
UpgradeData GetBrakeUpgradeData(int level)
UpgradeData GetNitroUpgradeData(int level)
UpgradeData GetHandlingUpgradeData(int level)

// Статус
bool IsEngineUpgradeUnlocked(int carIndex, int level)
bool IsBrakeUpgradeUnlocked(int carIndex, int level)
bool IsNitroUpgradeUnlocked(int carIndex, int level)
bool IsHandlingUpgradeUnlocked(int carIndex, int level)

// Текущие уровни
int GetEngineLevel(int carIndex)
int GetBrakeLevel(int carIndex)
int GetNitroLevel(int carIndex)
int GetHandlingLevel(int carIndex)

// Покупка
bool PurchaseEngineUpgrade(int carIndex, int level)
bool PurchaseBrakeUpgrade(int carIndex, int level)
bool PurchaseNitroUpgrade(int carIndex, int level)
bool PurchaseHandlingUpgrade(int carIndex, int level)
```

**События:**
- `OnEngineUpgraded(int carIndex, int level)`
- `OnBrakeUpgraded(int carIndex, int level)`
- `OnNitroUpgraded(int carIndex, int level)`
- `OnHandlingUpgraded(int carIndex, int level)`

**Сохранение:** `CarPerformance_{carIndex}` в PlayerPrefs

---

### 4. **SpoilerCustomizationModule** - Спойлеры
**Ответственность:** ТОЛЬКО спойлеры

**Данные:**
- Доступные спойлеры (6 типов + без спойлера)
- Текущий спойлер на машине
- Разблокированные спойлеры для каждой машины
- Бонус прижимной силы

**Методы:**
```csharp
int GetSpoilerCount()
SpoilerData GetSpoilerData(int spoilerIndex)
bool IsSpoilerUnlocked(int carIndex, int spoilerIndex)
int GetCurrentSpoilerIndex(int carIndex)
float GetCurrentDownforceBonus(int carIndex)
void ChangeSpoiler(int carIndex, int spoilerIndex)
bool PurchaseSpoiler(int carIndex, int spoilerIndex)
void UnlockSpoiler(int carIndex, int spoilerIndex)
```

**События:**
- `OnSpoilerChanged(int carIndex, int spoilerIndex)`
- `OnSpoilerPurchased(int carIndex, int spoilerIndex)`

**Сохранение:** `CarSpoiler_{carIndex}` в PlayerPrefs

---

## 🔧 Использование в CustomizationController

### Инициализация модулей:

```csharp
public class CustomizationController : MonoBehaviour
{
    // Ссылки на 4 модуля
    private PaintCustomizationModule _paintModule;
    private WheelCustomizationModule _wheelModule;
    private PerformanceUpgradeModule _performanceModule;
    private SpoilerCustomizationModule _spoilerModule;
    
    private void Start()
    {
        // Получаем модули из ModuleManager
        _paintModule = ModuleManager.Instance.GetModule<PaintCustomizationModule>();
        _wheelModule = ModuleManager.Instance.GetModule<WheelCustomizationModule>();
        _performanceModule = ModuleManager.Instance.GetModule<PerformanceUpgradeModule>();
        _spoilerModule = ModuleManager.Instance.GetModule<SpoilerCustomizationModule>();
        
        // Подписываемся на события View
        SubscribeToViewEvents();
        
        // Загружаем данные
        LoadCarCustomization();
    }
}
```

### Примеры обработки событий:

```csharp
// ПОКРАСКА
private void HandleColorPurchase()
{
    bool success = _paintModule.PurchaseColor(_currentCarIndex, _selectedColorIndex);
    
    if (success)
    {
        RefreshColorPanel();
    }
}

private void HandleColorSelect()
{
    var colorData = _paintModule.GetColorData(_selectedColorIndex);
    if (colorData != null)
    {
        _paintModule.PaintCar(_currentCarIndex, colorData.color);
    }
}

// КОЛЁСА
private void HandleWheelPurchase()
{
    bool success = _wheelModule.PurchaseWheel(_selectedWheelIndex);
    
    if (success)
    {
        RefreshWheelsPanel();
    }
}

private void HandleWheelSelect()
{
    _wheelModule.ChangeWheels(_currentCarIndex, _selectedWheelIndex);
}

// УЛУЧШЕНИЯ
private void HandleEngineSelection(int level)
{
    _selectedEngineLevel = level;
    RefreshUpgradePanel();
}

private void HandleUpgradePurchase()
{
    bool success = _performanceModule.PurchaseEngineUpgrade(
        _currentCarIndex, 
        _selectedEngineLevel
    );
    
    if (success)
    {
        RefreshUpgradePanel();
    }
}

// СПОЙЛЕРЫ
private void HandleSpoilerPurchase()
{
    bool success = _spoilerModule.PurchaseSpoiler(
        _currentCarIndex, 
        _selectedSpoilerIndex
    );
    
    if (success)
    {
        RefreshSpoilerPanel();
    }
}

private void HandleSpoilerSelect()
{
    _spoilerModule.ChangeSpoiler(_currentCarIndex, _selectedSpoilerIndex);
}
```

---

## 📊 Обновление UI

### Обновление панели цветов:

```csharp
private void RefreshColorPanel()
{
    // Обновляем все кнопки цветов
    for (int i = 0; i < _paintModule.GetColorCount(); i++)
    {
        bool isUnlocked = _paintModule.IsColorUnlocked(_currentCarIndex, i);
        _view.UpdateColorButtonState(i, isUnlocked);
    }
    
    // Обновляем информацию о выбранном цвете
    var colorData = _paintModule.GetColorData(_selectedColorIndex);
    if (colorData != null)
    {
        bool isUnlocked = _paintModule.IsColorUnlocked(_currentCarIndex, _selectedColorIndex);
        _view.UpdateColorUI(colorData.price, isUnlocked, colorData.color);
    }
}
```

### Обновление панели колёс:

```csharp
private void RefreshWheelsPanel()
{
    for (int i = 0; i < _wheelModule.GetWheelCount(); i++)
    {
        bool isUnlocked = _wheelModule.IsWheelUnlocked(i);
        _view.UpdateWheelButtonState(i, isUnlocked);
    }
    
    var wheelData = _wheelModule.GetWheelData(_selectedWheelIndex);
    if (wheelData != null)
    {
        bool isUnlocked = _wheelModule.IsWheelUnlocked(_selectedWheelIndex);
        _view.UpdateWheelUI(wheelData.price, isUnlocked);
    }
}
```

### Обновление панели улучшений:

```csharp
private void RefreshUpgradePanel()
{
    // Обновляем кнопки двигателя
    for (int i = 0; i < _performanceModule.GetEngineUpgradeCount(); i++)
    {
        bool isUnlocked = _performanceModule.IsEngineUpgradeUnlocked(_currentCarIndex, i);
        _view.UpdateEngineButtonState(i, isUnlocked);
    }
    
    // Аналогично для тормозов, нитро, управляемости
    // ...
    
    // Обновляем информацию о выбранном улучшении
    var upgradeData = _performanceModule.GetEngineUpgradeData(_selectedEngineLevel);
    if (upgradeData != null)
    {
        bool isUnlocked = _performanceModule.IsEngineUpgradeUnlocked(_currentCarIndex, _selectedEngineLevel);
        _view.UpdateUpgradeUI(upgradeData.price, isUnlocked);
    }
}
```

---

## 🎮 Настройка в Unity

### 1. На GameController:

```
GameController (GameObject)
├─ ModuleManager
├─ DataModule
├─ PaintCustomizationModule ← Добавить
├─ WheelCustomizationModule ← Добавить
├─ PerformanceUpgradeModule ← Добавить
└─ SpoilerCustomizationModule ← Добавить
```

### 2. Настройка модулей в Inspector:

**WheelCustomizationModule:**
- Available Wheels: Массив WheelData
  - Wheel Name: "Стандартные", "Спортивные", и т.д.
  - Wheel Prefab: Префаб колеса
  - Wheel Icon: Sprite иконки
  - Price: Цена
  - Is Unlocked: false (кроме первых)

**SpoilerCustomizationModule:**
- Available Spoilers: Массив SpoilerData
  - Spoiler Name: "Без спойлера", "Спортивный", и т.д.
  - Spoiler Prefab: Префаб спойлера
  - Spoiler Icon: Sprite иконки
  - Price: Цена
  - Downforce Bonus: Бонус прижимной силы
  - Is Unlocked: false (кроме "Без спойлера")

---

## 💾 Структура сохранения

### PlayerPrefs ключи для каждой машины (0-4):

| Модуль | Ключ | Данные |
|--------|------|--------|
| Paint | `CarPaint_{carIndex}` | Текущий цвет, разблокированные цвета |
| Wheel | `CarWheels_{carIndex}` | Выбранные колёса, разблокированные колёса |
| Performance | `CarPerformance_{carIndex}` | Уровни улучшений, разблокированные уровни |
| Spoiler | `CarSpoiler_{carIndex}` | Текущий спойлер, разблокированные спойлеры |

**Пример:**
```
CarPaint_0 → {"currentColor":{"r":1,"g":0,"b":0,"a":1},"unlockedColors":[true,false,true,...]}
CarWheels_2 → {"selectedWheelIndex":3,"unlockedWheels":[true,true,true,true,...]}
CarPerformance_4 → {"engineLevel":2,"brakeLevel":1,...}
```

---

## ✅ Преимущества модульной архитектуры

### 1. **Разделение ответственности**
- Каждый модуль отвечает только за свою часть
- Легко найти и исправить баги
- Код проще понять

### 2. **Независимость**
- Модули не зависят друг от друга
- Можно добавлять/убирать модули
- Легко тестировать отдельно

### 3. **Переиспользование**
- Модули можно использовать в других проектах
- Наследуются от BaseGameModule
- Стандартный интерфейс

### 4. **Масштабируемость**
- Легко добавить новые типы кастомизации
- Просто создать новый модуль
- Не нужно трогать существующие

### 5. **Оптимизация**
- Каждый модуль загружается отдельно
- Можно отключить ненужные модули
- Сохранение только изменённых данных

---

## 🔄 Миграция со старого CustomizationModule

### Было:
```csharp
CustomizationModule _customizationModule;

_customizationModule.PaintCar(carName, color);
_customizationModule.ChangeWheels(carName, wheelIndex);
_customizationModule.PurchaseEngineUpgrade(carName, level);
_customizationModule.ChangeSpoiler(carName, spoilerIndex);
```

### Стало:
```csharp
PaintCustomizationModule _paintModule;
WheelCustomizationModule _wheelModule;
PerformanceUpgradeModule _performanceModule;
SpoilerCustomizationModule _spoilerModule;

_paintModule.PaintCar(carIndex, color);
_wheelModule.ChangeWheels(carIndex, wheelIndex);
_performanceModule.PurchaseEngineUpgrade(carIndex, level);
_spoilerModule.ChangeSpoiler(carIndex, spoilerIndex);
```

**Изменения:**
- ✅ Разделено на 4 модуля
- ✅ Используется `carIndex` вместо `carName`
- ✅ Каждый модуль независим
- ✅ Улучшенная структура данных

---

## 📝 Примеры использования

### Пример 1: Покрасить машину после покупки цвета

```csharp
public void BuyAndApplyColor(int carIndex, int colorIndex)
{
    // 1. Покупаем цвет
    bool purchased = _paintModule.PurchaseColor(carIndex, colorIndex);
    
    if (purchased)
    {
        // 2. Получаем данные цвета
        var colorData = _paintModule.GetColorData(colorIndex);
        
        // 3. Красим машину
        _paintModule.PaintCar(carIndex, colorData.color);
        
        Debug.Log($"Машина {carIndex} покрашена в {colorData.colorName}");
    }
}
```

### Пример 2: Полная прокачка машины

```csharp
public void FullyUpgradeCar(int carIndex)
{
    // Прокачиваем все до максимума
    for (int level = 1; level < 5; level++)
    {
        _performanceModule.PurchaseEngineUpgrade(carIndex, level);
        _performanceModule.PurchaseBrakeUpgrade(carIndex, level);
        _performanceModule.PurchaseNitroUpgrade(carIndex, level);
        _performanceModule.PurchaseHandlingUpgrade(carIndex, level);
    }
    
    Debug.Log($"Машина {carIndex} полностью прокачана!");
}
```

### Пример 3: Проверка доступности улучшения

```csharp
public bool CanBuyUpgrade(int carIndex, int level)
{
    // Получаем данные улучшения
    var upgradeData = _performanceModule.GetEngineUpgradeData(level);
    
    // Проверяем условия
    bool notUnlocked = !_performanceModule.IsEngineUpgradeUnlocked(carIndex, level);
    bool hasEnoughCoins = _dataModule.Data.coins >= upgradeData.price;
    
    return notUnlocked && hasEnoughCoins;
}
```

---

**Готово! Теперь система кастомизации полностью модульная! 🚗✨**

