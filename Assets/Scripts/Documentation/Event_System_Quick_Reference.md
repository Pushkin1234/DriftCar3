# ⚡ Быстрый справочник по событиям CustomizationController

## 📋 Все доступные события

### События панелей навигации
```csharp
_view.OnColorPanelRequested    // Открыть панель цветов
_view.OnWheelsPanelRequested   // Открыть панель колес
_view.OnUpgradePanelRequested  // Открыть панель улучшений
_view.OnSpoilerPanelRequested  // Открыть панель спойлеров
_view.OnExitRequested          // Выход из кастомизации
_view.OnBackRequested          // Назад
_view.OnSelectRequested        // Применить все изменения
```

### События цветов
```csharp
_view.OnColorSelected(int colorIndex)  // Выбран цвет
_view.OnColorPurchaseRequested()       // Купить цвет
_view.OnColorSelectRequested()         // Применить цвет
```

### События колес
```csharp
_view.OnWheelSelected(int wheelIndex)  // Выбраны колеса
_view.OnWheelPurchaseRequested()       // Купить колеса
_view.OnWheelSelectRequested()         // Применить колеса
```

### События улучшений
```csharp
_view.OnEngineSelected(int level)      // Выбран уровень двигателя
_view.OnBrakeSelected(int level)       // Выбран уровень тормозов
_view.OnNitroSelected(int level)       // Выбран уровень нитро
_view.OnUpgradePurchaseRequested()     // Купить улучшение
_view.OnUpgradeSelectRequested()       // Применить улучшение
```

### События спойлеров
```csharp
_view.OnSpoilerSelected(int spoilerIndex)  // Выбран спойлер
_view.OnSpoilerPurchaseRequested()         // Купить спойлер
_view.OnSpoilerSelectRequested()           // Применить спойлер
```

---

## 🎯 Примеры использования в вашем проекте

### Пример 1: Покупка цвета для текущей машины

```csharp
// В CustomizationController.cs

private void HandleColorPurchase()
{
    // 1. Получаем название текущей машины
    string carName = $"Car_{_currentCarIndex}"; // Например: "Car_0", "Car_1"...
    
    // 2. Пытаемся купить цвет через модуль
    bool success = _customizationModule.PurchaseColor(carName, _selectedColorIndex);
    
    // 3. Обрабатываем результат
    if (success)
    {
        Debug.Log($"✅ Цвет {_selectedColorIndex} куплен для машины {_currentCarIndex}");
        
        // Обновляем UI
        RefreshColorPanel();
    }
    else
    {
        Debug.LogWarning($"❌ Недостаточно монет для покупки цвета!");
        
        // Можно показать сообщение игроку
        // UIManager.ShowMessage("Недостаточно средств!");
    }
}
```

### Пример 2: Применение купленного цвета

```csharp
private void HandleColorSelect()
{
    // 1. Получаем данные о цвете
    var colorData = _customizationModule.GetColorData(_selectedColorIndex);
    
    if (colorData != null)
    {
        // 2. Применяем цвет к машине
        string carName = $"Car_{_currentCarIndex}";
        _customizationModule.PaintCar(carName, colorData.color);
        
        Debug.Log($"🎨 Цвет {colorData.colorName} применен к машине {_currentCarIndex}");
        
        // 3. Сохраняем изменения
        SaveCarCustomization();
    }
}
```

### Пример 3: Выбор уровня двигателя

```csharp
private void HandleEngineSelection(int level)
{
    // 1. Сохраняем выбранный уровень
    _selectedEngineLevel = level;
    
    Debug.Log($"🔧 Выбран уровень двигателя: {level}");
    
    // 2. Получаем данные об улучшении
    var upgradeData = _customizationModule.GetEngineUpgradeData(level);
    
    if (upgradeData != null)
    {
        Debug.Log($"Название: {upgradeData.upgradeName}");
        Debug.Log($"Цена: {upgradeData.price}");
        Debug.Log($"Мощность: +{upgradeData.powerBoost}%");
    }
    
    // 3. Обновляем UI панели улучшений
    RefreshUpgradePanel();
}
```

### Пример 4: Покупка улучшения двигателя

```csharp
private void HandleUpgradePurchase()
{
    string carName = $"Car_{_currentCarIndex}";
    
    // Покупаем улучшение двигателя
    bool success = _customizationModule.PurchaseEngineUpgrade(
        carName, 
        _selectedEngineLevel
    );
    
    if (success)
    {
        Debug.Log($"⚡ Двигатель улучшен до уровня {_selectedEngineLevel}!");
        
        // Улучшение автоматически применяется при покупке
        RefreshUpgradePanel();
        
        // Сохраняем
        SaveCarCustomization();
    }
}
```

### Пример 5: Переключение между панелями

```csharp
private void HandleColorPanelRequest()
{
    Debug.Log("📊 Открываем панель цветов");
    
    // 1. Показываем панель цветов, скрываем остальные
    _view.ShowPanel(CustomizationView.PanelType.Color);
    
    // 2. Обновляем данные на панели
    RefreshColorPanel();
}

private void HandleWheelsPanelRequest()
{
    Debug.Log("🛞 Открываем панель колес");
    
    _view.ShowPanel(CustomizationView.PanelType.Wheels);
    RefreshWheelsPanel();
}
```

---

## 🔄 Жизненный цикл событий

### При запуске панели кастомизации:

```csharp
private void Start()
{
    // 1. Инициализируем модули
    InitializeModules();
    
    // 2. Подписываемся на все события View
    SubscribeToViewEvents();
    
    // 3. Загружаем данные текущей машины
    LoadCarCustomization();
    
    // 4. Показываем первую панель
    _view.ShowPanel(CustomizationView.PanelType.Color);
}
```

### При выборе элемента (например, цвета):

```
Пользователь нажимает кнопку цвета
         ↓
View генерирует событие: OnColorSelected(index)
         ↓
Controller получает событие: HandleColorSelection(index)
         ↓
Controller вызывает Module: SelectColor(carName, index)
         ↓
Module применяет изменения к данным
         ↓
Controller обновляет UI: RefreshColorPanel()
         ↓
View отображает обновленное состояние
```

### При покупке:

```
Пользователь нажимает "Купить"
         ↓
View: OnColorPurchaseRequested()
         ↓
Controller: HandleColorPurchase()
         ↓
Module: PurchaseColor() → списывает монеты, разблокирует цвет
         ↓
Если успешно:
  - Controller обновляет UI
  - Controller сохраняет данные
Если неудача:
  - Показываем сообщение об ошибке
```

---

## 💡 Практические советы

### 1. Как узнать индекс текущей машины?

```csharp
// CustomizationController.cs
private void InitializeModules()
{
    _dataModule = ModuleManager.Instance?.GetModule<DataModule>();
    
    // Получаем индекс выбранной машины из DataModule
    _currentCarIndex = _dataModule.Data.appliedCarIndex;
    
    Debug.Log($"Текущая машина: {_currentCarIndex}");
    // Выведет: 0, 1, 2, 3 или 4
}
```

### 2. Как проверить, куплена ли машина?

```csharp
private void LoadCarCustomization()
{
    // Проверяем куплена ли машина
    bool isCarPurchased = _dataModule.Data.isBuyShop[_currentCarIndex];
    
    if (!isCarPurchased)
    {
        Debug.LogWarning("Машина еще не куплена!");
        return;
    }
    
    // Загружаем кастомизацию
    var customization = _customizationModule.GetCarCustomizationByIndex(_currentCarIndex);
}
```

### 3. Как сохранить изменения?

```csharp
private void SaveCarCustomization()
{
    if (_customizationModule == null) return;
    
    // Сохраняет кастомизацию для текущей машины
    _customizationModule.SaveCarCustomizationByIndex(_currentCarIndex);
    
    Debug.Log($"💾 Кастомизация машины {_currentCarIndex} сохранена");
}
```

### 4. Как получить количество монет игрока?

```csharp
private void CheckPlayerCoins()
{
    int coins = _dataModule.Data.coins;
    Debug.Log($"У игрока {coins} монет");
    
    // Проверяем хватит ли на покупку
    var colorData = _customizationModule.GetColorData(_selectedColorIndex);
    if (coins >= colorData.price)
    {
        Debug.Log("✅ Достаточно монет");
    }
    else
    {
        Debug.Log($"❌ Не хватает {colorData.price - coins} монет");
    }
}
```

---

## 🐛 Частые проблемы и решения

### Проблема: События не срабатывают

```csharp
// Проверьте:

// 1. Назначен ли View в Inspector?
if (_view == null)
{
    Debug.LogError("❌ View не назначен!");
}

// 2. Подписались ли на события?
private void SubscribeToViewEvents()
{
    Debug.Log("✅ Подписываемся на события...");
    _view.OnColorSelected += HandleColorSelection;
}

// 3. Есть ли у View кнопки?
// Проверьте в CustomizationView, что все Button[] заполнены
```

### Проблема: Событие срабатывает несколько раз

```csharp
// Решение: отписываемся перед подпиской

private void SubscribeToViewEvents()
{
    // Сначала отписываемся (на случай повторного вызова)
    UnsubscribeFromViewEvents();
    
    // Затем подписываемся
    _view.OnColorSelected += HandleColorSelection;
}
```

### Проблема: NullReferenceException в Module

```csharp
private void HandleColorPurchase()
{
    // ОБЯЗАТЕЛЬНО проверяем на null!
    if (_customizationModule == null)
    {
        Debug.LogError("❌ CustomizationModule не найден!");
        return;
    }
    
    bool success = _customizationModule.PurchaseColor(...);
}
```

---

## 📊 Шпаргалка методов CustomizationModule

```csharp
// Работа с цветами
_customizationModule.GetColorCount()                        // Количество цветов
_customizationModule.GetColorData(index)                    // Данные о цвете
_customizationModule.IsColorUnlocked(carName, colorIndex)   // Разблокирован?
_customizationModule.PurchaseColor(carName, colorIndex)     // Купить
_customizationModule.SelectColor(carName, colorIndex)       // Выбрать (предпросмотр)
_customizationModule.PaintCar(carName, color)              // Применить цвет

// Работа с колесами
_customizationModule.GetWheelCount()                        // Количество колес
_customizationModule.GetWheelData(index)                    // Данные о колесах
_customizationModule.IsWheelUnlocked(wheelIndex)           // Разблокированы?
_customizationModule.UnlockWheel(wheelIndex)               // Купить
_customizationModule.ChangeWheels(carName, wheelIndex)      // Применить

// Работа с улучшениями
_customizationModule.GetEngineUpgradeCount()               // Количество уровней
_customizationModule.GetEngineUpgradeData(level)           // Данные об улучшении
_customizationModule.IsEngineUpgradeUnlocked(carName, level) // Разблокирован?
_customizationModule.PurchaseEngineUpgrade(carName, level) // Купить

// Работа со спойлерами
_customizationModule.GetSpoilerCount()                      // Количество спойлеров
_customizationModule.GetSpoilerData(index)                  // Данные о спойлере
_customizationModule.IsSpoilerUnlocked(carName, index)     // Разблокирован?
_customizationModule.PurchaseSpoiler(carName, index)       // Купить

// Сохранение
_customizationModule.SaveCarCustomizationByIndex(carIndex) // Сохранить для машины
_customizationModule.LoadAllCarCustomizations()            // Загрузить все
```

---

## 🎮 Готовые сниппеты

### Сниппет 1: Полная обработка покупки цвета

```csharp
private void HandleColorPurchase()
{
    // Проверки
    if (_customizationModule == null || _dataModule == null) return;
    
    string carName = $"Car_{_currentCarIndex}";
    var colorData = _customizationModule.GetColorData(_selectedColorIndex);
    
    if (colorData == null)
    {
        Debug.LogError("Данные о цвете не найдены!");
        return;
    }
    
    // Проверяем монеты
    if (_dataModule.Data.coins < colorData.price)
    {
        Debug.LogWarning($"Не хватает {colorData.price - _dataModule.Data.coins} монет!");
        // UIManager.ShowMessage("Недостаточно монет!");
        return;
    }
    
    // Покупаем
    bool success = _customizationModule.PurchaseColor(carName, _selectedColorIndex);
    
    if (success)
    {
        Debug.Log($"✅ Куплен цвет {colorData.colorName} за {colorData.price} монет");
        
        // Применяем сразу
        _customizationModule.PaintCar(carName, colorData.color);
        
        // Обновляем UI
        RefreshColorPanel();
        
        // Сохраняем
        SaveCarCustomization();
        
        // Эффекты
        // AudioManager.PlaySound("purchase_success");
        // ParticleManager.PlayEffect("coins_spent");
    }
}
```

### Сниппет 2: Проверка перед покупкой улучшения

```csharp
private bool CanPurchaseUpgrade(int level)
{
    string carName = $"Car_{_currentCarIndex}";
    
    // Проверка 1: Уже куплено?
    if (_customizationModule.IsEngineUpgradeUnlocked(carName, level))
    {
        Debug.Log("Уже куплено");
        return false;
    }
    
    // Проверка 2: Хватает ли монет?
    var upgradeData = _customizationModule.GetEngineUpgradeData(level);
    if (_dataModule.Data.coins < upgradeData.price)
    {
        Debug.Log($"Не хватает {upgradeData.price - _dataModule.Data.coins} монет");
        return false;
    }
    
    // Проверка 3: Куплен ли предыдущий уровень?
    if (level > 0 && !_customizationModule.IsEngineUpgradeUnlocked(carName, level - 1))
    {
        Debug.Log("Сначала купите предыдущий уровень");
        return false;
    }
    
    return true;
}
```

---

**Готово! Теперь у вас есть полный справочник по работе с событиями! 🚀**

