# 🎨 Система кастомизации цветов - Настройка

## ✨ Что добавлено

Теперь при выборе цвета машина **автоматически окрашивается** в выбранный цвет, в **правом нижнем углу появляется цена**, а при покупке появляется кнопка **"Выбрать"**!

## 🔧 Как настроить

### Шаг 1: Настройка CustomizationView

Добавьте в CustomizationView следующие элементы:

```
Color Selection:
├── Color Buttons (массив кнопок цветов)
├── Selected Color Indicator (индикатор выбранного цвета)
├── Color Price Text (текст цены)
├── Purchase Color Button (кнопка покупки)
├── Select Color Button (кнопка выбора)
└── Lock Icon (иконка замка)
```

### Шаг 2: Настройка CustomizationController

В CustomizationController назначьте:
```
Target Car Model Name: "SportsCar" (или название вашей машины)
```

### Шаг 3: Настройка CustomizationModule

В CustomizationModule настройте цвета:
```
Available Colors:
├── Белый (0 💰) - разблокирован
├── Черный (500 💰) - заблокирован
├── Красный (300 💰) - заблокирован
├── Синий (400 💰) - заблокирован
├── Зеленый (350 💰) - заблокирован
├── Желтый (600 💰) - заблокирован
├── Голубой (450 💰) - заблокирован
└── Фиолетовый (700 💰) - заблокирован
```

## 🎮 Как это работает

### 1. Выбор цвета:
- **Пользователь нажимает** на кнопку цвета
- **Машина окрашивается** в выбранный цвет (предварительный просмотр)
- **В правом нижнем углу** появляется цена цвета
- **Показывается иконка замка** если цвет заблокирован

### 2. Покупка цвета:
- **Пользователь нажимает** кнопку "Купить"
- **Проверяется** достаточно ли монет
- **Если достаточно** - цвет разблокируется, монеты списываются
- **Кнопка "Купить"** заменяется на кнопку "Выбрать"

### 3. Применение цвета:
- **Пользователь нажимает** кнопку "Выбрать"
- **Цвет применяется** к машине окончательно
- **Изменения сохраняются**

## 📋 Структура UI

### Панель выбора цвета:
```
┌─────────────────────────────────┐
│  🎨 Выбор цвета                 │
├─────────────────────────────────┤
│  [🔴] [🔵] [🟢] [🟡] [🟣] [⚫]  │
│                                 │
│  Выбранный цвет: Красный        │
│  Цена: 300 💰                   │
│                                 │
│  [🔒 Купить] [✅ Выбрать]       │
└─────────────────────────────────┘
```

### Состояния кнопок:
- **Заблокированный цвет**: тусклый + иконка замка + кнопка "Купить"
- **Разблокированный цвет**: яркий + кнопка "Выбрать"

## 💻 Программное управление

```csharp
// Получение модулей
CustomizationModule customizationModule = ModuleManager.Instance.GetModule<CustomizationModule>();
DataModule dataModule = ModuleManager.Instance.GetModule<DataModule>();

// Выбор цвета для предварительного просмотра
customizationModule.SelectColor("SportsCar", 2); // Красный

// Покупка цвета
bool success = customizationModule.PurchaseColor("SportsCar", 2);

// Проверка разблокирован ли цвет
bool isUnlocked = customizationModule.IsColorUnlocked("SportsCar", 2);

// Получение данных о цвете
var colorData = customizationModule.GetColorData(2);
Debug.Log($"Цвет: {colorData.colorName}, Цена: {colorData.price}");
```

## 🎯 События системы

```csharp
// Подписка на события
customizationModule.OnColorSelected += (carModel, colorIndex, color) => {
    Debug.Log($"Выбран цвет {colorIndex} для {carModel}");
};

customizationModule.OnColorPurchased += (carModel, colorIndex) => {
    Debug.Log($"Куплен цвет {colorIndex} для {carModel}");
};

customizationModule.OnColorUnlocked += (carModel, colorIndex) => {
    Debug.Log($"Разблокирован цвет {colorIndex} для {carModel}");
};
```

## ⚙️ Настройки в инспекторе

### CustomizationView:
- **Color Buttons**: массив кнопок цветов
- **Color Price Text**: текст для отображения цены
- **Purchase Color Button**: кнопка покупки
- **Select Color Button**: кнопка выбора
- **Lock Icon**: иконка замка

### CustomizationController:
- **Target Car Model Name**: название модели машины
- **View**: ссылка на CustomizationView

### CustomizationModule:
- **Available Colors**: массив цветов с ценами
- **Available Wheels**: массив колес (существующая функциональность)

## 🎨 Анимации

### При покупке:
- **Масштабирование кнопки** покупки
- **Изменение цвета** на зеленый
- **Плавное появление** кнопки "Выбрать"

### При ошибке покупки:
- **Тряска кнопки** покупки
- **Красное мигание** (опционально)

## 🔄 Сохранение данных

- **Разблокированные цвета** сохраняются для каждой машины отдельно
- **Монеты** списываются при покупке
- **Выбранный цвет** применяется к машине
- **Все изменения** автоматически сохраняются

## 🐛 Отладка

### Проверка состояния:
```csharp
// Проверка разблокированных цветов
for (int i = 0; i < customizationModule.GetColorCount(); i++)
{
    bool isUnlocked = customizationModule.IsColorUnlocked("SportsCar", i);
    Debug.Log($"Цвет {i}: {(isUnlocked ? "Разблокирован" : "Заблокирован")}");
}
```

### Логи в консоли:
- "Color [Name] selected for preview"
- "Color [Name] purchased for [Price] coins!"
- "Not enough coins to purchase [Name]"

---

**Готово!** Теперь ваша система кастомизации поддерживает покупку цветов с предварительным просмотром! 🎨✨
