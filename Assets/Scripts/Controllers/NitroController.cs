using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Контроллер для управления UI системы нитро.
/// Отображает заряд нитро, состояние активации и предоставляет кнопки управления.
/// </summary>
public class NitroController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider _nitroChargeBar;
    [SerializeField] private Button _nitroButton;
    [SerializeField] private TextMeshProUGUI _nitroStatusText;
    [SerializeField] private Image _nitroIcon;
    [SerializeField] private TextMeshProUGUI _nitroKeyHint;
    
    [Header("Visual Settings")]
    [SerializeField] private Color _readyColor = Color.green;
    [SerializeField] private Color _activeColor = Color.yellow;
    [SerializeField] private Color _rechargingColor = Color.red;
    [SerializeField] private Color _disabledColor = Color.gray;
    
    [Header("Configuration")]
    [SerializeField] private KeyCode _nitroKey = KeyCode.LeftShift;
    
    private NitroModule _nitroSystem;
    
    // Кэширование для оптимизации
    private bool _lastNitroState = false;
    private float _lastChargeValue = 1f;
    
    private void Start()
    {
        // Ищем систему нитро на машине
        _nitroSystem = FindObjectOfType<NitroModule>();
        
        if (_nitroSystem == null)
        {
            Debug.LogWarning("NitroController: CustomNitroSystem not found in scene!");
            gameObject.SetActive(false);
            return;
        }
        
        InitializeUI();
        SubscribeToEvents();
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void InitializeUI()
    {
        // Настраиваем прогресс-бар
        if (_nitroChargeBar != null)
        {
            _nitroChargeBar.minValue = 0f;
            _nitroChargeBar.maxValue = 1f;
            _nitroChargeBar.value = 1f;
        }
        
        // Настраиваем кнопку
        if (_nitroButton != null)
        {
            _nitroButton.onClick.AddListener(OnNitroButtonClicked);
        }
        
        // Настраиваем подсказку клавиши
        if (_nitroKeyHint != null)
        {
            _nitroKeyHint.text = $"Press {_nitroKey} for Nitro";
        }
    }
    
    private void SubscribeToEvents()
    {
        if (_nitroSystem != null)
        {
            _nitroSystem.OnNitroStateChanged += OnNitroStateChanged;
            _nitroSystem.OnNitroChargeChanged += OnNitroChargeChanged;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (_nitroSystem != null)
        {
            _nitroSystem.OnNitroStateChanged -= OnNitroStateChanged;
            _nitroSystem.OnNitroChargeChanged -= OnNitroChargeChanged;
        }
    }
    
    private void Update()
    {
        // Обновляем UI только при изменениях
        if (ShouldUpdateUI())
        {
            UpdateUI();
        }
    }
    
    private bool ShouldUpdateUI()
    {
        if (_nitroSystem == null) return false;
        
        bool currentNitroState = _nitroSystem.IsNitroActive;
        float currentChargeValue = _nitroSystem.NitroCharge;
        
        if (currentNitroState != _lastNitroState || 
            Mathf.Abs(currentChargeValue - _lastChargeValue) > 0.01f)
        {
            _lastNitroState = currentNitroState;
            _lastChargeValue = currentChargeValue;
            return true;
        }
        
        return false;
    }
    
    private void UpdateUI()
    {
        if (_nitroSystem == null) return;
        
        bool isActive = _nitroSystem.IsNitroActive;
        bool canUse = _nitroSystem.CanUseNitro;
        float charge = _nitroSystem.NitroCharge;
        
        // Обновляем прогресс-бар
        UpdateChargeBar(charge, isActive, canUse);
        
        // Обновляем кнопку
        UpdateButton(canUse, isActive);
        
        // Обновляем текст статуса
        UpdateStatusText(isActive, canUse, charge);
        
        // Обновляем иконку
        UpdateIcon(isActive, canUse);
    }
    
    private void UpdateChargeBar(float charge, bool isActive, bool canUse)
    {
        if (_nitroChargeBar == null) return;
        
        _nitroChargeBar.value = charge;
        
        // Меняем цвет в зависимости от состояния
        var fillImage = _nitroChargeBar.fillRect?.GetComponent<Image>();
        if (fillImage != null)
        {
            if (isActive)
                fillImage.color = _activeColor;
            else if (canUse)
                fillImage.color = _readyColor;
            else
                fillImage.color = _rechargingColor;
        }
    }
    
    private void UpdateButton(bool canUse, bool isActive)
    {
        if (_nitroButton == null) return;
        
        _nitroButton.interactable = canUse && !isActive;
        
        // Меняем цвет кнопки
        var buttonImage = _nitroButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (isActive)
                buttonImage.color = _activeColor;
            else if (canUse)
                buttonImage.color = _readyColor;
            else
                buttonImage.color = _disabledColor;
        }
    }
    
    private void UpdateStatusText(bool isActive, bool canUse, float charge)
    {
        if (_nitroStatusText == null) return;
        
        if (isActive)
        {
            _nitroStatusText.text = "NITRO ACTIVE!";
            _nitroStatusText.color = _activeColor;
        }
        else if (canUse)
        {
            _nitroStatusText.text = "NITRO READY";
            _nitroStatusText.color = _readyColor;
        }
        else
        {
            _nitroStatusText.text = $"RECHARGING... {charge * 100f:F0}%";
            _nitroStatusText.color = _rechargingColor;
        }
    }
    
    private void UpdateIcon(bool isActive, bool canUse)
    {
        if (_nitroIcon == null) return;
        
        if (isActive)
            _nitroIcon.color = _activeColor;
        else if (canUse)
            _nitroIcon.color = _readyColor;
        else
            _nitroIcon.color = _disabledColor;
    }
    
    private void OnNitroButtonClicked()
    {
        if (_nitroSystem != null && _nitroSystem.CanUseNitro)
        {
            _nitroSystem.ForceActivateNitro();
        }
    }
    
    private void OnNitroStateChanged(bool isActive)
    {
        // Дополнительные действия при изменении состояния нитро
        if (isActive)
        {
            Debug.Log("Nitro activated via event!");
        }
        else
        {
            Debug.Log("Nitro deactivated via event!");
        }
    }
    
    private void OnNitroChargeChanged(float charge)
    {
        // Можно добавить дополнительные эффекты при изменении заряда
        // Например, звуковые сигналы при полной зарядке
        if (charge >= 1f)
        {
            // Нитро полностью заряжено
        }
    }
    
    /// <summary>
    /// Устанавливает ссылку на систему нитро (если нужно переключить машину)
    /// </summary>
    public void SetNitroSystem( NitroModule nitroModule)
    {
        // Отписываемся от старой системы
        UnsubscribeFromEvents();
        
        // Подключаемся к новой
        _nitroSystem = nitroModule;
        SubscribeToEvents();
        UpdateUI();
    }
    
    /// <summary>
    /// Показывает/скрывает UI нитро
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
    
    /// <summary>
    /// Обновляет подсказку клавиши
    /// </summary>
    public void UpdateKeyHint(KeyCode newKey)
    {
        _nitroKey = newKey;
        if (_nitroKeyHint != null)
        {
            _nitroKeyHint.text = $"Press {_nitroKey} for Nitro";
        }
    }
}
