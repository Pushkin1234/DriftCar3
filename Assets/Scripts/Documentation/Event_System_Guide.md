# 🎯 Руководство по работе с событиями в CustomizationController

## 📚 Оглавление
1. [Основная концепция](#основная-концепция)
2. [Как это работает](#как-это-работает)
3. [Примеры использования](#примеры-использования)
4. [Добавление собственных событий](#добавление-собственных-событий)
5. [Отладка событий](#отладка-событий)

---

## Основная концепция

### Event-Driven архитектура (управление событиями)

```
┌─────────────────┐         События          ┌──────────────────────┐
│                 │ ──────────────────────> │                      │
│ CustomizationView│                         │ CustomizationController│
│  (UI Layer)     │ <────────────────────── │   (Logic Layer)      │
└─────────────────┘      Обновления UI       └──────────────────────┘
                                                      │
                                                      ▼
                                              ┌─────────────────┐
                                              │ CustomizationModule│
                                              │  (Data Layer)   │
                                              └─────────────────┘
```

### Преимущества:
✅ **Разделение ответственности** - View не знает о логике  
✅ **Легко тестировать** - можно тестировать Controller отдельно  
✅ **Переиспользование** - один View может работать с разными контроллерами  
✅ **Масштабируемость** - легко добавлять новые события

---

## Как это работает

### 1. View объявляет события

```csharp
// В CustomizationView.cs
public class CustomizationView : MonoBehaviour
{
    // Объявляем события
    public event Action<int> OnColorSelected;        // Выбран цвет (индекс)
    public event Action OnColorPurchaseRequested;    // Запрос на покупку цвета
    public event Action OnColorSelectRequested;      // Запрос на применение цвета
    
    private void SetupButtons()
    {
        // При нажатии на кнопку - вызываем событие
        for (int i = 0; i < _colorButtons.Length; i++)
        {
            int index = i; // Важно! Захватываем индекс
            _colorButtons[i]?.onClick.AddListener(() => 
            {
                // Генерируем событие
                OnColorSelected?.Invoke(index);
            });
        }
        
        _purchaseColorButton?.onClick.AddListener(() => 
        {
            OnColorPurchaseRequested?.Invoke();
        });
    }
}
```

### 2. Controller подписывается на события

```csharp
// В CustomizationController.cs
public class CustomizationController : MonoBehaviour
{
    [SerializeField] private CustomizationView _view;
    
    private void Start()
    {
        // Подписываемся на события View
        SubscribeToViewEvents();
    }
    
    private void SubscribeToViewEvents()
    {
        if (_view == null) return;
        
        // Подписка: когда View генерирует событие, вызывается наш метод
        _view.OnColorSelected += HandleColorSelection;
        _view.OnColorPurchaseRequested += HandleColorPurchase;
        _view.OnColorSelectRequested += HandleColorSelect;
    }
    
    // Обработчики событий
    private void HandleColorSelection(int colorIndex)
    {
        Debug.Log($"Пользователь выбрал цвет: {colorIndex}");
        
        // Применяем логику через Module
        string carName = $"Car_{_currentCarIndex}";
        _customizationModule.SelectColor(carName, colorIndex);
        
        // Обновляем UI через View
        RefreshColorPanel();
    }
    
    private void HandleColorPurchase()
    {
        Debug.Log("Пользователь нажал 'Купить'");
        
        string carName = $"Car_{_currentCarIndex}";
        bool success = _customizationModule.PurchaseColor(carName, _selectedColorIndex);
        
        if (success)
        {
            Debug.Log("Покупка успешна!");
            RefreshColorPanel();
        }
        else
        {
            Debug.LogWarning("Недостаточно монет!");
        }
    }
}
```

### 3. Отписка от событий (важно!)

```csharp
private void OnDestroy()
{
    // ОБЯЗАТЕЛЬНО отписываемся при уничтожении!
    UnsubscribeFromViewEvents();
}

private void UnsubscribeFromViewEvents()
{
    if (_view == null) return;
    
    _view.OnColorSelected -= HandleColorSelection;
    _view.OnColorPurchaseRequested -= HandleColorPurchase;
    _view.OnColorSelectRequested -= HandleColorSelect;
}
```

---

## Примеры использования

### Пример 1: Обработка выбора колеса

```csharp
// В CustomizationView.cs
public event Action<int> OnWheelSelected;

private void SetupWheelButtons()
{
    for (int i = 0; i < _wheelButtons.Length; i++)
    {
        int wheelIndex = i;
        _wheelButtons[i]?.onClick.AddListener(() => 
        {
            // Генерируем событие
            OnWheelSelected?.Invoke(wheelIndex);
        });
    }
}

// В CustomizationController.cs
private void SubscribeToViewEvents()
{
    _view.OnWheelSelected += HandleWheelSelection;
}

private void HandleWheelSelection(int wheelIndex)
{
    _selectedWheelIndex = wheelIndex;
    
    Debug.Log($"Выбрано колесо: {wheelIndex}");
    
    // Обновляем UI
    RefreshWheelsPanel();
}
```

### Пример 2: Цепочка событий (покупка → применение)

```csharp
private void HandleColorPurchase()
{
    string carName = $"Car_{_currentCarIndex}";
    bool success = _customizationModule.PurchaseColor(carName, _selectedColorIndex);
    
    if (success)
    {
        // 1. Обновляем UI
        RefreshColorPanel();
        
        // 2. Автоматически применяем купленный цвет
        var colorData = _customizationModule.GetColorData(_selectedColorIndex);
        if (colorData != null)
        {
            _customizationModule.PaintCar(carName, colorData.color);
        }
        
        // 3. Сохраняем изменения
        SaveCarCustomization();
        
        Debug.Log("Цвет куплен и применен!");
    }
}
```

### Пример 3: Переключение панелей

```csharp
// В CustomizationView.cs
public event Action OnColorPanelRequested;
public event Action OnWheelsPanelRequested;

private void SetupButtons()
{
    _colorButton?.onClick.AddListener(() => OnColorPanelRequested?.Invoke());
    _wheelsButton?.onClick.AddListener(() => OnWheelsPanelRequested?.Invoke());
}

// В CustomizationController.cs
private void HandleColorPanelRequest()
{
    // 1. Показываем панель цветов
    _view.ShowPanel(CustomizationView.PanelType.Color);
    
    // 2. Обновляем данные панели
    RefreshColorPanel();
    
    Debug.Log("Открыта панель цветов");
}

private void HandleWheelsPanelRequest()
{
    _view.ShowPanel(CustomizationView.PanelType.Wheels);
    RefreshWheelsPanel();
    
    Debug.Log("Открыта панель колес");
}
```

---

## Добавление собственных событий

### Пример: Добавить событие "Предпросмотр изменений"

#### Шаг 1: Объявить событие в View

```csharp
// CustomizationView.cs
public class CustomizationView : MonoBehaviour
{
    // Добавляем новое событие
    public event Action OnPreviewRequested;
    
    [SerializeField] private Button _previewButton;
    
    private void SetupButtons()
    {
        // Подключаем кнопку к событию
        _previewButton?.onClick.AddListener(() => 
        {
            OnPreviewRequested?.Invoke();
        });
    }
}
```

#### Шаг 2: Подписаться в Controller

```csharp
// CustomizationController.cs
private void SubscribeToViewEvents()
{
    // ... существующие подписки ...
    
    // Добавляем подписку на новое событие
    _view.OnPreviewRequested += HandlePreviewRequest;
}

private void UnsubscribeFromViewEvents()
{
    // ... существующие отписки ...
    
    _view.OnPreviewRequested -= HandlePreviewRequest;
}

// Обработчик нового события
private void HandlePreviewRequest()
{
    Debug.Log("Показываем предпросмотр изменений");
    
    // Ваша логика предпросмотра
    ShowPreview();
}

private void ShowPreview()
{
    // Например, можно показать 3D модель машины с изменениями
    Debug.Log("Предпросмотр: цвет, колеса, спойлер");
}
```

---

## Отладка событий

### 1. Логирование событий

```csharp
private void HandleColorSelection(int colorIndex)
{
    Debug.Log($"[EVENT] OnColorSelected вызвано с индексом: {colorIndex}");
    Debug.Log($"[EVENT] Время: {Time.time}");
    Debug.Log($"[EVENT] Вызывающий объект: {_view.name}");
    
    // Ваша логика...
}
```

### 2. Проверка подписчиков

```csharp
private void SubscribeToViewEvents()
{
    if (_view == null)
    {
        Debug.LogError("View == null! События не подключены!");
        return;
    }
    
    _view.OnColorSelected += HandleColorSelection;
    Debug.Log($"Подписались на OnColorSelected. Текущий View: {_view.name}");
}
```

### 3. Отслеживание утечек памяти

```csharp
private void OnDestroy()
{
    Debug.Log("Отписываемся от всех событий View");
    UnsubscribeFromViewEvents();
    
    // Проверяем что отписались
    if (_view != null)
    {
        Debug.Log("View все еще существует, но мы отписались");
    }
}
```

---

## Частые ошибки и решения

### ❌ Ошибка 1: Забыли отписаться

```csharp
// ПЛОХО: утечка памяти!
private void OnDestroy()
{
    // Забыли вызвать UnsubscribeFromViewEvents()
}

// ХОРОШО:
private void OnDestroy()
{
    UnsubscribeFromViewEvents(); // Обязательно!
}
```

### ❌ Ошибка 2: Подписываемся несколько раз

```csharp
// ПЛОХО: обработчик вызовется 2 раза!
private void Start()
{
    _view.OnColorSelected += HandleColorSelection;
    _view.OnColorSelected += HandleColorSelection; // Дубликат!
}

// ХОРОШО: подписываемся только один раз
private void Start()
{
    SubscribeToViewEvents();
}
```

### ❌ Ошибка 3: Не проверили на null

```csharp
// ПЛОХО: NullReferenceException!
_view.OnColorSelected += HandleColorSelection;

// ХОРОШО: проверяем
if (_view != null)
{
    _view.OnColorSelected += HandleColorSelection;
}

// ЕЩЕ ЛУЧШЕ: используем оператор ?.
_view?.OnColorSelected?.Invoke(index);
```

---

## Диаграмма последовательности событий

```
Пользователь  │  View          │  Controller      │  Module         │  View
    │         │                │                  │                 │
    │─────────┼───────────────>│                  │                 │
    │  Клик   │                │                  │                 │
    │         │                │                  │                 │
    │         │  OnColorSelected(5)               │                 │
    │         │────────────────>│                  │                 │
    │         │                │                  │                 │
    │         │                │  PurchaseColor() │                 │
    │         │                │─────────────────>│                 │
    │         │                │                  │                 │
    │         │                │  success = true  │                 │
    │         │                │<─────────────────│                 │
    │         │                │                  │                 │
    │         │                │  UpdateColorUI() │                 │
    │         │                │─────────────────────────────────────>│
    │         │                │                  │                 │
    │         │                │                  │                 │  UI
    │         │<───────────────────────────────────────────────────────┤ обновлен
    │  Цвет   │                │                  │                 │
    │ изменен │                │                  │                 │
```

---

## Сравнение со старым подходом

### Старый подход (без событий):
```csharp
// CustomizationController.cs - СТАРЫЙ СПОСОБ
private void Update()
{
    // Проверяем кнопки каждый кадр - неэффективно!
    if (Input.GetMouseButtonDown(0))
    {
        CheckIfColorButtonClicked();
        CheckIfWheelButtonClicked();
        CheckIfPurchaseButtonClicked();
        // ... много проверок
    }
}
```

### Новый подход (с событиями):
```csharp
// CustomizationController.cs - НОВЫЙ СПОСОБ
private void SubscribeToViewEvents()
{
    // Подписались один раз - работает всегда!
    _view.OnColorSelected += HandleColorSelection;
    _view.OnWheelSelected += HandleWheelSelection;
    _view.OnPurchaseRequested += HandlePurchase;
}
```

**Преимущества нового подхода:**
- ⚡ **Производительность**: события вызываются только при необходимости
- 🧹 **Чистый код**: нет проверок в Update()
- 📦 **Модульность**: легко добавлять/удалять обработчики
- 🐛 **Легко отлаживать**: видно что и когда вызывается

---

## Практические задачи

### Задача 1: Добавить подтверждение покупки

```csharp
// Решение:

// 1. В CustomizationView добавить событие
public event Action<int> OnPurchaseConfirmationRequested;

// 2. В Controller подписаться и обработать
private void HandlePurchaseConfirmation(int price)
{
    Debug.Log($"Подтвердить покупку за {price} монет?");
    
    // Показываем диалог подтверждения
    bool confirmed = ShowConfirmationDialog($"Купить за {price}?");
    
    if (confirmed)
    {
        HandleColorPurchase();
    }
}
```

### Задача 2: Добавить звуковой эффект при покупке

```csharp
// Решение:

private void HandleColorPurchase()
{
    bool success = _customizationModule.PurchaseColor(...);
    
    if (success)
    {
        // Воспроизводим звук покупки
        AudioManager.PlaySound("purchase_success");
        
        RefreshColorPanel();
    }
    else
    {
        // Воспроизводим звук ошибки
        AudioManager.PlaySound("purchase_failed");
    }
}
```

---

## Заключение

### Ключевые моменты:

1. ✅ **События = связь между View и Controller**
2. ✅ **View генерирует события, Controller обрабатывает**
3. ✅ **Всегда отписывайтесь в OnDestroy()**
4. ✅ **Проверяйте на null перед вызовом**
5. ✅ **Используйте осмысленные имена событий**

### Полезные ссылки:

- [C# Events и Delegates](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/)
- [Unity Event System](https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html)
- [MVC Pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller)

---

**Готово! Теперь вы знаете как работать с событиями в CustomizationController! 🎉**

