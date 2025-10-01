using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeView : MonoBehaviour
{
    [Header("Upgrade Buttons")]
    [SerializeField] private Button _engineUpgradeButton;
    [SerializeField] private Button _brakeUpgradeButton;
    [SerializeField] private Button _handlingUpgradeButton;
    
    [Header("Level Displays")]
    [SerializeField] private TextMeshProUGUI _engineLevelText;
    [SerializeField] private TextMeshProUGUI _brakeLevelText;
    [SerializeField] private TextMeshProUGUI _handlingLevelText;
    
    [Header("Cost Displays")]
    [SerializeField] private TextMeshProUGUI _engineCostText;
    [SerializeField] private TextMeshProUGUI _brakeCostText;
    [SerializeField] private TextMeshProUGUI _handlingCostText;
    
    [Header("Progress Bars")]
    [SerializeField] private Slider _engineProgressBar;
    [SerializeField] private Slider _brakeProgressBar;
    [SerializeField] private Slider _handlingProgressBar;
    
    [Header("Stats Displays")]
    [SerializeField] private TextMeshProUGUI _engineStatsText;
    [SerializeField] private TextMeshProUGUI _brakeStatsText;
    [SerializeField] private TextMeshProUGUI _handlingStatsText;
    
    [Header("General UI")]
    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    [SerializeField] private TextMeshProUGUI _carNameText;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _closeButton;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem _upgradeEffect;
    [SerializeField] private AudioSource _upgradeSound;
    
    // Публичные свойства для доступа контроллера
    public Button EngineUpgradeButton => _engineUpgradeButton;
    public Button BrakeUpgradeButton => _brakeUpgradeButton;
    public Button HandlingUpgradeButton => _handlingUpgradeButton;
    public Button ResetButton => _resetButton;
    public Button CloseButton => _closeButton;
    
    public TextMeshProUGUI EngineLevelText => _engineLevelText;
    public TextMeshProUGUI BrakeLevelText => _brakeLevelText;
    public TextMeshProUGUI HandlingLevelText => _handlingLevelText;
    
    public TextMeshProUGUI EngineCostText => _engineCostText;
    public TextMeshProUGUI BrakeCostText => _brakeCostText;
    public TextMeshProUGUI HandlingCostText => _handlingCostText;
    
    public Slider EngineProgressBar => _engineProgressBar;
    public Slider BrakeProgressBar => _brakeProgressBar;
    public Slider HandlingProgressBar => _handlingProgressBar;
    
    public TextMeshProUGUI EngineStatsText => _engineStatsText;
    public TextMeshProUGUI BrakeStatsText => _brakeStatsText;
    public TextMeshProUGUI HandlingStatsText => _handlingStatsText;
    
    public TextMeshProUGUI TotalCoinsText => _totalCoinsText;
    public TextMeshProUGUI CarNameText => _carNameText;
    
    /// <summary>
    /// Показывает эффект успешной прокачки
    /// </summary>
    public void PlayUpgradeEffect()
    {
        if (_upgradeEffect != null)
        {
            _upgradeEffect.Play();
        }
        
        if (_upgradeSound != null)
        {
            _upgradeSound.Play();
        }
    }
    
    /// <summary>
    /// Устанавливает название машины
    /// </summary>
    public void SetCarName(string carName)
    {
        if (_carNameText != null)
        {
            _carNameText.text = carName;
        }
    }
    
    /// <summary>
    /// Устанавливает интерактивность кнопки прокачки
    /// </summary>
    public void SetUpgradeButtonInteractable(UpgradeModule.UpgradeType upgradeType, bool interactable)
    {
        Button targetButton = null;
        
        switch (upgradeType)
        {
            case UpgradeModule.UpgradeType.Engine:
                targetButton = _engineUpgradeButton;
                break;
            case UpgradeModule.UpgradeType.Brake:
                targetButton = _brakeUpgradeButton;
                break;
            case UpgradeModule.UpgradeType.Handling:
                targetButton = _handlingUpgradeButton;
                break;
        }
        
        if (targetButton != null)
        {
            targetButton.interactable = interactable;
        }
    }
    
    /// <summary>
    /// Анимация изменения уровня
    /// </summary>
    public void AnimateLevelChange(UpgradeModule.UpgradeType upgradeType, int newLevel)
    {
        Slider targetSlider = null;
        
        switch (upgradeType)
        {
            case UpgradeModule.UpgradeType.Engine:
                targetSlider = _engineProgressBar;
                break;
            case UpgradeModule.UpgradeType.Brake:
                targetSlider = _brakeProgressBar;
                break;
            case UpgradeModule.UpgradeType.Handling:
                targetSlider = _handlingProgressBar;
                break;
        }
        
        if (targetSlider != null)
        {
            // Простая анимация через LeanTween или DOTween
            StartCoroutine(AnimateSliderValue(targetSlider, newLevel));
        }
    }
    
    private System.Collections.IEnumerator AnimateSliderValue(Slider slider, int targetValue)
    {
        float startValue = slider.value;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            slider.value = Mathf.Lerp(startValue, targetValue, progress);
            yield return null;
        }
        
        slider.value = targetValue;
    }
    
    /// <summary>
    /// Показывает/скрывает панель прокачки
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
    
    /// <summary>
    /// Подсвечивает кнопку при наведении
    /// </summary>
    public void HighlightButton(Button button, bool highlight)
    {
        if (button == null) return;
        
        var colors = button.colors;
        colors.highlightedColor = highlight ? Color.yellow : Color.white;
        button.colors = colors;
    }
}
