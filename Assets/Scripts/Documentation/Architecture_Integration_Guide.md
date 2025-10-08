# 🚗 Полная интеграция системы кастомизации с архитектурой проекта

## ✅ Что учтено из текущей архитектуры:

### 1. **ModuleManager** - централизованное управление модулями
- Все модули регистрируются через ModuleManager.Instance
- Используются BaseGameModule и IPersistentModule
- Поддерживается инициализация модулей

### 2. **DataModule** - система сохранения данных
- Расширен для хранения кастомизации каждой из 5 машин
- Использует PlayerPrefs для сохранения
- Поддерживает JSON сериализацию

### 3. **ShopModule** - система магазина с 5 машинами
- Интегрирован с appliedCarIndex (текущая выбранная машина)
- Поддерживает покупку машин через isBuyShop
- Сохраняет выбор машины через SelectCar()

### 4. **PlayerView** - спавн машин на сцене Level
- Обновлен для применения кастомизации при загрузке уровня
- Автоматически применяет цвет, колеса, спойлеры, улучшения
- Работает с индексом машины из DataModule

### 5. **RCC_CarControllerV3** - система управления машинами
- Поддерживает применение улучшений к характеристикам
- Интегрируется с NitroModule для нитро
- Работает с материалами для покраски

## 🎯 Как это работает:

### Цикл работы системы:

```
1. Игрок в МАГАЗИНЕ → выбирает машину (0-4) → ShopModule.SelectCar()
   ↓
2. DataModule сохраняет appliedCarIndex
   ↓
3. Игрок в ТЮНИНГЕ → TuningController загружает кастомизацию для appliedCarIndex
   ↓
4. Игрок изменяет цвет/колеса/улучшения → сохраняется для конкретной машины
   ↓
5. Игрок на сцене LEVEL → PlayerView загружает машину по appliedCarIndex
   ↓
6. CustomizationModule.ApplyCustomizationToCarByIndex() применяет все изменения
```

## 💾 Система сохранения:

### Для каждой машины (0-4) сохраняется:
- **Цвет** (RGBA)
- **Колеса** (индекс)
- **Спойлер** (индекс)
- **Уровень двигателя** (0-4)
- **Уровень тормозов** (0-4)
- **Уровень нитро** (0-4)
- **Уровень управления** (0-4)
- **Разблокированные цвета** (массив bool)
- **Разблокированные улучшения** (массивы bool)
- **Разблокированные спойлеры** (массив bool)

### Ключи сохранения:
```
PlayerPrefs:
- "GameData" - основные данные игры (монеты, текущая машина, купленные машины)
- "CarCustomization_0" - кастомизация машины 0
- "CarCustomization_1" - кастомизация машины 1
- "CarCustomization_2" - кастомизация машины 2
- "CarCustomization_3" - кастомизация машины 3
- "CarCustomization_4" - кастомизация машины 4
```

## 📋 Настройка компонентов:

### 1. На сцене Menu/Shop:

**TuningCanvas** (TuningController):
```
- Автоматически работает с текущей машиной из DataModule.appliedCarIndex
- Сохраняет изменения при выходе
- Загружает кастомизацию при открытии
```

### 2. На сцене Level:

**PlayerView** на GameObject с машинами:
```
Cars (List<GameObject>):
├── [0] Car1 (первая машина - бесплатная)
├── [1] Car2 (вторая машина - 10 монет)
├── [2] Car3 (третья машина - 20 монет)
├── [3] Car4 (четвертая машина - 30 монет)
└── [4] Car5 (пятая машина - 40 монет)
```

**Важно:**
- Индексы в списке _cars должны соответствовать индексам в ShopModule
- При загрузке уровня активируется машина по appliedCarIndex
- Автоматически применяется кастомизация

### 3. На GameController:

**CustomizationModule**:
```
- Загружает кастомизацию для всех 5 машин при Initialize()
- Сохраняет изменения автоматически
- Применяет кастомизацию к машинам на сцене
```

## 🔄 Сценарии использования:

### Сценарий 1: Покупка и настройка новой машины
```
1. Игрок в МАГАЗИНЕ покупает Car2 (индекс 1)
2. ShopModule.BuyCar(1) → isBuyShop[1] = true
3. ShopModule.SelectCar(1) → appliedCarIndex = 1
4. DataModule сохраняет изменения
5. Игрок переходит в ТЮНИНГ
6. TuningController загружает кастомизацию для машины 1
7. Игрок меняет цвет на красный, покупает спойлер
8. При выходе сохраняется кастомизация для машины 1
9. Игрок на сцене LEVEL → активируется Car2 с красным цветом и спойлером
```

### Сценарий 2: Переключение между машинами
```
1. Игрок имеет 3 купленные машины (0, 1, 2)
2. Машина 0: белая, стандартный двигатель
3. Машина 1: красная, турбо двигатель, спортивный спойлер
4. Машина 2: синяя, максимальный двигатель, карбоновые тормоза
5. В МАГАЗИНЕ переключается на машину 1 → appliedCarIndex = 1
6. На LEVEL загружается машина 1 со всеми улучшениями
```

### Сценарий 3: Улучшение текущей машины
```
1. Игрок на машине 0 (appliedCarIndex = 0)
2. В ТЮНИНГЕ покупает улучшение двигателя уровня 2
3. CustomizationModule сохраняет engineLevel = 2 для машины 0
4. На LEVEL машина 0 имеет +50% мощности
5. Переключается на машину 1 → она имеет свои улучшения
6. Возвращается на машину 0 → улучшения сохранены
```

## 🎮 API для работы с кастомизацией:

### Получение кастомизации:
```csharp
CustomizationModule customizationModule = ModuleManager.Instance.GetModule<CustomizationModule>();
DataModule dataModule = ModuleManager.Instance.GetModule<DataModule>();

// Получить текущую машину
int currentCarIndex = dataModule.Data.appliedCarIndex;

// Получить кастомизацию текущей машины
var customization = customizationModule.GetCarCustomizationByIndex(currentCarIndex);
```

### Применение кастомизации:
```csharp
// Применить кастомизацию к GameObject машины
customizationModule.ApplyCustomizationToCarByIndex(carGameObject, currentCarIndex);
```

### Сохранение кастомизации:
```csharp
// Сохранить изменения для конкретной машины
customizationModule.SaveCarCustomizationByIndex(currentCarIndex);
```

### Проверка разблокировок для конкретной машины:
```csharp
// Проверить разблокирован ли цвет для текущей машины
bool isUnlocked = customizationModule.IsColorUnlocked($"Car_{currentCarIndex}", colorIndex);

// Проверить разблокировано ли улучшение для текущей машины
bool isEngineUnlocked = customizationModule.IsEngineUpgradeUnlocked($"Car_{currentCarIndex}", level);
```

## ⚙️ Интеграция с RCC (Realistic Car Controller):

### Применение улучшений к RCC:
```csharp
private void ApplyUpgradesToCarObject(GameObject carObject, CarCustomization customization)
{
    var carController = carObject.GetComponent<RCC_CarControllerV3>();
    if (carController != null)
    {
        // Улучшение двигателя
        carController.maxEngineTorque *= engineUpgrade.powerMultiplier;
        
        // Улучшение тормозов
        carController.brakeTorque *= brakeUpgrade.powerMultiplier;
        
        // Улучшение управления
        carController.tractionHelperStrength *= handlingUpgrade.powerMultiplier;
    }
}
```

## 🔍 Отладка:

### Проверка сохранения:
```csharp
// В консоли Unity
[ContextMenu("Show All Car Customizations")]
public void ShowAllCarCustomizations()
{
    for (int i = 0; i < 5; i++)
    {
        var customization = customizationModule.GetCarCustomizationByIndex(i);
        Debug.Log($"Car {i}: Color={customization.paintColor}, Engine={customization.engineLevel}, Brake={customization.brakeLevel}");
    }
}
```

### Логи в консоли:
- `[CustomizationModule] Loaded customization for car index {i}`
- `[CustomizationModule] Saved customization for car index {i}`
- `[CustomizationModule] Applied customization to car index {i}`
- `[PlayerView] Applied customization to car index {i} on Level scene`
- `[TuningController] Working with car index: {i}`

---

**Система полностью интегрирована с архитектурой проекта!** ✅
