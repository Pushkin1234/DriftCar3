# 📋 КРАТКАЯ ИНСТРУКЦИЯ ПО НАСТРОЙКЕ

## ✅ Что уже сделано (автоматически):

1. ✅ **CustomizationView.cs** - создан в `Assets/Scripts/Views/`
2. ✅ **CustomizationController.cs** - обновлен в `Assets/Scripts/Controllers/`
3. ✅ **CarCustomizationApplier.cs** - создан в `Assets/Scripts/Services/`
4. ✅ **CustomizationModule.cs** - обновлен (удалена логика GameObject)
5. ✅ **PlayerView.cs** - обновлен (использует новый сервис)

## 🔧 Что нужно сделать вручную в Unity:

### Шаг 1: Обновить TuningCanvas (панель кастомизации)

#### 1.1 Удалить старый компонент (если есть)
```
1. Выберите TuningCanvas в иерархии
2. В Inspector найдите старый контроллер
3. Нажмите "Remove Component"
```

#### 1.2 Добавить CustomizationView
```
1. Выберите TuningCanvas
2. Add Component → CustomizationView
3. Назначьте ВСЕ UI элементы в Inspector:

Main Buttons:
├─ Color Button: [ваша кнопка ColorButton]
├─ Wheels Button: [ваша кнопка WeelsButton]
├─ Upgrade Button: [ваша кнопка UpGradeButton]
├─ Spoiler Button: [ваша кнопка SpoilerButton]
└─ Exit Button: [ваша кнопка ExitButton]

Scroll Views:
├─ Color Scroll View: [Scroll View Color]
├─ Wheels Scroll View: [Scroll View Weels]
├─ Upgrade Scroll View: [Scroll View UpGrade]
└─ Spoiler Scroll View: [Scroll View Spoiler]

Color UI:
├─ Color Buttons: [массив кнопок цветов]
├─ Color Price Text: [текст для цены]
├─ Purchase Color Button: [кнопка "Купить"]
├─ Select Color Button: [кнопка "Выбрать"]
└─ Color Lock Icon: [иконка замка]

... (аналогично для Wheels, Upgrade, Spoiler)

Bottom Actions:
├─ Back Button: [НАЗАД]
└─ Select Button: [ВЫБРАТЬ]
```

#### 1.3 Добавить новый CustomizationController
```
1. Выберите TuningCanvas
2. Add Component → CustomizationController
3. В Inspector:
   └─ View: [перетащите сюда сам TuningCanvas с компонентом CustomizationView]
```

### Шаг 2: Проверить GameController
```
Убедитесь что на GameController есть:
- ModuleManager
- CustomizationModule
- DataModule
```

### Шаг 3: Тестирование
```
1. Запустите игру
2. Откройте панель тюнинга
3. Проверьте:
   ✓ Переключение между панелями работает
   ✓ Выбор цветов/колес/улучшений работает
   ✓ Цены отображаются корректно
   ✓ Кнопка "Купить" сменяется на "Выбрать" после покупки
   ✓ На сцене Level машина имеет все сохраненные улучшения
```

## 📊 Итоговая структура:

```
TuningCanvas (GameObject)
├─ CustomizationView (MonoBehaviour)
│   └─ [все UI элементы назначены]
└─ CustomizationController (MonoBehaviour)
    └─ View: TuningCanvas [ссылка]
```

## ❓ Если что-то не работает:

### Проблема: "CustomizationModule not found"
```
Решение:
1. Проверьте что на GameController есть CustomizationModule
2. Убедитесь что ModuleManager.Initialize() вызывается
```

### Проблема: "NullReferenceException в CustomizationView"
```
Решение:
1. Проверьте что ВСЕ UI элементы назначены в Inspector
2. Некоторые элементы могут быть null - это нормально, они опциональны
```

### Проблема: "Машина на Level не имеет кастомизации"
```
Решение:
1. Проверьте что на сцене Level есть PlayerView
2. Убедитесь что массив _cars содержит все 5 машин
3. Проверьте что CustomizationModule инициализирован
```

## 🎯 Готово!

После настройки у вас будет:
- ✅ Чистая архитектура (MVC + Service)
- ✅ Разделение ответственности
- ✅ Легко расширяемая система
- ✅ Минимум дублирования кода
- ✅ Легко тестируемая логика

---

**Для подробной информации см.:**
- `Architecture_Refactoring_Analysis.md` - анализ изменений
- `Architecture_Diagram.md` - диаграмма архитектуры
- `Refactoring_Complete.md` - полная документация
