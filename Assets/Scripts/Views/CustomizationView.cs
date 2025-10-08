using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// View для панели кастомизации. Отвечает ТОЛЬКО за отображение UI элементов.
/// Не содержит бизнес-логики - только обновление визуальных элементов.
/// </summary>
public class CustomizationView : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private Button _colorButton;
    [SerializeField] private Button _wheelsButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _spoilerButton;
    [SerializeField] private Button _exitButton;
    
    [Header("Scroll Views")]
    [SerializeField] private ScrollRect _colorScrollView;
    [SerializeField] private ScrollRect _wheelsScrollView;
    [SerializeField] private ScrollRect _upgradeScrollView;
    [SerializeField] private ScrollRect _spoilerScrollView;
    
    [Header("Color UI")]
    [SerializeField] private Button[] _colorButtons;
    [SerializeField] private TextMeshProUGUI _colorPriceText;
    [SerializeField] private Button _purchaseColorButton;
    [SerializeField] private Button _selectColorButton;
    
    [Header("Wheels UI")]
    [SerializeField] private Button[] _wheelButtons;
    [SerializeField] private TextMeshProUGUI _wheelPriceText;
    [SerializeField] private Button _purchaseWheelButton;
    [SerializeField] private Button _selectWheelButton;
    
    [Header("Upgrade UI")]
    [SerializeField] private Button[] _engineButtons;
    [SerializeField] private Button[] _brakeButtons;
    [SerializeField] private Button[] _nitroButtons;
    [SerializeField] private TextMeshProUGUI _upgradePriceText;
    [SerializeField] private Button _purchaseUpgradeButton;
    [SerializeField] private Button _selectUpgradeButton;
    
    [Header("Spoiler UI")]
    [SerializeField] private Button[] _spoilerButtons;
    [SerializeField] private TextMeshProUGUI _spoilerPriceText;
    [SerializeField] private Button _purchaseSpoilerButton;
    [SerializeField] private Button _selectSpoilerButton;
    
    [Header("Bottom Actions")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _selectButton;

    [SerializeField] private Canvas _tuningCanvas;
    [SerializeField] private Canvas _shopCanvas;
    [SerializeField] private Canvas _mainMenuCanvas;
    
    // События для контроллера
    public event Action OnColorPanelRequested;
    public event Action OnWheelsPanelRequested;
    public event Action OnUpgradePanelRequested;
    public event Action OnSpoilerPanelRequested;
    public event Action OnExitRequested;
    public event Action OnBackRequested;
    public event Action OnSelectRequested;
    
    public event Action<int> OnColorSelected;
    public event Action OnColorPurchaseRequested;
    public event Action OnColorSelectRequested;
    
    public event Action<int> OnWheelSelected;
    public event Action OnWheelPurchaseRequested;
    public event Action OnWheelSelectRequested;
    
    public event Action<int> OnEngineSelected;
    public event Action<int> OnBrakeSelected;
    public event Action<int> OnNitroSelected;
    public event Action OnUpgradePurchaseRequested;
    public event Action OnUpgradeSelectRequested;
    
    public event Action<int> OnSpoilerSelected;
    public event Action OnSpoilerPurchaseRequested;
    public event Action OnSpoilerSelectRequested;
    
    private void Start()
    {
        SetupButtons();
    }
    
    private void SetupButtons()
    {
        // Основные кнопки панелей
        _colorButton?.onClick.AddListener(() => OnColorPanelRequested?.Invoke());
        _wheelsButton?.onClick.AddListener(() => OnWheelsPanelRequested?.Invoke());
        _upgradeButton?.onClick.AddListener(() => OnUpgradePanelRequested?.Invoke());
        _spoilerButton?.onClick.AddListener(() => OnSpoilerPanelRequested?.Invoke());
        _exitButton?.onClick.AddListener(() => OnExitRequested?.Invoke());
        
        // Кнопки цветов
        for (int i = 0; i < _colorButtons.Length; i++)
        {
            int index = i;
            _colorButtons[i]?.onClick.AddListener(() => OnColorSelected?.Invoke(index));
        }
        _purchaseColorButton?.onClick.AddListener(() => OnColorPurchaseRequested?.Invoke());
        _selectColorButton?.onClick.AddListener(() => OnColorSelectRequested?.Invoke());
        
        // Кнопки колес
        for (int i = 0; i < _wheelButtons.Length; i++)
        {
            int index = i;
            _wheelButtons[i]?.onClick.AddListener(() => OnWheelSelected?.Invoke(index));
        }
        _purchaseWheelButton?.onClick.AddListener(() => OnWheelPurchaseRequested?.Invoke());
        _selectWheelButton?.onClick.AddListener(() => OnWheelSelectRequested?.Invoke());
        
        // Кнопки улучшений
        for (int i = 0; i < _engineButtons.Length; i++)
        {
            int index = i;
            _engineButtons[i]?.onClick.AddListener(() => OnEngineSelected?.Invoke(index));
        }
        for (int i = 0; i < _brakeButtons.Length; i++)
        {
            int index = i;
            _brakeButtons[i]?.onClick.AddListener(() => OnBrakeSelected?.Invoke(index));
        }
        for (int i = 0; i < _nitroButtons.Length; i++)
        {
            int index = i;
            _nitroButtons[i]?.onClick.AddListener(() => OnNitroSelected?.Invoke(index));
        }
        _purchaseUpgradeButton?.onClick.AddListener(() => OnUpgradePurchaseRequested?.Invoke());
        _selectUpgradeButton?.onClick.AddListener(() => OnUpgradeSelectRequested?.Invoke());
        
        // Кнопки спойлеров
        for (int i = 0; i < _spoilerButtons.Length; i++)
        {
            int index = i;
            _spoilerButtons[i]?.onClick.AddListener(() => OnSpoilerSelected?.Invoke(index));
        }
        _purchaseSpoilerButton?.onClick.AddListener(() => OnSpoilerPurchaseRequested?.Invoke());
        _selectSpoilerButton?.onClick.AddListener(() => OnSpoilerSelectRequested?.Invoke());
        
        // Нижние кнопки
        _backButton?.onClick.AddListener(() => OnBackRequested?.Invoke());
        _selectButton?.onClick.AddListener(() => OnSelectRequested?.Invoke());
    }
    
    #region Panel Visibility
    
    public void ShowPanel(PanelType panelType)
    {
        HideAllPanels();
        
        switch (panelType)
        {
            case PanelType.Color:
                _colorScrollView?.gameObject.SetActive(true);
                break;
            case PanelType.Wheels:
                _wheelsScrollView?.gameObject.SetActive(true);
                break;
            case PanelType.Upgrade:
                _upgradeScrollView?.gameObject.SetActive(true);
                break;
            case PanelType.Spoiler:
                _spoilerScrollView?.gameObject.SetActive(true);
                break;
        }
    }
    
    private void HideAllPanels()
    {
        _colorScrollView?.gameObject.SetActive(false);
        _wheelsScrollView?.gameObject.SetActive(false);
        _upgradeScrollView?.gameObject.SetActive(false);
        _spoilerScrollView?.gameObject.SetActive(false);
    }
    
    #endregion
    
    #region Update UI Methods
    
    public void UpdateColorUI(int price, bool isUnlocked, Color buttonColor)
    {
        UpdatePriceText(_colorPriceText, price, isUnlocked);
        UpdateActionButtons(_purchaseColorButton, _selectColorButton, isUnlocked);
    }
    
    public void UpdateColorButtonState(int index, bool isUnlocked)
    {
        if (index < 0 || index >= _colorButtons.Length) return;
        UpdateButtonVisual(_colorButtons[index], isUnlocked);
    }
    
    public void UpdateWheelUI(int price, bool isUnlocked)
    {
        UpdatePriceText(_wheelPriceText, price, isUnlocked);
        UpdateActionButtons(_purchaseWheelButton, _selectWheelButton, isUnlocked);
    }
    
    public void UpdateWheelButtonState(int index, bool isUnlocked)
    {
        if (index < 0 || index >= _wheelButtons.Length) return;
        UpdateButtonVisual(_wheelButtons[index], isUnlocked);
    }
    
    public void UpdateUpgradeUI(int price, bool isUnlocked)
    {
        UpdatePriceText(_upgradePriceText, price, isUnlocked);
        UpdateActionButtons(_purchaseUpgradeButton, _selectUpgradeButton, isUnlocked);
    }
    
    public void UpdateEngineButtonState(int index, bool isUnlocked)
    {
        if (index < 0 || index >= _engineButtons.Length) return;
        UpdateButtonVisual(_engineButtons[index], isUnlocked);
    }
    
    public void UpdateBrakeButtonState(int index, bool isUnlocked)
    {
        if (index < 0 || index >= _brakeButtons.Length) return;
        UpdateButtonVisual(_brakeButtons[index], isUnlocked);
    }
    
    public void UpdateNitroButtonState(int index, bool isUnlocked)
    {
        if (index < 0 || index >= _nitroButtons.Length) return;
        UpdateButtonVisual(_nitroButtons[index], isUnlocked);
    }
    
    public void UpdateSpoilerUI(int price, bool isUnlocked)
    {
        UpdatePriceText(_spoilerPriceText, price, isUnlocked);
        UpdateActionButtons(_purchaseSpoilerButton, _selectSpoilerButton, isUnlocked);
    }
    
    public void UpdateSpoilerButtonState(int index, bool isUnlocked)
    {
        if (index < 0 || index >= _spoilerButtons.Length) return;
        UpdateButtonVisual(_spoilerButtons[index], isUnlocked);
    }
    
    #endregion
    
    #region Helper Methods
    
    private void UpdatePriceText(TextMeshProUGUI priceText, int price, bool isUnlocked)
    {
        if (priceText == null) return;
        
        if (isUnlocked)
        {
            priceText.text = "Разблокирован";
            priceText.color = Color.green;
        }
        else
        {
            priceText.text = $"{price} 💰";
            priceText.color = Color.yellow;
        }
    }
    
    private void UpdateLockIcon(GameObject lockIcon, bool isUnlocked)
    {
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);
    }
    
    private void UpdateActionButtons(Button purchaseButton, Button selectButton, bool isUnlocked)
    {
        if (purchaseButton != null)
            purchaseButton.gameObject.SetActive(!isUnlocked);
            
        if (selectButton != null)
            selectButton.gameObject.SetActive(isUnlocked);
    }
    
    private void UpdateButtonVisual(Button button, bool isUnlocked)
    {
        if (button == null) return;
        
        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            float dimFactor = isUnlocked ? 1f : 0.5f;
            buttonImage.color = new Color(
                originalColor.r * dimFactor,
                originalColor.g * dimFactor,
                originalColor.b * dimFactor,
                originalColor.a
            );
        }
    }

    public void ActivateMainMenu()
    {
        _tuningCanvas.gameObject.SetActive(false);
        _mainMenuCanvas.gameObject.SetActive(true);
        _shopCanvas.gameObject.SetActive(true);
    }
    
    #endregion
    
    public enum PanelType
    {
        Color,
        Wheels,
        Upgrade,
        Spoiler
    }
}

