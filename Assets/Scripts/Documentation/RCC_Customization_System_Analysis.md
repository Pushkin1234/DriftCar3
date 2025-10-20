# 🔍 Полный анализ системы кастомизации RCC

## 📚 Обзор структуры

Система кастомизации RCC состоит из **нескольких уровней:**

```
┌─────────────────────────────────────────┐
│   RCC_CustomizationManager (Singleton)  │ ← Глобальный менеджер
│   Управляет всей кастомизацией         │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│   RCC_CustomizationApplier              │ ← На каждой машине
│   Применяет изменения к конкретной      │
│   машине через 5 менеджеров             │
└──────────────────┬──────────────────────┘
                   │
         ┌─────────┴─────────┐
         ▼                   ▼
┌────────────────┐   ┌────────────────┐
│ PaintManager   │   │ WheelManager   │
├────────────────┤   ├────────────────┤
│ UpgradeManager │   │ SpoilerManager │
├────────────────┤   ├────────────────┤
│ SirenManager   │   │                │
└────────────────┘   └────────────────┘
```

---

## 🎯 1. Где определяется машина

### 1.1 RCC_CustomizationManager (глобальный уровень)

```csharp
public class RCC_CustomizationManager : RCC_Singleton<RCC_CustomizationManager>
{
    public RCC_CustomizationApplier vehicle; // ← ТЕКУЩАЯ МАШИНА
    
    // Автоматически определяет активную машину игрока
    private void RCC_SceneManager_OnVehicleChanged()
    {
        // Получаем машину из RCC_SceneManager
        vehicle = RCC_SceneManager.Instance.activePlayerVehicle
                  .GetComponent<RCC_CustomizationApplier>();
    }
}
```

**Где:** `Assets/RealisticCarControllerV3/Scripts/RCC_CustomizationManager.cs`

**Как работает:**
1. Подписывается на событие `RCC_SceneManager.OnVehicleChanged`
2. Когда игрок меняет машину → автоматически обновляет ссылку `vehicle`
3. Все методы (`Paint()`, `ChangeWheels()`, и т.д.) работают с этой машиной

**Альтернативный способ - вручную:**
```csharp
// Установить целевую машину вручную
RCC_CustomizationManager.Instance.SetTarget(myCarApplier);
```

---

### 1.2 RCC_CustomizationApplier (на машине)

```csharp
[RequireComponent(typeof(RCC_CarControllerV3))] // ← Требует CarController
public class RCC_CustomizationApplier : MonoBehaviour
{
    private RCC_CarControllerV3 _carController;
    public RCC_CarControllerV3 CarController
    {
        get
        {
            if (_carController == null)
                // Получаем контроллер машины из дочерних объектов
                _carController = GetComponentInChildren<RCC_CarControllerV3>();
            
            return _carController;
        }
    }
}
```

**Где добавлять:** На **префаб машины** (или родительский GameObject)

**Структура в Unity:**
```
CarPrefab (GameObject)
├─ RCC_CustomizationApplier ← ЭТОТ КОМПОНЕНТ
├─ RCC_CarControllerV3
├─ PaintManager (Child GameObject)
│  └─ Paint_1 (RCC_VehicleUpgrade_Paint)
├─ WheelManager (Child GameObject)
├─ UpgradeManager (Child GameObject)
├─ SpoilerManager (Child GameObject)
└─ SirenManager (Child GameObject)
```

---

## 🎨 2. Как применяются изменения

### 2.1 Покраска (Paint)

#### Цепочка вызовов:

```
UI кнопка "Покрасить"
    ↓
RCC_CustomizationManager.Paint(color)
    ↓
vehicle.PaintManager.Paint(color)
    ↓
paints[i].UpdatePaint(color)
    ↓
bodyRenderer.materials[index].color = color
    ↓
Сохранение: ModApplier.loadout.paint = color
```

#### Код:

**Шаг 1: Глобальный менеджер**
```csharp
// RCC_CustomizationManager.cs
public void Paint(Color color)
{
    if (!vehicle) return;
    
    // Красим кузов
    vehicle.PaintManager.Paint(color);
    
    // Красим спойлеры (если включено)
    if (vehicle.SpoilerManager && vehicle.SpoilerManager.paintSpoilers)
        vehicle.SpoilerManager.Paint(color);
}
```

**Шаг 2: Paint Manager**
```csharp
// RCC_VehicleUpgrade_PaintManager.cs
public void Paint(Color newColor)
{
    // Применяем цвет ко всем Paint элементам
    for (int i = 0; i < paints.Length; i++)
        paints[i].UpdatePaint(newColor);
}
```

**Шаг 3: Paint (конкретная часть машины)**
```csharp
// RCC_VehicleUpgrade_Paint.cs
public void UpdatePaint(Color newColor)
{
    // Красим материал
    bodyRenderer.materials[index].color = newColor;
    
    // Сохраняем в loadout
    ModApplier.loadout.paint = newColor;
    ModApplier.SaveLoadout();
}
```

**Настройка в Unity:**
```
PaintManager (GameObject)
└─ Paint_1 (RCC_VehicleUpgrade_Paint)
   ├─ Body Renderer: [MeshRenderer кузова машины]
   └─ Index: 0 (индекс материала для покраски)
```

---

### 2.2 Смена колёс (Wheels)

#### Цепочка вызовов:

```
UI кнопка "Сменить колёса"
    ↓
RCC_CustomizationManager.ChangeWheels(wheelIndex)
    ↓
vehicle.WheelManager.UpdateWheel(wheelIndex)
    ↓
RCC_Customization.ChangeWheels(CarController, wheelPrefab, true)
    ↓
Для каждого WheelCollider:
  - Отключает старую модель
  - Создаёт новую модель из префаба
  - Применяет радиус (если applyRadius = true)
    ↓
Сохранение: ModApplier.loadout.wheel = wheelIndex
```

#### Код:

**Шаг 1: Wheel Manager**
```csharp
// RCC_VehicleUpgrade_WheelManager.cs
public void UpdateWheel(int wheelIndex)
{
    // Сохраняем индекс
    ModApplier.loadout.wheel = wheelIndex;
    ModApplier.SaveLoadout();
    
    // Применяем изменения через статический метод
    RCC_Customization.ChangeWheels(
        ModApplier.CarController, 
        RCC_ChangableWheels.Instance.wheels[wheelIndex].wheel, 
        true // applyRadius
    );
}
```

**Шаг 2: RCC_Customization (статический класс)**
```csharp
// RCC_Customization.cs
public static void ChangeWheels(RCC_CarControllerV3 vehicle, GameObject wheel, bool applyRadius)
{
    // Для каждого колеса
    for (int i = 0; i < vehicle.AllWheelColliders.Length; i++)
    {
        // 1. Отключаем старую модель
        if (vehicle.AllWheelColliders[i].wheelModel.GetComponent<MeshRenderer>())
            vehicle.AllWheelColliders[i].wheelModel.GetComponent<MeshRenderer>().enabled = false;
        
        // 2. Отключаем всех детей
        foreach (Transform t in vehicle.AllWheelColliders[i].wheelModel.GetComponentInChildren<Transform>())
            t.gameObject.SetActive(false);
        
        // 3. Создаём новое колесо
        GameObject newWheel = Instantiate(
            wheel, 
            vehicle.AllWheelColliders[i].wheelModel.position,
            vehicle.AllWheelColliders[i].wheelModel.rotation, 
            vehicle.AllWheelColliders[i].wheelModel // parent
        );
        
        // 4. Для правых колёс - зеркалим по X
        if (vehicle.AllWheelColliders[i].wheelModel.localPosition.x > 0f)
            newWheel.transform.localScale = new Vector3(
                newWheel.transform.localScale.x * -1f, 
                newWheel.transform.localScale.y, 
                newWheel.transform.localScale.z
            );
        
        // 5. Применяем радиус (опционально)
        if (applyRadius)
            vehicle.AllWheelColliders[i].WheelCollider.radius = 
                RCC_GetBounds.MaxBoundsExtent(wheel.transform);
    }
}
```

**Откуда берутся префабы колёс:**
```csharp
// RCC_ChangableWheels - ScriptableObject с массивом колёс
RCC_ChangableWheels.Instance.wheels[wheelIndex].wheel
```

---

### 2.3 Улучшения (Upgrades)

#### Типы улучшений:
- **Engine** (двигатель) → увеличивает `maxEngineTorque`
- **Handling** (управляемость) → улучшает `steeringHelper`
- **Brake** (тормоза) → увеличивает `brakeTorque`

#### Цепочка вызовов:

```
UI кнопка "Улучшить двигатель"
    ↓
RCC_CustomizationManager.UpgradeSpeed()
    ↓
vehicle.UpgradeManager.UpgradeEngine()
    ↓
Увеличивает уровень двигателя
    ↓
Применяет к машине: 
  CarController.maxEngineTorque += engineUpgrades[level].torque
    ↓
Сохранение: ModApplier.loadout.engineLevel++
```

#### Код:

```csharp
// RCC_CustomizationManager.cs
public void UpgradeSpeed()
{
    if (!vehicle) return;
    vehicle.UpgradeManager.UpgradeEngine();
}

// RCC_VehicleUpgrade_UpgradeManager.cs (предполагаемый код)
public void UpgradeEngine()
{
    // Увеличиваем уровень
    ModApplier.loadout.engineLevel++;
    
    // Применяем улучшение
    RCC_VehicleUpgrade_Engine engineUpgrade = GetComponent<RCC_VehicleUpgrade_Engine>();
    if (engineUpgrade)
        engineUpgrade.Upgrade(ModApplier.loadout.engineLevel);
    
    ModApplier.SaveLoadout();
}
```

---

### 2.4 Спойлеры (Spoilers)

#### Цепочка вызовов:

```
UI выбор спойлера
    ↓
RCC_CustomizationManager.Spoiler(spoilerIndex)
    ↓
vehicle.SpoilerManager.Upgrade(spoilerIndex)
    ↓
Отключает все спойлеры
    ↓
Включает выбранный: spoilers[spoilerIndex].gameObject.SetActive(true)
    ↓
Сохранение: ModApplier.loadout.spoiler = spoilerIndex
```

---

## 💾 3. Система сохранения

### 3.1 RCC_CustomizationLoadout

```csharp
[System.Serializable]
public class RCC_CustomizationLoadout
{
    public Color paint = new Color(1f, 1f, 1f, 0f);
    public int spoiler = -1;  // -1 = нет спойлера
    public int siren = -1;
    public int wheel = -1;
    
    public int engineLevel = 0;
    public int handlingLevel = 0;
    public int brakeLevel = 0;
}
```

### 3.2 Сохранение/Загрузка

```csharp
// RCC_CustomizationApplier.cs
public string saveFileName = ""; // Ключ для PlayerPrefs
public RCC_CustomizationLoadout loadout = new RCC_CustomizationLoadout();

// Сохранить
public void SaveLoadout()
{
    PlayerPrefs.SetString(saveFileName, JsonUtility.ToJson(loadout));
}

// Загрузить
public void LoadLoadout()
{
    loadout = new RCC_CustomizationLoadout();
    
    if (PlayerPrefs.HasKey(saveFileName))
        loadout = (RCC_CustomizationLoadout)JsonUtility.FromJson(
            PlayerPrefs.GetString(saveFileName), 
            typeof(RCC_CustomizationLoadout)
        );
}
```

**Ключ сохранения:**
- По умолчанию = `transform.name` (имя GameObject машины)
- Например: `"SportsCar"`, `"RaceCar"`, и т.д.

**Автозагрузка:**
```csharp
private void OnEnable()
{
    if (autoLoadLoadout)
        LoadLoadout(); // Загружает при активации
    
    // Инициализирует менеджеры с загруженными данными
    PaintManager?.Initialize();
    WheelManager?.Initialize();
    UpgradeManager?.Initialize();
    SpoilerManager?.Initialize();
}
```

---

## 🔄 4. Инициализация при загрузке

### Порядок инициализации:

```
1. OnEnable() вызывается
    ↓
2. LoadLoadout() - загружает JSON из PlayerPrefs
    ↓
3. PaintManager.Initialize()
    - Применяет loadout.paint
    ↓
4. WheelManager.Initialize()
    - Применяет loadout.wheel
    ↓
5. UpgradeManager.Initialize()
    - Применяет loadout.engineLevel, brakeLevel, handlingLevel
    ↓
6. SpoilerManager.Initialize()
    - Применяет loadout.spoiler
```

### Пример инициализации PaintManager:

```csharp
public void Initialize()
{
    if (paints == null) return;
    
    // Если есть сохранённый цвет - применяем
    if (ModApplier.loadout.paint != new Color(1f, 1f, 1f, 0f))
        Paint(ModApplier.loadout.paint);
}
```

---

## 🎮 5. Как использовать в вашем проекте

### Вариант 1: Прямое использование RCC системы

```csharp
// В вашем CustomizationController.cs
private void HandleColorPurchase()
{
    // Получаем машину
    RCC_CarControllerV3 car = GetCurrentCar();
    
    // Красим через RCC
    RCC_CustomizationManager.Instance.Paint(selectedColor);
    
    // ИЛИ напрямую через Applier
    var applier = car.GetComponent<RCC_CustomizationApplier>();
    applier.PaintManager.Paint(selectedColor);
}
```

### Вариант 2: Интеграция с вашим CustomizationModule

```csharp
// В CustomizationModule.cs
public void PaintCar(string carName, Color color)
{
    // Находим машину
    GameObject carObject = FindCarByName(carName);
    var applier = carObject.GetComponent<RCC_CustomizationApplier>();
    
    if (applier != null)
    {
        // Применяем через RCC
        applier.PaintManager.Paint(color);
    }
    
    // Сохраняем в ваш DataModule
    _carCustomizations[carName].color = color;
    SaveCarCustomization(carName);
}
```

### Вариант 3: Замена сохранения RCC на ваше

```csharp
// В RCC_CustomizationApplier установите:
autoLoadLoadout = false; // Отключаем автозагрузку RCC

// И управляйте через ваш CustomizationModule
public void ApplyCustomizationToCar(GameObject carObject, string carModelName)
{
    var applier = carObject.GetComponent<RCC_CustomizationApplier>();
    var customization = GetCarCustomization(carModelName);
    
    // Применяем через RCC менеджеры
    applier.PaintManager.Paint(customization.color);
    
    if (customization.selectedWheelIndex >= 0)
        applier.WheelManager.UpdateWheel(customization.selectedWheelIndex);
    
    if (customization.selectedSpoilerIndex >= 0)
        applier.SpoilerManager.Upgrade(customization.selectedSpoilerIndex);
}
```

---

## 📊 6. Сравнительная таблица

| Аспект | RCC Система | Ваша система |
|--------|------------|--------------|
| **Определение машины** | `RCC_SceneManager.activePlayerVehicle` | `DataModule.appliedCarIndex` |
| **Сохранение** | PlayerPrefs + JSON (по имени машины) | PlayerPrefs + JSON (по индексу 0-4) |
| **Покраска** | `PaintManager.Paint()` → материал | `CustomizationModule.PaintCar()` |
| **Колёса** | `RCC_Customization.ChangeWheels()` | Ваша реализация |
| **Улучшения** | `UpgradeManager` → прямые свойства машины | CustomizationModule + CarCustomizationData |
| **Спойлеры** | `SpoilerManager` → активация GameObject | Ваша реализация |

---

## ✅ Рекомендации для вашего проекта

### 1. Используйте RCC менеджеры для визуальных изменений

```csharp
// CustomizationModule.cs - только данные
public void PaintCar(string carName, Color color)
{
    _carCustomizations[carName].color = color;
}

// CarCustomizationApplier.cs (ваш сервис) - применение
public void ApplyCustomization(GameObject car, CarCustomizationData data)
{
    var rccApplier = car.GetComponent<RCC_CustomizationApplier>();
    
    // Используем RCC для визуального применения
    rccApplier.PaintManager.Paint(data.color);
    rccApplier.WheelManager.UpdateWheel(data.selectedWheelIndex);
}
```

### 2. Сохраняйте через ваш DataModule

```csharp
// Отключите автосохранение RCC
rccApplier.autoLoadLoadout = false;

// Используйте ваш DataModule для сохранения
_dataModule.SaveData();
```

### 3. Синхронизируйте данные

```csharp
public void SyncWithRCCLoadout(GameObject car)
{
    var rccApplier = car.GetComponent<RCC_CustomizationApplier>();
    var customization = GetCarCustomization(carName);
    
    // Синхронизируем данные
    customization.color = rccApplier.loadout.paint;
    customization.selectedWheelIndex = rccApplier.loadout.wheel;
    customization.engineLevel = rccApplier.loadout.engineLevel;
}
```

---

## 🎯 Итог

### Где определяется машина:
1. **Глобально:** `RCC_CustomizationManager.Instance.vehicle`
2. **На префабе:** `RCC_CustomizationApplier.CarController`
3. **Автоматически:** Через `RCC_SceneManager.OnVehicleChanged`

### Как применяются изменения:
1. **Покраска:** `PaintManager` → меняет `material.color`
2. **Колёса:** `WheelManager` → создаёт новые GameObject из префабов
3. **Улучшения:** `UpgradeManager` → меняет свойства `RCC_CarControllerV3`
4. **Спойлеры:** `SpoilerManager` → включает/выключает GameObject

### Сохранение:
- **Формат:** JSON через `RCC_CustomizationLoadout`
- **Хранилище:** `PlayerPrefs` с ключом = имя машины
- **Автозагрузка:** При `OnEnable()` → `LoadLoadout()` → `Initialize()`

**Вывод:** RCC система хорошо продумана и может отлично интегрироваться с вашим CustomizationModule! 🚗✨

