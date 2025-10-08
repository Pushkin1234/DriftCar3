# 📜 ScrollViewButtonController - Документация

## 🎯 Описание

`ScrollViewButtonController` - это универсальный скрипт для управления Scroll View при нажатии на кнопку, интегрированный с модульной архитектурой проекта DriftCar3.

## 🚀 Основные возможности

- **Переключение видимости** Scroll View по нажатию кнопки
- **Плавная анимация** прокрутки с использованием DOTween
- **Различные типы поведения** прокрутки (Toggle, Scroll Up/Down/Left/Right, Custom)
- **Визуальная обратная связь** при нажатии на кнопку
- **Интеграция с UIModule** для централизованного управления
- **Настраиваемые параметры** анимации и поведения

## 📋 Настройка компонента

### Обязательные поля:
- **Scroll Rect** - ссылка на компонент ScrollRect
- **Control Button** - кнопка для управления Scroll View

### Настройки анимации:
- **Animation Duration** - длительность анимации (по умолчанию 0.5 сек)
- **Animation Ease** - тип анимации (по умолчанию OutCubic)
- **Smooth Scrolling** - включить плавную прокрутку

### Настройки поведения:
- **Scroll Behavior** - тип поведения:
  - `Toggle` - переключение видимости
  - `ScrollUp` - прокрутка вверх
  - `ScrollDown` - прокрутка вниз
  - `ScrollLeft` - прокрутка влево
  - `ScrollRight` - прокрутка вправо
  - `Custom` - пользовательское поведение
- **Scroll Amount** - количество прокрутки (0.0 - 1.0)
- **Invert Direction** - инвертировать направление

### Визуальная обратная связь:
- **Show Visual Feedback** - показывать визуальную обратную связь
- **Active Color** - цвет активного состояния
- **Inactive Color** - цвет неактивного состояния
- **Feedback Duration** - длительность обратной связи

## 🔧 Использование

### Базовое использование:

1. **Добавьте компонент** `ScrollViewButtonController` на GameObject
2. **Назначьте Scroll Rect** и Control Button в инспекторе
3. **Настройте параметры** по необходимости
4. **Скрипт автоматически** зарегистрируется в UIModule

### Программное управление:

```csharp
// Получение ссылки на контроллер
ScrollViewButtonController controller = GetComponent<ScrollViewButtonController>();

// Переключение состояния
controller.ToggleScrollViewProgrammatically();

// Установка состояния
controller.SetScrollViewActive(true);

// Прокрутка к определенным позициям
controller.ScrollToTop();
controller.ScrollToBottom();
controller.ScrollToLeft();
controller.ScrollToRight();
```

### Централизованное управление через UIModule:

```csharp
// Получение UIModule
UIModule uiModule = ModuleManager.Instance.GetModule<UIModule>();

// Закрытие всех Scroll View
uiModule.CloseAllScrollViews();

// Открытие Scroll View по имени
uiModule.OpenScrollViewByName("MyScrollView");

// Получение всех контроллеров
List<ScrollViewButtonController> controllers = uiModule.GetScrollViewControllers();
```

## 🎨 Специализированные контроллеры

### ShopScrollViewController

Специализированный контроллер для магазина с дополнительными возможностями:

- **Автоматическое закрытие** при выборе товара
- **Прокрутка к определенному товару**
- **События открытия/закрытия** магазина
- **Интеграция с ShopModule**

```csharp
// Прокрутка к определенному товару
shopScrollViewController.ScrollToItem(itemIndex);
```

## 📝 Примеры использования

### Пример 1: Простое переключение

```csharp
// Настройка в инспекторе:
// Scroll Behavior: Toggle
// Smooth Scrolling: true
// Animation Duration: 0.5
```

### Пример 2: Прокрутка по направлению

```csharp
// Настройка в инспекторе:
// Scroll Behavior: ScrollDown
// Scroll Amount: 0.3
// Smooth Scrolling: true
```

### Пример 3: Пользовательское поведение

```csharp
public class CustomScrollController : ScrollViewButtonController
{
    protected override void OnCustomScrollBehavior()
    {
        // Ваша логика здесь
        Debug.Log("Пользовательское поведение активировано!");
        
        // Например, прокрутка к определенному элементу
        ScrollToPosition(new Vector2(0.5f, 0.5f));
    }
}
```

## 🎯 Интеграция с архитектурой проекта

### Модульная архитектура:

1. **UIModule** - централизованное управление всеми Scroll View
2. **ModuleManager** - доступ к модулям через синглтон
3. **BaseGameModule** - базовый класс для всех модулей

### Паттерны проекта:

- **MVC Pattern** - контроллеры отделены от представлений
- **Module Pattern** - функциональность разделена на модули
- **Event-Driven** - использование событий для связи компонентов

## ⚠️ Важные моменты

1. **DOTween** - скрипт использует DOTween для анимаций. Убедитесь, что он подключен к проекту
2. **ModuleManager** - скрипт автоматически регистрируется в UIModule при старте
3. **Производительность** - анимации оптимизированы для WebGL
4. **Совместимость** - работает с Unity UI (uGUI)

## 🐛 Отладка

### Контекстное меню (только в редакторе):

- **Test Toggle** - тестирование переключения
- **Test Scroll Up** - тестирование прокрутки вверх
- **Test Scroll Down** - тестирование прокрутки вниз

### Логи:

Скрипт выводит подробные логи в консоль для отладки:
- Регистрация в UIModule
- Ошибки инициализации
- Предупреждения о недоступных модулях

## 🔄 Обновления и расширения

### Добавление новых типов поведения:

1. Расширьте enum `ScrollBehavior`
2. Добавьте обработку в `ExecuteScrollBehavior()`
3. Реализуйте соответствующую логику

### Интеграция с другими модулями:

```csharp
// Пример интеграции с DataModule
private void OnButtonClicked()
{
    base.OnButtonClicked(); // Вызов базовой логики
    
    // Дополнительная логика
    DataModule dataModule = ModuleManager.Instance.GetModule<DataModule>();
    if (dataModule != null)
    {
        // Ваша логика здесь
    }
}
```

## 📊 Производительность

- **Оптимизированные анимации** - использование DOTween
- **Умные обновления** - обновление только при изменении
- **Кэширование ссылок** - избежание повторных поисков компонентов
- **WebGL совместимость** - оптимизация для веб-платформы
