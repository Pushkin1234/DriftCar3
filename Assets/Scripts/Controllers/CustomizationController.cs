using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationController : MonoBehaviour
{
    [Header("View Reference")]
    [SerializeField] private CustomizationView _view;
    
    [Header("Configuration")]
    [SerializeField] private string _targetCarModelName = "SportsCar"; // Название модели машины
    [SerializeField] private Color[] _availableColors;
    
    private CustomizationModule _customizationModule;
    private DataModule _dataModule;
    private int _currentWheelIndex = 0;
    
    private void Start()
    {
        _customizationModule = ModuleManager.Instance.GetModule<CustomizationModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        InitializeUI();
        LoadCurrentCustomization();
    }
    
    private void InitializeUI()
    {
        if (_view == null)
        {
            Debug.LogError("CustomizationView is not assigned!");
            return;
        }
        
        // Настраиваем кнопки цветов через View
        for (int i = 0; i < _view.ColorButtons.Length && i < _availableColors.Length; i++)
        {
            int colorIndex = i; // Захватываем индекс для замыкания
            Color targetColor = _availableColors[i];
            
            // Устанавливаем цвет кнопки
            var buttonImage = _view.ColorButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = targetColor;
            }
            
            // Добавляем обработчик клика
            _view.ColorButtons[i].onClick.AddListener(() => OnColorButtonClicked(targetColor));
        }
        
        // Настраиваем кнопки колес через View
        if (_view.PreviousWheelButton != null)
            _view.PreviousWheelButton.onClick.AddListener(OnPreviousWheelClicked);
            
        if (_view.NextWheelButton != null)
            _view.NextWheelButton.onClick.AddListener(OnNextWheelClicked);
            
        // Кнопка сброса через View
        if (_view.ResetButton != null)
            _view.ResetButton.onClick.AddListener(OnResetCustomizationClicked);
            
        // Кнопка закрытия через View
        if (_view.CloseButton != null)
            _view.CloseButton.onClick.AddListener(OnCloseClicked);
    }
    
    private void LoadCurrentCustomization()
    {
        if (_customizationModule == null) return;
        
        var customization = _customizationModule.GetCarCustomization(_targetCarModelName);
        _currentWheelIndex = customization.selectedWheelIndex;
        
        UpdateWheelUI();
    }
    
    private void OnColorButtonClicked(Color selectedColor)
    {
        if (_customizationModule == null) return;
        
        // Применяем цвет к машине
        _customizationModule.PaintCar(_targetCarModelName, selectedColor);
        
        Debug.Log($"Car {_targetCarModelName} painted with color: {selectedColor}");
    }
    
    private void OnPreviousWheelClicked()
    {
        if (_customizationModule == null) return;
        
        _currentWheelIndex--;
        if (_currentWheelIndex < 0)
            _currentWheelIndex = _customizationModule.GetWheelCount() - 1;
            
        ApplyWheelChange();
    }
    
    private void OnNextWheelClicked()
    {
        if (_customizationModule == null) return;
        
        _currentWheelIndex++;
        if (_currentWheelIndex >= _customizationModule.GetWheelCount())
            _currentWheelIndex = 0;
            
        ApplyWheelChange();
    }
    
    private void ApplyWheelChange()
    {
        if (_customizationModule == null) return;
        
        _customizationModule.ChangeWheels(_targetCarModelName, _currentWheelIndex);
        UpdateWheelUI();
        
        Debug.Log($"Car {_targetCarModelName} wheels changed to index: {_currentWheelIndex}");
    }
    
    private void UpdateWheelUI()
    {
        if (_customizationModule == null || _view == null) return;
        
        var wheelData = _customizationModule.GetWheelData(_currentWheelIndex);
        if (wheelData != null)
        {
            string wheelName = wheelData.wheelName;
            bool isUnlocked = wheelData.isUnlocked;
            int totalWheels = _customizationModule.GetWheelCount();
            
            string status = isUnlocked ? "✓" : $"🔒 {wheelData.price}💰";
            string displayText = $"{wheelName} ({_currentWheelIndex + 1}/{totalWheels}) {status}";
            
            // Обновляем UI через View
            _view.UpdateWheelDisplay(displayText, wheelData.wheelIcon);
        }
    }
    
    private void OnResetCustomizationClicked()
    {
        if (_customizationModule == null) return;
        
        // Сбрасываем к стандартным настройкам
        _customizationModule.PaintCar(_targetCarModelName, Color.white);
        _customizationModule.ChangeWheels(_targetCarModelName, 0);
        
        _currentWheelIndex = 0;
        UpdateWheelUI();
        
        Debug.Log($"Car {_targetCarModelName} customization reset to default");
    }
    
    private void OnCloseClicked()
    {
        // Закрываем панель кастомизации
        if (_view != null)
        {
            _view.SetVisible(false);
        }
        
        Debug.Log("Customization panel closed");
    }
    
    /// <summary>
    /// Устанавливает целевую модель машины для кастомизации
    /// </summary>
    public void SetTargetCar(string carModelName)
    {
        _targetCarModelName = carModelName;
        LoadCurrentCustomization();
    }
    
    /// <summary>
    /// Показывает панель кастомизации
    /// </summary>
    public void ShowCustomizationPanel()
    {
        if (_view != null)
        {
            _view.SetVisible(true);
            LoadCurrentCustomization();
        }
    }
    
    /// <summary>
    /// Применяет сохраненную кастомизацию к заспавненной машине
    /// </summary>
    public void ApplyCustomizationToSpawnedCar(GameObject carObject)
    {
        if (_customizationModule == null) return;
        
        _customizationModule.ApplyCustomizationToCar(carObject, _targetCarModelName);
    }
}
