# 🎯 Автоматическое закрытие панелей - Настройка

## ✨ Что добавлено

Теперь при нажатии на кнопку характеристики **автоматически закрываются другие панели** в той же группе! Это создает эффект "аккордеона" - только одна панель может быть открыта одновременно.

## 🔧 Как настроить

### Шаг 1: Настройка панелей в инспекторе

Для каждой панели ScrollViewButtonController:

```
Auto Close Others: ✓ (включить автоматическое закрытие)
Panel Group: "Characteristics" (название группы)
```

### Шаг 2: Пример настройки для характеристик машины

```
EnginePanel (ScrollViewButtonController):
├── Auto Close Others: ✓
├── Panel Group: "Characteristics"
└── Scroll Behavior: Toggle

BrakePanel (ScrollViewButtonController):
├── Auto Close Others: ✓
├── Panel Group: "Characteristics"
└── Scroll Behavior: Toggle

HandlingPanel (ScrollViewButtonController):
├── Auto Close Others: ✓
├── Panel Group: "Characteristics"
└── Scroll Behavior: Toggle

NitroPanel (ScrollViewButtonController):
├── Auto Close Others: ✓
├── Panel Group: "Characteristics"
└── Scroll Behavior: Toggle
```

## 🎮 Как это работает

1. **Пользователь нажимает** на кнопку "Двигатель"
2. **Открывается панель** двигателя
3. **Автоматически закрываются** все другие панели в группе "Characteristics"
4. **Результат**: открыта только панель двигателя

## 📋 Примеры групп

### Группа "Characteristics" (характеристики машины):
- Engine Panel (Двигатель)
- Brake Panel (Тормоза)
- Handling Panel (Управление)
- Nitro Panel (Нитро)

### Группа "Settings" (настройки игры):
- Graphics Panel (Графика)
- Audio Panel (Звук)
- Controls Panel (Управление)

### Группа "Shop" (магазин):
- Cars Panel (Машины)
- Upgrades Panel (Улучшения)
- Skins Panel (Скины)

## 💻 Программное управление

```csharp
// Получение UIModule
UIModule uiModule = ModuleManager.Instance.GetModule<UIModule>();

// Закрытие всех панелей в группе
uiModule.CloseOthersInGroup("Characteristics", null);

// Получение всех панелей в группе
List<ScrollViewButtonController> panels = uiModule.GetControllersInGroup("Characteristics");

// Получение всех групп
Dictionary<string, List<ScrollViewButtonController>> allGroups = uiModule.GetAllPanelGroups();
```

## ⚙️ Настройки в инспекторе

### Auto Close Others:
- **✓ Включено** - автоматически закрывает другие панели в группе
- **✗ Выключено** - панели работают независимо

### Panel Group:
- **"Default"** - группа по умолчанию
- **"Characteristics"** - для характеристик машины
- **"Settings"** - для настроек игры
- **"Shop"** - для магазина
- **Любое название** - для создания собственных групп

## 🎯 Пример использования

### Сценарий: Характеристики машины

1. **Создайте 4 панели**:
   - EnginePanel
   - BrakePanel  
   - HandlingPanel
   - NitroPanel

2. **Настройте каждую панель**:
   - Auto Close Others: ✓
   - Panel Group: "Characteristics"

3. **Результат**:
   - При нажатии на "Двигатель" → открывается панель двигателя, остальные закрываются
   - При нажатии на "Тормоза" → открывается панель тормозов, двигатель закрывается
   - И так далее...

## 🔄 Совместимость

- **Работает с существующими** ScrollViewButtonController
- **Не влияет на панели** с Auto Close Others = ✗
- **Поддерживает разные группы** одновременно
- **Интегрируется с UIModule** автоматически

## 🐛 Отладка

### Проверка групп:
```csharp
// В консоли Unity
[ContextMenu("Show Group Info")]
public void ShowGroupInfo()
{
    UIModule uiModule = ModuleManager.Instance.GetModule<UIModule>();
    var groups = uiModule.GetAllPanelGroups();
    
    foreach (var group in groups)
    {
        Debug.Log($"Группа: {group.Key}, Панелей: {group.Value.Count}");
    }
}
```

### Логи в консоли:
- "Зарегистрирован ScrollView контроллер: [Name] в группе: [Group]"
- "Отменена регистрация ScrollView контроллера: [Name]"

---

**Готово!** Теперь ваши панели характеристик будут автоматически закрывать друг друга! 🎉
