# 🔄 Руководство по сбросу сохранений

## 📋 Что добавлено

### 1. Метод `ResetData()` в DataModule
Сбрасывает все игровые данные к значениям по умолчанию:
- ✅ Монеты → 0
- ✅ Рекорд дрифта → 0
- ✅ Куплено машин → только 1-я (бесплатная)
- ✅ Кастомизация всех машин → сброшена
- ✅ Настройки → по умолчанию

### 2. Компонент `ResetSaveManager`
Обрабатывает нажатие клавиши "R" для сброса сохранений

---

## 🎮 Как использовать

### Настройка в Unity

1. **Добавьте компонент на GameController:**
   ```
   1. Выберите GameController в иерархии
   2. Add Component → ResetSaveManager
   3. Готово!
   ```

2. **Настройки в Inspector:**
   ```
   Reset Save Manager:
   ├─ Reset Key: R (клавиша для сброса)
   ├─ Require Confirmation: ✓ (требовать подтверждение)
   ├─ Reload Scene After Reset: ✓ (перезагрузить сцену)
   └─ Show Debug Messages: ✓ (показывать логи)
   ```

---

## ⌨️ Управление

### С подтверждением (по умолчанию):
1. Нажмите **R** → появится предупреждение в консоли
2. Нажмите **R** еще раз в течение 3 секунд → сброс выполнен
3. Если не нажать - сброс отменяется

### Без подтверждения:
1. Снимите галочку `Require Confirmation`
2. Нажмите **R** → сброс выполнен мгновенно

---

## 🔧 Настройки компонента

### Reset Key
**Клавиша для сброса**
- По умолчанию: `R`
- Можно изменить на любую клавишу (Delete, F12, и т.д.)

```csharp
[SerializeField] private KeyCode _resetKey = KeyCode.R;
```

### Require Confirmation
**Требовать подтверждение перед сбросом**
- ✅ Включено (безопасно) - нужно нажать R дважды
- ❌ Выключено (опасно!) - сброс сразу после нажатия

```csharp
[SerializeField] private bool _requireConfirmation = true;
```

### Reload Scene After Reset
**Перезагружать сцену после сброса**
- ✅ Включено - автоматически перезагружает текущую сцену
- ❌ Выключено - нужно перезагрузить вручную

```csharp
[SerializeField] private bool _reloadSceneAfterReset = true;
```

### Show Debug Messages
**Показывать сообщения в консоли**
- ✅ Включено - полезно для отладки
- ❌ Выключено - тихий режим

```csharp
[SerializeField] private bool _showDebugMessages = true;
```

---

## 💻 Использование из кода

### Сброс данных программно

```csharp
// Получаем DataModule
var dataModule = ModuleManager.Instance.GetModule<DataModule>();

// Сбрасываем все сохранения
dataModule.ResetData();

Debug.Log("Сохранения сброшены!");
```

### Сброс через ResetSaveManager

```csharp
// Получаем менеджер
var resetManager = FindObjectOfType<ResetSaveManager>();

// Вызываем сброс напрямую (обходит подтверждение)
resetManager.TestReset(); // Только в Editor
```

---

## 🔍 Отладка

### Проверка текущих сохранений

В Inspector компонента `ResetSaveManager`:
1. ПКМ → `Show Current Save Data`
2. В консоли появится информация:
   ```
   === Текущие сохранения ===
   Монеты: 5000
   Рекорд дрифта: 12500
   Выбранная машина: 2
   Куплено машин: 3/5
     Машина 0: ✅ Куплена
     Машина 1: ✅ Куплена
     Машина 2: ✅ Куплена
     Машина 3: ❌ Не куплена
     Машина 4: ❌ Не куплена
   ```

### Тестовый сброс из Editor

В Inspector компонента `ResetSaveManager`:
1. ПКМ → `Test - Reset Save Data`
2. Сохранения сбросятся без подтверждения

---

## 📊 Что происходит при сбросе

### 1. Очистка PlayerPrefs
```csharp
PlayerPrefs.DeleteAll();
PlayerPrefs.Save();
```

### 2. Создание данных по умолчанию
```csharp
_data = new GameData();
// coins = 0
// recordDriftScore = 0
// appliedCarIndex = 0
// isBuyShop = {true, false, false, false, false}
```

### 3. Очистка кастомизации машин
```csharp
for (int i = 0; i < 5; i++)
{
    PlayerPrefs.DeleteKey($"CarCustomization_{i}");
}
```

### 4. Сохранение новых данных
```csharp
SaveData();
```

### 5. Перезагрузка сцены (опционально)
```csharp
SceneManager.LoadScene(currentSceneName);
```

---

## ⚠️ Важные моменты

### 1. Сброс необратим!
- После сброса восстановить данные невозможно
- Всегда используйте подтверждение в релизной версии

### 2. Очищаются ВСЕ данные:
- ✅ Монеты и прогресс
- ✅ Куплено машины (кроме первой)
- ✅ Кастомизация всех машин
- ✅ Настройки игры
- ✅ Рекорды

### 3. Требует ModuleManager
- Компонент работает только если `ModuleManager.Instance` доступен
- Убедитесь что ModuleManager инициализирован в сцене

---

## 🎨 Добавление UI уведомления

Вы можете добавить визуальное подтверждение:

```csharp
private void ShowConfirmationUI()
{
    // Ваш UI менеджер
    UIManager.Instance.ShowMessage(
        "⚠️ Нажмите R еще раз для сброса всех сохранений!",
        duration: 3f,
        messageType: UIMessageType.Warning
    );
}

private void HideConfirmationUI()
{
    UIManager.Instance.HideMessage();
}
```

---

## 🛠️ Примеры использования

### Пример 1: Сброс в главном меню

```csharp
// На кнопке "Сбросить прогресс" в настройках
public void OnResetButtonClicked()
{
    // Показываем диалог подтверждения
    DialogManager.ShowDialog(
        "Сбросить весь прогресс?",
        "Это действие нельзя отменить!",
        onConfirm: () => 
        {
            var dataModule = ModuleManager.Instance.GetModule<DataModule>();
            dataModule.ResetData();
            
            // Возвращаемся в главное меню
            SceneManager.LoadScene("MainMenu");
        }
    );
}
```

### Пример 2: Чит-код для разработчиков

```csharp
private string _cheatCode = "";

private void Update()
{
    // Вводим "reset"
    foreach (char c in Input.inputString)
    {
        _cheatCode += c;
        
        if (_cheatCode.EndsWith("reset"))
        {
            Debug.Log("Чит-код активирован: сброс сохранений");
            
            var dataModule = ModuleManager.Instance.GetModule<DataModule>();
            dataModule.ResetData();
            
            _cheatCode = "";
        }
        
        // Ограничиваем длину
        if (_cheatCode.Length > 10)
            _cheatCode = _cheatCode.Substring(_cheatCode.Length - 10);
    }
}
```

### Пример 3: Автоматический сброс при запуске (для тестирования)

```csharp
public class AutoResetOnStart : MonoBehaviour
{
    [SerializeField] private bool _resetOnStart = false;
    
    private void Start()
    {
        #if UNITY_EDITOR
        if (_resetOnStart)
        {
            Debug.LogWarning("AUTO-RESET включен! Сбрасываем сохранения...");
            
            var dataModule = ModuleManager.Instance.GetModule<DataModule>();
            dataModule.ResetData();
        }
        #endif
    }
}
```

---

## 🔐 Защита от случайного сброса

### В релизной версии:

```csharp
#if !UNITY_EDITOR
    // Отключаем сброс в релизе
    private void Awake()
    {
        enabled = false;
        Debug.Log("ResetSaveManager отключен в релизной версии");
    }
#endif
```

### Или требуем специальную комбинацию:

```csharp
private void Update()
{
    // Ctrl + Shift + R для сброса
    if (Input.GetKey(KeyCode.LeftControl) && 
        Input.GetKey(KeyCode.LeftShift) && 
        Input.GetKeyDown(KeyCode.R))
    {
        PerformReset();
    }
}
```

---

## ✅ Готово!

Теперь в вашей игре есть:
- 🔄 Быстрый сброс сохранений по кнопке **R**
- ✅ Защита от случайного сброса (двойное нажатие)
- 🛠️ Удобные инструменты для отладки
- 📊 Возможность проверить текущие сохранения

**Для включения:** Добавьте `ResetSaveManager` на GameController в любой сцене!

