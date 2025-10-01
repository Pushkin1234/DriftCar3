using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI представление для системы нитро.
/// Содержит все UI элементы и методы для их обновления.
/// </summary>
public class NitroView : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private Slider _nitroChargeBar;
    [SerializeField] private Button _nitroButton;
    [SerializeField] private TextMeshProUGUI _nitroStatusText;
    [SerializeField] private Image _nitroIcon;
    
    [Header("Additional Elements")]
    [SerializeField] private TextMeshProUGUI _nitroKeyHint;
    [SerializeField] private TextMeshProUGUI _nitroLevelText;
    [SerializeField] private GameObject _nitroActiveIndicator;
    [SerializeField] private Image _nitroProgressFill;
    
    [Header("Visual Effects")]
    [SerializeField] private Animator _nitroAnimator;
    [SerializeField] private ParticleSystem _uiNitroEffect;
    
    // Публичные свойства для доступа контроллера
    public Slider NitroChargeBar => _nitroChargeBar;
    public Button NitroButton => _nitroButton;
    public TextMeshProUGUI NitroStatusText => _nitroStatusText;
    public Image NitroIcon => _nitroIcon;
    public TextMeshProUGUI NitroKeyHint => _nitroKeyHint;
    public TextMeshProUGUI NitroLevelText => _nitroLevelText;
    public GameObject NitroActiveIndicator => _nitroActiveIndicator;
    public Image NitroProgressFill => _nitroProgressFill;
    public Animator NitroAnimator => _nitroAnimator;
    
    /// <summary>
    /// Обновляет отображение заряда нитро
    /// </summary>
    public void UpdateChargeDisplay(float charge, Color fillColor)
    {
        if (_nitroChargeBar != null)
        {
            _nitroChargeBar.value = charge;
        }
        
        if (_nitroProgressFill != null)
        {
            _nitroProgressFill.color = fillColor;
        }
    }
    
    /// <summary>
    /// Обновляет состояние кнопки нитро
    /// </summary>
    public void UpdateButtonState(bool interactable, Color buttonColor)
    {
        if (_nitroButton != null)
        {
            _nitroButton.interactable = interactable;
            
            var buttonImage = _nitroButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = buttonColor;
            }
        }
    }
    
    /// <summary>
    /// Обновляет текст статуса нитро
    /// </summary>
    public void UpdateStatusText(string statusText, Color textColor)
    {
        if (_nitroStatusText != null)
        {
            _nitroStatusText.text = statusText;
            _nitroStatusText.color = textColor;
        }
    }
    
    /// <summary>
    /// Обновляет иконку нитро
    /// </summary>
    public void UpdateIcon(Color iconColor)
    {
        if (_nitroIcon != null)
        {
            _nitroIcon.color = iconColor;
        }
    }
    
    /// <summary>
    /// Обновляет подсказку клавиши
    /// </summary>
    public void UpdateKeyHint(string keyText)
    {
        if (_nitroKeyHint != null)
        {
            _nitroKeyHint.text = keyText;
        }
    }
    
    /// <summary>
    /// Обновляет отображение уровня нитро
    /// </summary>
    public void UpdateNitroLevel(int level, int maxLevel)
    {
        if (_nitroLevelText != null)
        {
            _nitroLevelText.text = $"Nitro Level: {level}/{maxLevel}";
        }
    }
    
    /// <summary>
    /// Показывает/скрывает индикатор активного нитро
    /// </summary>
    public void SetActiveIndicator(bool active)
    {
        if (_nitroActiveIndicator != null)
        {
            _nitroActiveIndicator.SetActive(active);
        }
    }
    
    /// <summary>
    /// Запускает анимацию активации нитро
    /// </summary>
    public void PlayActivationAnimation()
    {
        if (_nitroAnimator != null)
        {
            _nitroAnimator.SetTrigger("Activate");
        }
        
        if (_uiNitroEffect != null)
        {
            _uiNitroEffect.Play();
        }
    }
    
    /// <summary>
    /// Запускает анимацию деактивации нитро
    /// </summary>
    public void PlayDeactivationAnimation()
    {
        if (_nitroAnimator != null)
        {
            _nitroAnimator.SetTrigger("Deactivate");
        }
        
        if (_uiNitroEffect != null)
        {
            _uiNitroEffect.Stop();
        }
    }
    
    /// <summary>
    /// Показывает/скрывает весь UI нитро
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
    
    /// <summary>
    /// Инициализирует UI элементы
    /// </summary>
    public void Initialize()
    {
        // Настраиваем прогресс-бар
        if (_nitroChargeBar != null)
        {
            _nitroChargeBar.minValue = 0f;
            _nitroChargeBar.maxValue = 1f;
            _nitroChargeBar.value = 1f;
        }
        
        // Скрываем индикатор активности по умолчанию
        SetActiveIndicator(false);
        
        // Останавливаем эффекты
        if (_uiNitroEffect != null)
        {
            _uiNitroEffect.Stop();
        }
    }
}
