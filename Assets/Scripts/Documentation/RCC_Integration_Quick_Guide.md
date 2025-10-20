# ⚡ Быстрое руководство по интеграции RCC кастомизации

## 🎯 Ключевые классы

### 1. **RCC_CustomizationManager** - Глобальный менеджер
```csharp
// Singleton - единственный экземпляр в сцене
RCC_CustomizationManager.Instance

// Текущая машина игрока
RCC_CustomizationApplier vehicle;

// Методы:
.Paint(Color color)           // Покрасить
.ChangeWheels(int wheelIndex) // Сменить колёса
.UpgradeSpeed()              // Улучшить двигатель
.UpgradeBrake()              // Улучшить тормоза
.UpgradeHandling()           // Улучшить управляемость
.Spoiler(int index)          // Установить спойлер
```

### 2. **RCC_CustomizationApplier** - На каждой машине
```csharp
// Добавляется на префаб машины
[RequireComponent(typeof(RCC_CarControllerV3))]

// Основные свойства:
RCC_CarControllerV3 CarController;
RCC_VehicleUpgrade_PaintManager PaintManager;
RCC_VehicleUpgrade_WheelManager WheelManager;
RCC_VehicleUpgrade_UpgradeManager UpgradeManager;
RCC_VehicleUpgrade_SpoilerManager SpoilerManager;

// Сохранение:
string saveFileName;  // Ключ PlayerPrefs
RCC_CustomizationLoadout loadout;

// Методы:
.SaveLoadout()  // Сохранить в PlayerPrefs
.LoadLoadout()  // Загрузить из PlayerPrefs
```

### 3. **RCC_Customization** - Статические утилиты
```csharp
// Все методы статические, работают напрямую с машиной

RCC_Customization.ChangeWheels(vehicle, wheelPrefab, applyRadius);
RCC_Customization.SetMaximumTorque(vehicle, 5000f);
RCC_Customization.SetMaximumSpeed(vehicle, 250f);
RCC_Customization.SetMaximumBrake(vehicle, 3000f);
RCC_Customization.SetFrontCambers(vehicle, -3f);
RCC_Customization.SetSmokeColor(vehicle, 0, Color.red);
RCC_Customization.SetHeadlightsColor(vehicle, Color.blue);
```

---

## 🚗 Где находится машина

### В Runtime:

```csharp
// Способ 1: Через CustomizationManager
RCC_CustomizationApplier applier = RCC_CustomizationManager.Instance.vehicle;

// Способ 2: Через SceneManager
RCC_CarControllerV3 car = RCC_SceneManager.Instance.activePlayerVehicle;
RCC_CustomizationApplier applier = car.GetComponent<RCC_CustomizationApplier>();

// Способ 3: Найти по ссылке
GameObject carObject = /* ваша ссылка */;
RCC_CustomizationApplier applier = carObject.GetComponent<RCC_CustomizationApplier>();
```

---

## 🎨 Как применить изменения

### Покраска

```csharp
// Вариант 1: Через глобальный менеджер
RCC_CustomizationManager.Instance.Paint(Color.red);

// Вариант 2: Напрямую через Applier
applier.PaintManager.Paint(Color.red);

// Что происходит:
// 1. Цвет применяется к materials[index] всех Paint элементов
// 2. Сохраняется в loadout.paint
// 3. Автосохранение в PlayerPrefs
```

### Смена колёс

```csharp
// Вариант 1: Через менеджер
RCC_CustomizationManager.Instance.ChangeWheels(2); // индекс колеса

// Вариант 2: Напрямую
applier.WheelManager.UpdateWheel(2);

// Что происходит:
// 1. Старые модели колёс отключаются
// 2. Создаются новые из префаба RCC_ChangableWheels.Instance.wheels[2]
// 3. Для правых колёс зеркалит scale.x
// 4. Сохраняется в loadout.wheel
```

### Улучшения

```csharp
// Двигатель
RCC_CustomizationManager.Instance.UpgradeSpeed();

// Тормоза
RCC_CustomizationManager.Instance.UpgradeBrake();

// Управляемость
RCC_CustomizationManager.Instance.UpgradeHandling();

// Что происходит:
// 1. Увеличивается уровень (engineLevel++, brakeLevel++, и т.д.)
// 2. Применяется к свойствам машины (maxEngineTorque, brakeTorque)
// 3. Сохраняется в loadout
```

### Спойлеры

```csharp
// Установить спойлер
RCC_CustomizationManager.Instance.Spoiler(1); // индекс

// Убрать спойлер
RCC_CustomizationManager.Instance.Spoiler(-1);

// Что происходит:
// 1. Все спойлеры отключаются
// 2. Включается выбранный spoilers[index].gameObject.SetActive(true)
// 3. Сохраняется в loadout.spoiler
```

---

## 💾 Сохранение и загрузка

### Автоматическое сохранение

```csharp
// При изменении любого параметра автоматически вызывается:
applier.SaveLoadout();

// Внутри:
PlayerPrefs.SetString(saveFileName, JsonUtility.ToJson(loadout));
```

### Автоматическая загрузка

```csharp
// При OnEnable() если autoLoadLoadout = true
private void OnEnable()
{
    if (autoLoadLoadout)
        LoadLoadout();
    
    // Применяет загруженные данные
    PaintManager.Initialize();
    WheelManager.Initialize();
    UpgradeManager.Initialize();
    SpoilerManager.Initialize();
}
```

### Ручное управление

```csharp
// Отключить автозагрузку
applier.autoLoadLoadout = false;

// Загрузить вручную
applier.LoadLoadout();

// Сохранить вручную
applier.SaveLoadout();

// Проверить есть ли сохранение
bool hasSave = PlayerPrefs.HasKey(applier.saveFileName);
```

---

## 🔗 Интеграция с вашим CustomizationModule

### Пример 1: Применить кастомизацию при загрузке машины

```csharp
// В вашем PlayerView или CarSpawner
public void SpawnCar(int carIndex)
{
    // 1. Создаём машину
    GameObject car = Instantiate(carPrefabs[carIndex]);
    
    // 2. Получаем Applier
    var applier = car.GetComponent<RCC_CustomizationApplier>();
    
    // 3. Отключаем автозагрузку RCC (используем свою)
    applier.autoLoadLoadout = false;
    
    // 4. Загружаем данные из CustomizationModule
    var customization = CustomizationModule.GetCarCustomizationByIndex(carIndex);
    
    // 5. Применяем через RCC
    applier.PaintManager.Paint(customization.color);
    
    if (customization.selectedWheelIndex >= 0)
        applier.WheelManager.UpdateWheel(customization.selectedWheelIndex);
    
    if (customization.selectedSpoilerIndex >= 0)
        applier.SpoilerManager.Upgrade(customization.selectedSpoilerIndex);
    
    // 6. Применяем улучшения
    RCC_Customization.SetMaximumTorque(
        applier.CarController, 
        GetEngineTorque(customization.engineLevel)
    );
}
```

### Пример 2: Сохранение через CustomizationModule

```csharp
// В CustomizationController
private void HandleColorPurchase()
{
    // 1. Покупаем через CustomizationModule
    bool success = _customizationModule.PurchaseColor(carName, colorIndex);
    
    if (success)
    {
        // 2. Применяем визуально через RCC
        var car = GetCurrentCar();
        var applier = car.GetComponent<RCC_CustomizationApplier>();
        applier.PaintManager.Paint(colorData.color);
        
        // 3. НЕ вызываем applier.SaveLoadout() - используем свою систему
        _customizationModule.SaveCarCustomizationByIndex(_currentCarIndex);
    }
}
```

### Пример 3: Синхронизация данных

```csharp
// Синхронизировать RCC loadout с вашими данными
public void SyncFromRCC(GameObject car, int carIndex)
{
    var applier = car.GetComponent<RCC_CustomizationApplier>();
    var customization = GetCarCustomizationByIndex(carIndex);
    
    // Читаем из RCC loadout
    customization.color = applier.loadout.paint;
    customization.selectedWheelIndex = applier.loadout.wheel;
    customization.selectedSpoilerIndex = applier.loadout.spoiler;
    customization.engineLevel = applier.loadout.engineLevel;
    customization.brakeLevel = applier.loadout.brakeLevel;
    
    // Сохраняем в ваш DataModule
    SaveCarCustomizationByIndex(carIndex);
}

// Синхронизировать ваши данные с RCC
public void SyncToRCC(GameObject car, int carIndex)
{
    var applier = car.GetComponent<RCC_CustomizationApplier>();
    var customization = GetCarCustomizationByIndex(carIndex);
    
    // Записываем в RCC loadout
    applier.loadout.paint = customization.color;
    applier.loadout.wheel = customization.selectedWheelIndex;
    applier.loadout.spoiler = customization.selectedSpoilerIndex;
    applier.loadout.engineLevel = customization.engineLevel;
    applier.loadout.brakeLevel = customization.brakeLevel;
    
    // НЕ сохраняем через RCC - используем свою систему
}
```

---

## 🛠️ Настройка в Unity

### На префабе машины:

```
1. Добавить RCC_CustomizationApplier:
   - Add Component → RCC Customization Applier
   - Save File Name: "Car_0" (уникальное имя)
   - Auto Load Loadout: false (если используете свою систему)

2. Создать дочерние объекты:
   CarPrefab
   ├─ PaintManager (Add → RCC Vehicle Upgrade Paint Manager)
   │  └─ Paint_1 (Add → RCC Vehicle Upgrade Paint)
   │     - Body Renderer: [MeshRenderer кузова]
   │     - Index: 0
   ├─ WheelManager (Add → RCC Vehicle Upgrade Wheel Manager)
   ├─ UpgradeManager (Add → RCC Vehicle Upgrade Upgrade Manager)
   ├─ SpoilerManager (Add → RCC Vehicle Upgrade Spoiler Manager)
   └─ SirenManager (Add → RCC Vehicle Upgrade Siren Manager)

3. В сцене добавить:
   - RCC_CustomizationManager (один на сцену)
   - Auto Register Player Vehicle: true
```

---

## 📋 Чеклист интеграции

### ✅ Шаг 1: Настройка префабов машин
- [ ] Добавить `RCC_CustomizationApplier` на каждую машину
- [ ] Создать дочерние менеджеры (Paint, Wheel, Upgrade, Spoiler)
- [ ] Назначить `Body Renderer` в Paint компонентах
- [ ] Установить `saveFileName` (уникальное для каждой машины)

### ✅ Шаг 2: Настройка сцены
- [ ] Добавить `RCC_CustomizationManager` в сцену
- [ ] Проверить что `RCC_SceneManager` есть
- [ ] Настроить `RCC_ChangableWheels` (список колёс)

### ✅ Шаг 3: Интеграция с CustomizationModule
- [ ] Отключить `autoLoadLoadout` на машинах
- [ ] Применять изменения через RCC менеджеры
- [ ] Сохранять через свой DataModule
- [ ] Синхронизировать данные при загрузке

### ✅ Шаг 4: Тестирование
- [ ] Покраска работает
- [ ] Смена колёс работает
- [ ] Улучшения применяются
- [ ] Спойлеры устанавливаются
- [ ] Сохранение/загрузка работает

---

## 🎮 Готовые сниппеты

### Покрасить текущую машину

```csharp
public void PaintCurrentCar(Color color)
{
    var applier = RCC_CustomizationManager.Instance.vehicle;
    if (applier != null)
        applier.PaintManager.Paint(color);
}
```

### Сменить колёса

```csharp
public void ChangeWheels(int wheelIndex)
{
    var applier = RCC_CustomizationManager.Instance.vehicle;
    if (applier != null)
        applier.WheelManager.UpdateWheel(wheelIndex);
}
```

### Улучшить все характеристики

```csharp
public void UpgradeAll()
{
    var manager = RCC_CustomizationManager.Instance;
    manager.UpgradeSpeed();    // Двигатель
    manager.UpgradeBrake();    // Тормоза
    manager.UpgradeHandling(); // Управляемость
}
```

### Сбросить кастомизацию

```csharp
public void ResetCustomization()
{
    var applier = RCC_CustomizationManager.Instance.vehicle;
    
    // Сбросить цвет
    applier.PaintManager.Paint(Color.white);
    
    // Убрать спойлер
    applier.SpoilerManager.Upgrade(-1);
    
    // Стандартные колёса
    applier.WheelManager.UpdateWheel(0);
    
    // Сбросить loadout
    applier.loadout = new RCC_CustomizationLoadout();
    applier.SaveLoadout();
}
```

---

**Готово! Теперь вы знаете как работает система кастомизации RCC и как интегрировать её в ваш проект! 🚗✨**

