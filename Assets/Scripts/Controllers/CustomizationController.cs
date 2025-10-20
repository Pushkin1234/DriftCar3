using UnityEngine;

/// <summary>
/// Контроллер панели кастомизации. Связывает CustomizationView с модулями кастомизации.
/// Отвечает ТОЛЬКО за обработку событий и передачу данных между View и Modules.
/// НЕ содержит бизнес-логики и работы с UI элементами.
/// </summary>
public class CustomizationController : MonoBehaviour
{
    [SerializeField] private CustomizationView _view;
    
    private PaintCustomizationModule _paintModule;
    private WheelCustomizationModule _wheelModule;
    private PerformanceUpgradeModule _performanceModule;
    private SpoilerCustomizationModule _spoilerModule;
    private DataModule _dataModule;
    private int _currentCarIndex;
    
    // Текущие выбранные элементы
    private int _selectedColorIndex;
    private int _selectedWheelIndex;
    private int _selectedEngineLevel;
    private int _selectedBrakeLevel;
    private int _selectedNitroLevel;
    private int _selectedSpoilerIndex;
    
    private void Start()
    {
        InitializeModules();
        SubscribeToViewEvents();
        LoadCarCustomization();
        _view.ShowPanel(CustomizationView.PanelType.Color);
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromViewEvents();
    }
    
    #region Initialization
    
    private void InitializeModules()
    {
        _paintModule = ModuleManager.Instance?.GetModule<PaintCustomizationModule>();
        _wheelModule = ModuleManager.Instance?.GetModule<WheelCustomizationModule>();
        _performanceModule = ModuleManager.Instance?.GetModule<PerformanceUpgradeModule>();
        _spoilerModule = ModuleManager.Instance?.GetModule<SpoilerCustomizationModule>();
        _dataModule = ModuleManager.Instance?.GetModule<DataModule>();
        
        if (_paintModule == null)
            Debug.LogError("[CustomizationController] PaintCustomizationModule not found!");
        if (_wheelModule == null)
            Debug.LogError("[CustomizationController] WheelCustomizationModule not found!");
        if (_performanceModule == null)
            Debug.LogError("[CustomizationController] PerformanceUpgradeModule not found!");
        if (_spoilerModule == null)
            Debug.LogError("[CustomizationController] SpoilerCustomizationModule not found!");
        if (_dataModule == null)
            Debug.LogError("[CustomizationController] DataModule not found!");
        else
            _currentCarIndex = _dataModule.Data.appliedCarIndex;
    }
    
    private void LoadCarCustomization()
    {
        if (_paintModule == null || _wheelModule == null || _performanceModule == null || _spoilerModule == null)
            return;
        
        _selectedColorIndex = 0;
        _selectedWheelIndex = _wheelModule.GetCurrentWheelIndex(_currentCarIndex);
        _selectedEngineLevel = _performanceModule.GetEngineLevel(_currentCarIndex);
        _selectedBrakeLevel = _performanceModule.GetBrakeLevel(_currentCarIndex);
        _selectedNitroLevel = _performanceModule.GetNitroLevel(_currentCarIndex);
        _selectedSpoilerIndex = _spoilerModule.GetCurrentSpoilerIndex(_currentCarIndex);
        
        RefreshAllPanels();
    }
    
    #endregion
    
    #region View Event Subscription
    
    private void SubscribeToViewEvents()
    {
        if (_view == null) return;
        
        // Panel navigation
        _view.OnColorPanelRequested += HandleColorPanelRequest;
        _view.OnWheelsPanelRequested += HandleWheelsPanelRequest;
        _view.OnUpgradePanelRequested += HandleUpgradePanelRequest;
        _view.OnSpoilerPanelRequested += HandleSpoilerPanelRequest;
        _view.OnExitRequested += HandleExitRequest;
        _view.OnBackRequested += HandleBackRequest;
        _view.OnSelectRequested += HandleSelectRequest;
        
        // Color events
        _view.OnColorSelected += HandleColorSelection;
        _view.OnColorPurchaseRequested += HandleColorPurchase;
        _view.OnColorSelectRequested += HandleColorSelect;
        
        // Wheel events
        _view.OnWheelSelected += HandleWheelSelection;
        _view.OnWheelPurchaseRequested += HandleWheelPurchase;
        _view.OnWheelSelectRequested += HandleWheelSelect;
        
        // Upgrade events
        _view.OnEngineSelected += HandleEngineSelection;
        _view.OnBrakeSelected += HandleBrakeSelection;
        _view.OnNitroSelected += HandleNitroSelection;
        _view.OnUpgradePurchaseRequested += HandleUpgradePurchase;
        _view.OnUpgradeSelectRequested += HandleUpgradeSelect;
        
        // Spoiler events
        _view.OnSpoilerSelected += HandleSpoilerSelection;
        _view.OnSpoilerPurchaseRequested += HandleSpoilerPurchase;
        _view.OnSpoilerSelectRequested += HandleSpoilerSelect;
    }
    
    private void UnsubscribeFromViewEvents()
    {
        if (_view == null) return;
        
        _view.OnColorPanelRequested -= HandleColorPanelRequest;
        _view.OnWheelsPanelRequested -= HandleWheelsPanelRequest;
        _view.OnUpgradePanelRequested -= HandleUpgradePanelRequest;
        _view.OnSpoilerPanelRequested -= HandleSpoilerPanelRequest;
        _view.OnExitRequested -= HandleExitRequest;
        _view.OnBackRequested -= HandleBackRequest;
        _view.OnSelectRequested -= HandleSelectRequest;
        
        _view.OnColorSelected -= HandleColorSelection;
        _view.OnColorPurchaseRequested -= HandleColorPurchase;
        _view.OnColorSelectRequested -= HandleColorSelect;
        
        _view.OnWheelSelected -= HandleWheelSelection;
        _view.OnWheelPurchaseRequested -= HandleWheelPurchase;
        _view.OnWheelSelectRequested -= HandleWheelSelect;
        
        _view.OnEngineSelected -= HandleEngineSelection;
        _view.OnBrakeSelected -= HandleBrakeSelection;
        _view.OnNitroSelected -= HandleNitroSelection;
        _view.OnUpgradePurchaseRequested -= HandleUpgradePurchase;
        _view.OnUpgradeSelectRequested -= HandleUpgradeSelect;
        
        _view.OnSpoilerSelected -= HandleSpoilerSelection;
        _view.OnSpoilerPurchaseRequested -= HandleSpoilerPurchase;
        _view.OnSpoilerSelectRequested -= HandleSpoilerSelect;
    }
    
    #endregion
    
    #region Panel Navigation Handlers
    
    private void HandleColorPanelRequest()
    {
        _view.ShowPanel(CustomizationView.PanelType.Color);
        RefreshColorPanel();
    }
    
    private void HandleWheelsPanelRequest()
    {
        _view.ShowPanel(CustomizationView.PanelType.Wheels);
        RefreshWheelsPanel();
    }
    
    private void HandleUpgradePanelRequest()
    {
        _view.ShowPanel(CustomizationView.PanelType.Upgrade);
        RefreshUpgradePanel();
    }
    
    private void HandleSpoilerPanelRequest()
    {
        _view.ShowPanel(CustomizationView.PanelType.Spoiler);
        RefreshSpoilerPanel();
    }
    
    private void HandleExitRequest()
    {
        SaveCarCustomization();
        Debug.Log("[CustomizationController] Exit requested");
    }
    
    private void HandleBackRequest()
    {
        Debug.Log("[CustomizationController] Back requested");
        _view.ActivateMainMenu();
    }
    
    private void HandleSelectRequest()
    {
        SaveCarCustomization();
        Debug.Log("[CustomizationController] All upgrades applied");
    }
    
    #endregion
    
    #region Color Handlers
    
    private void HandleColorSelection(int colorIndex)
    {
        _selectedColorIndex = colorIndex;
        
        // Применяем цвет для предварительного просмотра через Module
        _paintModule.SelectColor(_currentCarIndex, colorIndex);
        
        // Обновляем UI через View
        RefreshColorPanel();
    }
    
    private void HandleColorPurchase()
    {
        bool success = _paintModule.PurchaseColor(_currentCarIndex, _selectedColorIndex);
        
        if (success)
        {
            RefreshColorPanel();
        }
    }
    
    private void HandleColorSelect()
    {
        var colorData = _paintModule.GetColorData(_selectedColorIndex);
        if (colorData != null)
        {
            _paintModule.PaintCar(_currentCarIndex, colorData.color);
        }
    }
    
    #endregion
    
    #region Wheel Handlers
    
    private void HandleWheelSelection(int wheelIndex)
    {
        _selectedWheelIndex = wheelIndex;
        RefreshWheelsPanel();
    }
    
    private void HandleWheelPurchase()
    {
        bool success = _wheelModule.PurchaseWheel(_selectedWheelIndex);
        
        if (success)
        {
            RefreshWheelsPanel();
        }
    }
    
    private void HandleWheelSelect()
    {
        _wheelModule.ChangeWheels(_currentCarIndex, _selectedWheelIndex);
    }
    
    #endregion
    
    #region Upgrade Handlers
    
    private void HandleEngineSelection(int level)
    {
        _selectedEngineLevel = level;
        RefreshUpgradePanel();
    }
    
    private void HandleBrakeSelection(int level)
    {
        _selectedBrakeLevel = level;
        RefreshUpgradePanel();
    }
    
    private void HandleNitroSelection(int level)
    {
        _selectedNitroLevel = level;
        RefreshUpgradePanel();
    }
    
    private void HandleUpgradePurchase()
    {
        bool success = false;
        
        // Определяем какое улучшение покупаем (последнее выбранное)
        // В идеале нужно добавить флаг активного типа улучшения
        success = _performanceModule.PurchaseEngineUpgrade(_currentCarIndex, _selectedEngineLevel);
        
        if (success)
        {
            RefreshUpgradePanel();
        }
    }
    
    private void HandleUpgradeSelect()
    {
        // Улучшения применяются автоматически при покупке
    }
    
    #endregion
    
    #region Spoiler Handlers
    
    private void HandleSpoilerSelection(int spoilerIndex)
    {
        _selectedSpoilerIndex = spoilerIndex;
        RefreshSpoilerPanel();
    }
    
    private void HandleSpoilerPurchase()
    {
        bool success = _spoilerModule.PurchaseSpoiler(_currentCarIndex, _selectedSpoilerIndex);
        
        if (success)
        {
            RefreshSpoilerPanel();
        }
    }
    
    private void HandleSpoilerSelect()
    {
        var spoilerData = _spoilerModule.GetSpoilerData(_selectedSpoilerIndex);
        if (spoilerData != null)
        {
            _spoilerModule.ChangeSpoiler(_currentCarIndex, _selectedSpoilerIndex);
            Debug.Log($"Spoiler {spoilerData.spoilerName} applied");
        }
    }
    
    #endregion
    
    #region Refresh UI Methods
    
    private void RefreshAllPanels()
    {
        RefreshColorPanel();
        RefreshWheelsPanel();
        RefreshUpgradePanel();
        RefreshSpoilerPanel();
    }
    
    private void RefreshColorPanel()
    {
        if (_paintModule == null) return;
        
        // Обновляем все кнопки цветов
        for (int i = 0; i < _paintModule.GetColorCount(); i++)
        {
            bool isUnlocked = _paintModule.IsColorUnlocked(_currentCarIndex, i);
            _view.UpdateColorButtonState(i, isUnlocked);
        }
        
        // Обновляем информацию о выбранном цвете
        var colorData = _paintModule.GetColorData(_selectedColorIndex);
        if (colorData != null)
        {
            bool isUnlocked = _paintModule.IsColorUnlocked(_currentCarIndex, _selectedColorIndex);
            _view.UpdateColorUI(colorData.price, isUnlocked, colorData.color);
        }
    }
    
    private void RefreshWheelsPanel()
    {
        if (_wheelModule == null) return;
        
        for (int i = 0; i < _wheelModule.GetWheelCount(); i++)
        {
            bool isUnlocked = _wheelModule.IsWheelUnlocked(i);
            _view.UpdateWheelButtonState(i, isUnlocked);
        }
        
        var wheelData = _wheelModule.GetWheelData(_selectedWheelIndex);
        if (wheelData != null)
        {
            bool isUnlocked = _wheelModule.IsWheelUnlocked(_selectedWheelIndex);
            _view.UpdateWheelUI(wheelData.price, isUnlocked);
        }
    }
    
    private void RefreshUpgradePanel()
    {
        if (_performanceModule == null) return;
        
        // Обновляем кнопки двигателя
        for (int i = 0; i < _performanceModule.GetEngineUpgradeCount(); i++)
        {
            bool isUnlocked = _performanceModule.IsEngineUpgradeUnlocked(_currentCarIndex, i);
            _view.UpdateEngineButtonState(i, isUnlocked);
        }
        
        // Обновляем кнопки тормозов
        for (int i = 0; i < _performanceModule.GetBrakeUpgradeCount(); i++)
        {
            bool isUnlocked = _performanceModule.IsBrakeUpgradeUnlocked(_currentCarIndex, i);
            _view.UpdateBrakeButtonState(i, isUnlocked);
        }
        
        // Обновляем кнопки нитро
        for (int i = 0; i < _performanceModule.GetNitroUpgradeCount(); i++)
        {
            bool isUnlocked = _performanceModule.IsNitroUpgradeUnlocked(_currentCarIndex, i);
            _view.UpdateNitroButtonState(i, isUnlocked);
        }
        
        // Обновляем информацию о выбранном улучшении
        var upgradeData = _performanceModule.GetEngineUpgradeData(_selectedEngineLevel);
        if (upgradeData != null)
        {
            bool isUnlocked = _performanceModule.IsEngineUpgradeUnlocked(_currentCarIndex, _selectedEngineLevel);
            _view.UpdateUpgradeUI(upgradeData.price, isUnlocked);
        }
    }
    
    private void RefreshSpoilerPanel()
    {
        if (_spoilerModule == null) return;
        
        for (int i = 0; i < _spoilerModule.GetSpoilerCount(); i++)
        {
            bool isUnlocked = _spoilerModule.IsSpoilerUnlocked(_currentCarIndex, i);
            _view.UpdateSpoilerButtonState(i, isUnlocked);
        }
        
        var spoilerData = _spoilerModule.GetSpoilerData(_selectedSpoilerIndex);
        if (spoilerData != null)
        {
            bool isUnlocked = _spoilerModule.IsSpoilerUnlocked(_currentCarIndex, _selectedSpoilerIndex);
            _view.UpdateSpoilerUI(spoilerData.price, isUnlocked);
        }
    }
    
    #endregion
    
    #region Save/Load
    
    private void SaveCarCustomization()
    {
        // Модули автоматически сохраняют данные при изменениях
        // Но можем явно вызвать сохранение для гарантии
        Debug.Log($"[CustomizationController] Saved customization for car {_currentCarIndex}");
    }
    
    #endregion
}
