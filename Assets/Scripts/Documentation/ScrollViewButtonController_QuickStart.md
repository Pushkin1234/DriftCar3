# 🎮 ScrollViewButtonController - Быстрый старт

## 📋 Что это?

`ScrollViewButtonController` - это скрипт для управления Scroll View при нажатии на кнопку в проекте DriftCar3. Он интегрирован с модульной архитектурой проекта и поддерживает различные типы поведения прокрутки.

## 🚀 Быстрая настройка

### Шаг 1: Добавление компонента
1. Выберите GameObject с кнопкой
2. Добавьте компонент `ScrollViewButtonController`
3. Назначьте ScrollRect и Control Button в инспекторе

### Шаг 2: Базовая настройка
```
Scroll Rect: [Ваш ScrollRect]
Control Button: [Ваша кнопка]
Scroll Behavior: Toggle
Smooth Scrolling: ✓
Animation Duration: 0.5
```

### Шаг 3: Готово!
Скрипт автоматически:
- Зарегистрируется в UIModule
- Настроит обработчик кнопки
- Добавит анимации прокрутки

## 🎯 Типы поведения

### Toggle (Переключение)
- Показывает/скрывает Scroll View
- Самый простой тип использования

### Scroll Up/Down/Left/Right
- Прокручивает в указанном направлении
- Настраивается через Scroll Amount

### Custom (Пользовательское)
- Для создания собственной логики
- Переопределите метод `OnCustomScrollBehavior()`

## 💻 Программное управление

```csharp
// Получение контроллера
ScrollViewButtonController controller = GetComponent<ScrollViewButtonController>();

// Переключение
controller.ToggleScrollViewProgrammatically();

// Установка состояния
controller.SetScrollViewActive(true);

// Прокрутка
controller.ScrollToTop();
controller.ScrollToBottom();
controller.ScrollToLeft();
controller.ScrollToRight();
```

## 🏪 Специализированные контроллеры

### ShopScrollViewController
Для магазина с дополнительными возможностями:
- Автоматическое закрытие при выборе товара
- Прокрутка к определенному товару
- События открытия/закрытия

```csharp
ShopScrollViewController shopController = GetComponent<ShopScrollViewController>();
shopController.ScrollToItem(itemIndex);
```

## 🎨 Централизованное управление

```csharp
// Получение UIModule
UIModule uiModule = ModuleManager.Instance.GetModule<UIModule>();

// Закрытие всех Scroll View
uiModule.CloseAllScrollViews();

// Открытие по имени
uiModule.OpenScrollViewByName("MyScrollView");
```

## ⚙️ Настройки анимации

- **Animation Duration**: Длительность анимации (сек)
- **Smooth Scrolling**: Плавная прокрутка
- **Show Visual Feedback**: Визуальная обратная связь
- **Active/Inactive Color**: Цвета состояний

## 🔧 Примеры использования

### Пример 1: Простое меню
```
Scroll Behavior: Toggle
Smooth Scrolling: ✓
Animation Duration: 0.3
```

### Пример 2: Галерея изображений
```
Scroll Behavior: ScrollLeft
Scroll Amount: 0.25
Smooth Scrolling: ✓
```

### Пример 3: Магазин товаров
Используйте `ShopScrollViewController` с дополнительными настройками.

## 🐛 Отладка

### Контекстное меню (в редактоre):
- **Test Toggle** - тестирование переключения
- **Test Scroll Up** - тестирование прокрутки вверх
- **Test Scroll Down** - тестирование прокрутки вниз

### Логи в консоли:
- Регистрация в UIModule
- Ошибки инициализации
- Предупреждения о недоступных модулях

## 📝 Важные моменты

1. **Совместимость**: Работает без DOTween (использует корутины)
2. **Производительность**: Оптимизирован для WebGL
3. **Архитектура**: Интегрирован с модульной системой проекта
4. **Расширяемость**: Легко создавать специализированные контроллеры

## 🆘 Решение проблем

### Scroll View не реагирует на кнопку:
- Проверьте назначение ScrollRect и Control Button
- Убедитесь, что кнопка не заблокирована другими UI элементами

### Анимация не работает:
- Проверьте настройку Smooth Scrolling
- Убедитесь, что Animation Duration > 0

### Ошибки в консоли:
- Проверьте наличие ModuleManager в сцене
- Убедитесь, что UIModule инициализирован

---

**Готово!** Теперь вы можете использовать ScrollViewButtonController в своем проекте. 🎉
