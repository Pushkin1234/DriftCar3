# 🎮 CustomizationController - Настройка под вашу структуру UI

## ✨ Что создано

Я создал **CustomizationController** специально под вашу структуру UI панели кастомизации! Теперь у вас есть полная система управления всеми типами улучшений.

## 🎯 На какие элементы накидывать скрипты:

### 1. **CustomizationController** - основной контроллер
**Накидывается на:** `TuningCanvas` (ваша панель кастомизации!)

### 2. **CustomizationModule** - модуль данных
**Накидывается на:** GameObject с ModuleManager (обычно GameController)

## 📋 Настройка в инспекторе:

### CustomizationController на TuningCanvas:
```
UI References:
├── Color Button: [ColorButton]
├── Wheels Button: [WeelsButton] 
├── Upgrade Button: [UpGradeButton]
├── Spoiler Button: [SpoilerButton]
└── Exit Button: [ExitButton]

Scroll Views:
├── Color Scroll View: [Scroll View Color]
├── Wheels Scroll View: [Scroll View Weels]
├── Upgrade Scroll View: [Scroll View UpGrade]
└── Spoiler Scroll View: [Scroll View Spoiler]

Color UI:
├── Color Buttons: [массив кнопок цветов из Scroll View Color]
├── Color Price Text: [текст цены]
├── Purchase Color Button: [кнопка покупки]
├── Select Color Button: [кнопка выбора]
└── Color Lock Icon: [иконка замка]

Wheels UI:
├── Wheel Buttons: [массив кнопок колес из Scroll View Weels]
├── Wheel Price Text: [текст цены]
├── Purchase Wheel Button: [кнопка покупки]
├── Select Wheel Button: [кнопка выбора]
└── Wheel Lock Icon: [иконка замка]

Upgrade UI:
├── Engine Buttons: [кнопки уровней двигателя]
├── Brake Buttons: [кнопки уровней тормозов]
├── Nitro Buttons: [кнопки уровней нитро]
├── Upgrade Price Text: [текст цены]
├── Purchase Upgrade Button: [кнопка покупки]
├── Select Upgrade Button: [кнопка выбора]
└── Upgrade Lock Icon: [иконка замка]

Spoiler UI:
├── Spoiler Buttons: [массив кнопок спойлеров]
├── Spoiler Price Text: [текст цены]
├── Purchase Spoiler Button: [кнопка покупки]
├── Select Spoiler Button: [кнопка выбора]
└── Spoiler Lock Icon: [иконка замка]

Bottom Actions:
├── Back Button: [НАЗАД]
└── Select Button: [ВЫБРАТЬ]

Configuration:
└── Target Car Model: "SportsCar"
```

## 🎮 Как это работает:

### 1. **Переключение панелей:**
- Нажатие на **ColorButton** → показывает панель цветов
- Нажатие на **WeelsButton** → показывает панель колес
- Нажатие на **UpGradeButton** → показывает панель улучшений
- Нажатие на **SpoilerButton** → показывает панель спойлеров

### 2. **Выбор элементов:**
- Нажатие на кнопку элемента → предварительный просмотр
- Отображение цены в правом нижнем углу
- Показ иконки замка для заблокированных элементов

### 3. **Покупка элементов:**
- Нажатие на кнопку "Купить" → списание монет
- Разблокировка элемента
- Замена кнопки "Купить" на кнопку "Выбрать"

### 4. **Применение элементов:**
- Нажатие на кнопку "Выбрать" → применение к машине
- Сохранение изменений

## 🔧 Интеграция с ScrollViewButtonController:

Для автоматического закрытия панелей при переключении, добавьте **ScrollViewButtonController** на каждую кнопку:

```
ColorButton (ScrollViewButtonController):
├── Scroll Rect: Scroll View Color
├── Control Button: ColorButton
├── Scroll Behavior: Toggle
├── Auto Close Others: ✓
└── Panel Group: "TuningPanels"

WeelsButton (ScrollViewButtonController):
├── Scroll Rect: Scroll View Weels
├── Control Button: WeelsButton
├── Scroll Behavior: Toggle
├── Auto Close Others: ✓
└── Panel Group: "TuningPanels"

UpGradeButton (ScrollViewButtonController):
├── Scroll Rect: Scroll View UpGrade
├── Control Button: UpGradeButton
├── Scroll Behavior: Toggle
├── Auto Close Others: ✓
└── Panel Group: "TuningPanels"

SpoilerButton (ScrollViewButtonController):
├── Scroll Rect: Scroll View Spoiler
├── Control Button: SpoilerButton
├── Scroll Behavior: Toggle
├── Auto Close Others: ✓
└── Panel Group: "TuningPanels"
```

## 💰 Система цен:

### Цвета:
- Белый: 0 💰 (разблокирован)
- Черный: 500 💰
- Красный: 300 💰
- Синий: 400 💰
- Зеленый: 350 💰
- Желтый: 600 💰
- Голубой: 450 💰
- Фиолетовый: 700 💰

### Улучшения двигателя:
- Стандартный: 0 💰 (разблокирован)
- Улучшенный: 1000 💰 (+20% мощности)
- Спортивный: 2500 💰 (+50% мощности)
- Турбо: 5000 💰 (+80% мощности)
- Максимальный: 10000 💰 (+120% мощности)

### Улучшения тормозов:
- Стандартные: 0 💰 (разблокированы)
- Улучшенные: 800 💰 (+30% эффективности)
- Спортивные: 2000 💰 (+60% эффективности)
- Керамические: 4000 💰 (+100% эффективности)
- Карбоновые: 8000 💰 (+150% эффективности)

### Улучшения нитро:
- Стандартный: 0 💰 (разблокирован)
- Улучшенный: 1200 💰 (+40% мощности)
- Спортивный: 3000 💰 (+80% мощности)
- Турбо: 6000 💰 (+130% мощности)
- Максимальный: 12000 💰 (+180% мощности)

### Спойлеры:
- Без спойлера: 0 💰 (разблокирован)
- Спортивный: 1500 💰
- GT: 3000 💰
- Racing: 5000 💰
- Carbon: 8000 💰
- Wing: 12000 💰

## 🎯 Готовый пример настройки:

1. **Добавьте CustomizationView** на TuningCanvas
2. **Добавьте CustomizationController** на TuningCanvas
3. **Назначьте CustomizationView** в поле _view контроллера
4. **Назначьте все UI элементы** в CustomizationView
5. **Добавьте ScrollViewButtonController** на каждую кнопку панели
6. **Настройте CustomizationModule** на GameController
7. **Готово!** Система кастомизации полностью функциональна

Теперь ваша панель кастомизации будет работать как полноценная система с покупками, предварительным просмотром и применением улучшений! 🚗✨
