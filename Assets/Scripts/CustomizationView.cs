using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationView : MonoBehaviour
{
    [Header("Color Selection")]
    [SerializeField] private Button[] _colorButtons;
    [SerializeField] private Image _selectedColorIndicator;
    
    [Header("Wheel Selection")]
    [SerializeField] private Button _previousWheelButton;
    [SerializeField] private Button _nextWheelButton;
    [SerializeField] private TextMeshProUGUI _wheelNameText;
    [SerializeField] private Image _wheelPreviewImage;
    
    [Header("Actions")]
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _closeButton;
    
    [Header("Car Preview")]
    [SerializeField] private Transform _carPreviewParent;
    [SerializeField] private Camera _previewCamera;
    
    // Публичные свойства для доступа контроллера
    public Button[] ColorButtons => _colorButtons;
    public Button PreviousWheelButton => _previousWheelButton;
    public Button NextWheelButton => _nextWheelButton;
    public Button ApplyButton => _applyButton;
    public Button ResetButton => _resetButton;
    public Button CloseButton => _closeButton;
    
    public Image SelectedColorIndicator => _selectedColorIndicator;
    public TextMeshProUGUI WheelNameText => _wheelNameText;
    public Image WheelPreviewImage => _wheelPreviewImage;
    public Transform CarPreviewParent => _carPreviewParent;
    public Camera PreviewCamera => _previewCamera;
    
    /// <summary>
    /// Показывает индикатор выбранного цвета
    /// </summary>
    public void ShowSelectedColor(Color color)
    {
        if (_selectedColorIndicator != null)
        {
            _selectedColorIndicator.color = color;
            _selectedColorIndicator.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Обновляет отображение колес
    /// </summary>
    public void UpdateWheelDisplay(string wheelName, Sprite wheelSprite)
    {
        if (_wheelNameText != null)
            _wheelNameText.text = wheelName;
            
        if (_wheelPreviewImage != null && wheelSprite != null)
            _wheelPreviewImage.sprite = wheelSprite;
    }
    
    /// <summary>
    /// Показывает/скрывает панель кастомизации
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
