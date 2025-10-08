# 🏗️ Архитектура системы тюнинга - Финальная структура

## 📐 Диаграмма компонентов:

```
┌─────────────────────────────────────────────────────────────────────┐
│                          UNITY SCENE                                 │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ TuningCanvas                                                   │  │
│  │  ├─ TuningView (MonoBehaviour)         [VIEW LAYER]           │  │
│  │  │   ├─ UI Elements (SerializeField)                          │  │
│  │  │   │   ├─ Buttons                                           │  │
│  │  │   │   ├─ TextMeshProUGUI                                   │  │
│  │  │   │   ├─ ScrollRects                                       │  │
│  │  │   │   └─ GameObjects                                       │  │
│  │  │   └─ Events (Action)                                       │  │
│  │  │       ├─ OnColorSelected                                   │  │
│  │  │       ├─ OnPurchaseRequested                               │  │
│  │  │       └─ OnSelectRequested                                 │  │
│  │  │                                                             │  │
│  │  └─ TuningController (MonoBehaviour)   [CONTROLLER LAYER]     │  │
│  │      ├─ Reference to TuningView                               │  │
│  │      ├─ Event Handlers                                        │  │
│  │      │   ├─ HandleColorSelection()                            │  │
│  │      │   ├─ HandlePurchase()                                  │  │
│  │      │   └─ HandleApply()                                     │  │
│  │      └─ Calls to Module                                       │  │
│  │          ├─ _customizationModule.PurchaseColor()              │  │
│  │          ├─ _customizationModule.GetColorData()               │  │
│  │          └─ _view.UpdateColorUI()                             │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                              ↕ (calls)
┌─────────────────────────────────────────────────────────────────────┐
│                    MODULE MANAGER (Singleton)                        │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ CustomizationModule (BaseGameModule)  [MODEL LAYER]           │  │
│  │  ├─ Data Structures                                           │  │
│  │  │   ├─ CarCustomization                                      │  │
│  │  │   ├─ ColorData[]                                           │  │
│  │  │   ├─ UpgradeData[]                                         │  │
│  │  │   └─ SpoilerData[]                                         │  │
│  │  ├─ Business Logic                                            │  │
│  │  │   ├─ PurchaseColor()                                       │  │
│  │  │   ├─ IsColorUnlocked()                                     │  │
│  │  │   ├─ PurchaseEngineUpgrade()                               │  │
│  │  │   └─ ValidatePrice()                                       │  │
│  │  └─ Data Persistence                                          │  │
│  │      ├─ SaveCarCustomizationByIndex()                         │  │
│  │      ├─ LoadCarCustomizationByIndex()                         │  │
│  │      └─ GetCarCustomizationByIndex()                          │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                      │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ DataModule (BaseGameModule)                                   │  │
│  │  ├─ GameData                                                  │  │
│  │  │   ├─ coins                                                 │  │
│  │  │   ├─ appliedCarIndex                                       │  │
│  │  │   └─ isBuyShop[]                                           │  │
│  │  └─ SaveData() / LoadData()                                   │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                              ↕ (calls)
┌─────────────────────────────────────────────────────────────────────┐
│                    SERVICE LAYER                                     │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ CarCustomizationApplier (Singleton MonoBehaviour)             │  │
│  │  ├─ ApplyCustomization(GameObject, CarCustomization, Module)  │  │
│  │  │   ├─ ApplyPaint()                                          │  │
│  │  │   ├─ ApplyWheels()                                         │  │
│  │  │   ├─ ApplySpoiler()                                        │  │
│  │  │   └─ ApplyUpgrades()                                       │  │
│  │  └─ Integration with RCC                                      │  │
│  │      ├─ RCC_CarControllerV3                                   │  │
│  │      ├─ RCC_CustomizationApplier                              │  │
│  │      └─ NitroModule                                           │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                              ↕ (applies to)
┌─────────────────────────────────────────────────────────────────────┐
│                        GAME OBJECTS                                  │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ Car GameObject (on Level scene)                               │  │
│  │  ├─ RCC_CarControllerV3                                       │  │
│  │  ├─ MeshRenderer (for paint)                                  │  │
│  │  ├─ WheelColliders (for wheels)                               │  │
│  │  ├─ SpoilerPoint (for spoilers)                               │  │
│  │  └─ NitroModule                                               │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

## 🔄 Поток данных:

### Сценарий 1: Покупка цвета
```
1. USER: Clicks Color Button
   ↓
2. VIEW: OnColorSelected event (colorIndex) → Controller
   ↓
3. CONTROLLER: HandleColorSelection(colorIndex)
   ├─ Call: _customizationModule.SelectColor(carName, colorIndex)
   └─ Call: RefreshColorPanel()
   ↓
4. MODULE: SelectColor()
   ├─ Update internal state
   ├─ Call: OnColorSelected event (for preview)
   └─ Return color data
   ↓
5. CONTROLLER: Get color data from Module
   ├─ colorData = _customizationModule.GetColorData(colorIndex)
   └─ isUnlocked = _customizationModule.IsColorUnlocked(carName, colorIndex)
   ↓
6. CONTROLLER: Update View
   └─ _view.UpdateColorUI(price, isUnlocked, color)
   ↓
7. VIEW: Updates UI elements
   ├─ _colorPriceText.text = "500 💰"
   ├─ _colorLockIcon.SetActive(false)
   └─ _purchaseColorButton.gameObject.SetActive(true)

8. USER: Clicks Purchase Button
   ↓
9. VIEW: OnColorPurchaseRequested event → Controller
   ↓
10. CONTROLLER: HandleColorPurchase()
    └─ success = _customizationModule.PurchaseColor(carName, colorIndex)
    ↓
11. MODULE: PurchaseColor()
    ├─ Validate: enough coins?
    ├─ Deduct coins: _dataModule.Data.coins -= price
    ├─ Unlock: colorData.isUnlocked = true
    ├─ Save: SaveCustomizations()
    └─ Return: true/false
    ↓
12. CONTROLLER: RefreshColorPanel()
    └─ _view.UpdateColorUI() with new state
    ↓
13. VIEW: Updates UI
    ├─ _purchaseColorButton.SetActive(false)
    └─ _selectColorButton.SetActive(true)
```

### Сценарий 2: Применение на сцене Level
```
1. SCENE: Level loads
   ↓
2. PLAYERVIEW: Start()
   ├─ Get: carIndex = _dataModule.Data.appliedCarIndex
   ├─ PlacingSkin(carIndex) → Activates correct car
   └─ ApplyCustomization(carIndex)
   ↓
3. PLAYERVIEW: ApplyCustomization(carIndex)
   ├─ Get: customization = _customizationModule.GetCarCustomizationByIndex(carIndex)
   └─ Call: CarCustomizationApplier.Instance.ApplyCustomization(carObject, customization, module)
   ↓
4. SERVICE: ApplyCustomization()
   ├─ ApplyPaint(carObject, customization.paintColor)
   │   ├─ Find: RCC_CustomizationApplier (preferred)
   │   └─ Or: MeshRenderer.material.color = color
   │
   ├─ ApplyWheels(carObject, customization.selectedWheelIndex)
   │   ├─ Get: wheelData from module
   │   └─ Update: RCC_CarControllerV3.AllWheelColliders
   │
   ├─ ApplySpoiler(carObject, customization.selectedSpoilerIndex)
   │   ├─ Find/Create: SpoilerPoint
   │   ├─ Destroy: old spoiler
   │   └─ Instantiate: new spoiler prefab
   │
   └─ ApplyUpgrades(carObject, customization)
       ├─ Get: RCC_CarControllerV3
       ├─ Multiply: maxEngineTorque *= engineUpgrade.powerMultiplier
       ├─ Multiply: brakeTorque *= brakeUpgrade.powerMultiplier
       └─ Update: NitroModule (if exists)
   ↓
5. RESULT: Car on scene has all customizations applied
```

## 📊 Зависимости между слоями:

```
VIEW (TuningView)
  ↓ (knows nothing about)
  
CONTROLLER (TuningController)
  ↓ (depends on)
  ├─ View (via reference)
  └─ Module (via ModuleManager)
  
MODEL (CustomizationModule)
  ↓ (depends on)
  └─ DataModule (via ModuleManager)
  
SERVICE (CarCustomizationApplier)
  ↓ (depends on)
  ├─ Module (for data)
  └─ GameObject (for application)
```

## 🎯 Ключевые принципы:

### 1. Separation of Concerns
```
VIEW        → ТОЛЬКО отображение
CONTROLLER  → ТОЛЬКО координация
MODEL       → ТОЛЬКО данные и бизнес-логика
SERVICE     → ТОЛЬКО применение к GameObject
```

### 2. Single Responsibility
```
Каждый класс имеет ОДНУ причину для изменения:
- View меняется если меняется UI
- Controller меняется если меняется логика взаимодействия
- Module меняется если меняются правила бизнес-логики
- Service меняется если меняется способ применения к машинам
```

### 3. Dependency Inversion
```
Высокоуровневые модули НЕ зависят от низкоуровневых:
- Controller не знает о конкретной реализации UI
- Module не знает о конкретном способе применения к GameObject
- Все зависимости через абстракции (events, interfaces)
```

### 4. Open/Closed
```
Система открыта для расширения, закрыта для модификации:
- Добавление нового типа кастомизации НЕ требует изменения существующего кода
- Можно добавить новый View без изменения Controller
- Можно добавить новый способ применения без изменения Module
```

## 🔧 Расширяемость:

### Добавление нового типа кастомизации (например, Decals):

1. **Module** → Add DecalData structure and business logic
2. **View** → Add UI elements and events
3. **Controller** → Add event handlers
4. **Service** → Add ApplyDecal() method

**Изменяется ТОЛЬКО 4 метода в разных файлах - НЕ переписывается существующий код!**

---

**Архитектура полностью соответствует best practices и готова к долгосрочной поддержке!** ✅
