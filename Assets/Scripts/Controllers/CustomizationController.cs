using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button[] _colorButtons;
    [SerializeField] private Button _previousWheelButton;
    [SerializeField] private Button _nextWheelButton;
    [SerializeField] private TextMeshProUGUI _wheelIndexText;
    [SerializeField] private Button _resetCustomizationButton;
    
    [Header("Configuration")]
    [SerializeField] private string _targetCarModelName = "SportsCar"; // Название модели машины
    [SerializeField] private Color[] _availableColors;
    
    private CustomizationModule _customizationModule;
    private DataModule _dataModule;
    private int _currentWheelIndex = 0;
    private int _maxWheelIndex = 5; // Максимальное количество типов колес
    
    private void Start()
    {
        _customizationModule = ModuleManager.Instance.GetModule<CustomizationModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        InitializeUI();
        LoadCurrentCustomization();
    }
    
    private void InitializeUI()
    {
        // Настраиваем кнопки цветов
        for (int i = 0; i < _colorButtons.Length && i < _availableColors.Length; i++)
        {
            int colorIndex = i; // Захватываем индекс для замыкания
            Color targetColor = _availableColors[i];
            
            // Устанавливаем цвет кнопки
            var buttonImage = _colorButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = targetColor;
            }
            
            // Добавляем обработчик клика
            _colorButtons[i].onClick.AddListener(() => OnColorButtonClicked(targetColor));
        }
        
        // Настраиваем кнопки колес
        if (_previousWheelButton != null)
            _previousWheelButton.onClick.AddListener(OnPreviousWheelClicked);
            
        if (_nextWheelButton != null)
            _nextWheelButton.onClick.AddListener(OnNextWheelClicked);
            
        // Кнопка сброса
        if (_resetCustomizationButton != null)
            _resetCustomizationButton.onClick.AddListener(OnResetCustomizationClicked);
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
        _currentWheelIndex--;
        if (_currentWheelIndex < 0)
            _currentWheelIndex = _maxWheelIndex;
            
        ApplyWheelChange();
    }
    
    private void OnNextWheelClicked()
    {
        _currentWheelIndex++;
        if (_currentWheelIndex > _maxWheelIndex)
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
        if (_wheelIndexText != null)
        {
            _wheelIndexText.text = $"Wheels: {_currentWheelIndex + 1}/{_maxWheelIndex + 1}";
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
    
    /// <summary>
    /// Устанавливает целевую модель машины для кастомизации
    /// </summary>
    public void SetTargetCar(string carModelName)
    {
        _targetCarModelName = carModelName;
        LoadCurrentCustomization();
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
