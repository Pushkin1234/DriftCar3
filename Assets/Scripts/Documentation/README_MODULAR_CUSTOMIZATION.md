# 🚗 Модульная система кастомизации машин

## 📖 Содержание

1. [Обзор](#обзор)
2. [Модули](#модули)
3. [Быстрый старт](#быстрый-старт)
4. [Документация](#документация)
5. [Примеры использования](#примеры-использования)

---

## 🎯 Обзор

Система кастомизации разделена на **4 независимых модуля**, каждый отвечает за свою часть:

```
┌─────────────────────────────────────────┐
│      CustomizationController            │
│     (координирует все модули)           │
└──────────────┬──────────────────────────┘
               │
       ┌───────┴───────┐
       │               │
  ┌────▼────┐     ┌───▼────┐
  │  Paint  │     │ Wheel  │
  │ Module  │     │ Module │
  └─────────┘     └────────┘
       │               │
  ┌────▼────┐     ┌───▼────┐
  │Performance│    │Spoiler │
  │ Module  │     │ Module │
  └─────────┘     └────────┘
```

---

## 📦 Модули

| # | Модуль | Ответственность | События |
|---|--------|----------------|---------|
| 1️⃣ | **PaintCustomizationModule** | Покраска машин (8 цветов) | `OnCarPainted`, `OnColorPurchased`, `OnColorSelected` |
| 2️⃣ | **WheelCustomizationModule** | Смена колёс | `OnWheelsChanged`, `OnWheelsPurchased` |
| 3️⃣ | **PerformanceUpgradeModule** | Улучшения (двигатель, тормоза, нитро, управляемость) | `OnEngineUpgraded`, `OnBrakeUpgraded`, и т.д. |
| 4️⃣ | **SpoilerCustomizationModule** | Спойлеры | `OnSpoilerChanged`, `OnSpoilerPurchased` |

---

## 🚀 Быстрый старт

### Шаг 1: Добавить модули в Unity

На `GameController` GameObject добавить 4 компонента:
1. `PaintCustomizationModule`
2. `WheelCustomizationModule`
3. `PerformanceUpgradeModule`
4. `SpoilerCustomizationModule`

### Шаг 2: Настроить в Inspector

**WheelCustomizationModule:**
- Available Wheels → настроить префабы, иконки, цены

**SpoilerCustomizationModule:**
- Available Spoilers → уже настроены (можно переопределить)

### Шаг 3: Использовать в коде

```csharp
public class CustomizationController : MonoBehaviour
{
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
}
```

---

## 📚 Документация

### 📄 Основные файлы:

| Файл | Описание |
|------|----------|
| **README_MODULAR_CUSTOMIZATION.md** | Этот файл - обзор системы |
| **Modular_Customization_Architecture.md** | Полная документация по архитектуре |
| **Modular_Customization_Quick_Start.md** | Быстрый старт и шпаргалка |
| **MODULAR_CUSTOMIZATION_SUMMARY.md** | Краткое резюме |
| **MIGRATION_FROM_OLD_CUSTOMIZATION.md** | Гайд по миграции со старого кода |

### 📂 Исходные файлы модулей:

```
Assets/Scripts/Modules/Game/
├── PaintCustomizationModule.cs       ← Покраска
├── WheelCustomizationModule.cs       ← Колёса
├── PerformanceUpgradeModule.cs       ← Улучшения
└── SpoilerCustomizationModule.cs     ← Спойлеры
```

---

## 💻 Примеры использования

### Пример 1: Покраска

```csharp
// Купить и покрасить машину
var colorData = _paintModule.GetColorData(colorIndex);
if (_paintModule.PurchaseColor(carIndex, colorIndex))
{
    _paintModule.PaintCar(carIndex, colorData.color);
    Debug.Log($"Машина покрашена в {colorData.colorName}");
}
```

### Пример 2: Колёса

```csharp
// Купить и установить колёса
if (_wheelModule.PurchaseWheel(wheelIndex))
{
    _wheelModule.ChangeWheels(carIndex, wheelIndex);
    Debug.Log("Колёса установлены!");
}
```

### Пример 3: Улучшения

```csharp
// Прокачать двигатель
var upgradeData = _performanceModule.GetEngineUpgradeData(level);
if (_performanceModule.PurchaseEngineUpgrade(carIndex, level))
{
    Debug.Log($"Мощность: x{upgradeData.powerMultiplier}");
}
```

### Пример 4: Спойлеры

```csharp
// Купить и установить спойлер
if (_spoilerModule.PurchaseSpoiler(carIndex, spoilerIndex))
{
    _spoilerModule.ChangeSpoiler(carIndex, spoilerIndex);
    
    float bonus = _spoilerModule.GetCurrentDownforceBonus(carIndex);
    Debug.Log($"Прижимная сила: +{bonus}");
}
```

---

## 🎨 API Модулей

### 1. PaintCustomizationModule

```csharp
// Информация
int GetColorCount()
ColorData GetColorData(int colorIndex)
bool IsColorUnlocked(int carIndex, int colorIndex)
Color GetCurrentColor(int carIndex)

// Действия
void PaintCar(int carIndex, Color color)
void SelectColor(int carIndex, int colorIndex)
bool PurchaseColor(int carIndex, int colorIndex)
```

### 2. WheelCustomizationModule

```csharp
// Информация
int GetWheelCount()
WheelData GetWheelData(int wheelIndex)
bool IsWheelUnlocked(int wheelIndex) // ⚠️ Без carIndex!
int GetCurrentWheelIndex(int carIndex)

// Действия
void ChangeWheels(int carIndex, int wheelIndex)
bool PurchaseWheel(int wheelIndex) // ⚠️ Для всех машин!
```

### 3. PerformanceUpgradeModule

```csharp
// Двигатель
UpgradeData GetEngineUpgradeData(int level)
bool IsEngineUpgradeUnlocked(int carIndex, int level)
int GetEngineLevel(int carIndex)
bool PurchaseEngineUpgrade(int carIndex, int level)

// Аналогично для: Brake, Nitro, Handling
```

### 4. SpoilerCustomizationModule

```csharp
// Информация
int GetSpoilerCount()
SpoilerData GetSpoilerData(int spoilerIndex)
bool IsSpoilerUnlocked(int carIndex, int spoilerIndex)
int GetCurrentSpoilerIndex(int carIndex)
float GetCurrentDownforceBonus(int carIndex)

// Действия
void ChangeSpoiler(int carIndex, int spoilerIndex)
bool PurchaseSpoiler(int carIndex, int spoilerIndex)
```

---

## 🔔 События

### Подписка на события:

```csharp
private void Start()
{
    // Paint
    _paintModule.OnCarPainted += (carIndex, color) => 
        Debug.Log($"Машина {carIndex} покрашена");
    
    // Wheel
    _wheelModule.OnWheelsChanged += (carIndex, wheelIndex) => 
        Debug.Log($"Колёса изменены на {wheelIndex}");
    
    // Performance
    _performanceModule.OnEngineUpgraded += (carIndex, level) => 
        Debug.Log($"Двигатель прокачан до {level}");
    
    // Spoiler
    _spoilerModule.OnSpoilerChanged += (carIndex, spoilerIndex) => 
        Debug.Log($"Спойлер изменён");
}
```

---

## 💾 Сохранение данных

Каждый модуль сохраняет данные отдельно:

| Модуль | Ключ PlayerPrefs | Данные |
|--------|------------------|--------|
| Paint | `CarPaint_{carIndex}` | Текущий цвет, разблокированные цвета |
| Wheel | `CarWheels_{carIndex}` | Выбранные колёса, разблокированные колёса |
| Performance | `CarPerformance_{carIndex}` | Уровни улучшений |
| Spoiler | `CarSpoiler_{carIndex}` | Текущий спойлер, разблокированные спойлеры |

**Всего:** 5 машин × 4 модуля = 20 ключей

---

## ✨ Преимущества

### 1. **Чистый код**
- Один модуль = одна ответственность
- Легко читать и понимать
- Каждый модуль < 300 строк

### 2. **Независимость**
- Модули не зависят друг от друга
- Можно отключить любой модуль
- Легко тестировать

### 3. **Расширяемость**
- Легко добавить новый тип кастомизации
- Просто создать новый модуль
- Не нужно трогать существующие

### 4. **Переиспользование**
- Модули можно использовать в других проектах
- Стандартный интерфейс BaseGameModule

---

## 🔄 Миграция со старого кода

См. **MIGRATION_FROM_OLD_CUSTOMIZATION.md**

Основные изменения:
- `string carName` → `int carIndex` (0-4)
- `CustomizationModule` → 4 специализированных модуля
- Колёса теперь общие для всех машин

---

## 📖 Читать далее

- **Modular_Customization_Architecture.md** - Полная документация
- **Modular_Customization_Quick_Start.md** - Шпаргалка по API
- **MODULAR_CUSTOMIZATION_SUMMARY.md** - Краткое резюме

---

## 🎮 Готово к использованию!

✅ 4 независимых модуля  
✅ Полная документация  
✅ Примеры кода  
✅ Без ошибок компиляции  

**Можно интегрировать и использовать! 🚗💨**

---

**Дата создания:** 2025-10-13  
**Версия:** 1.0  
**Архитектура:** Модульная (BaseGameModule)

