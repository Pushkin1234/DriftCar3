using UnityEngine;

/// <summary>
/// Контроллер панели кастомизации. Связывает CustomizationView и CustomizationModule.
/// Отвечает ТОЛЬКО за обработку событий и передачу данных между View и Module.
/// НЕ содержит бизнес-логики и работы с UI элементами.
/// </summary>
public class CustomizationController : MonoBehaviour
{
    [SerializeField] private CustomizationView _view;
    
    private CustomizationModule _customizationModule;
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
        _customizationModule = ModuleManager.Instance?.GetModule<CustomizationModule>();
        _dataModule = ModuleManager.Instance?.GetModule<DataModule>();
        
        if (_customizationModule == null)
            Debug.LogError("[CustomizationController] CustomizationModule not found!");
            
        if (_dataModule == null)
            Debug.LogError("[CustomizationController] DataModule not found!");
        else
            _currentCarIndex = _dataModule.Data.appliedCarIndex;
    }
    
    private void LoadCarCustomization()
    {
        if (_customizationModule == null) return;
        
        var customization = _customizationModule.GetCarCustomizationByIndex(_currentCarIndex);
        
        _selectedColorIndex = 0;
        _selectedWheelIndex = customization.selectedWheelIndex;
        _selectedEngineLevel = customization.engineLevel;
        _selectedBrakeLevel = customization.brakeLevel;
        _selectedNitroLevel = customization.nitroLevel;
        _selectedSpoilerIndex = customization.selectedSpoilerIndex;
        
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
        string carName = $"Car_{_currentCarIndex}";
        _customizationModule.SelectColor(carName, colorIndex);
        
        // Обновляем UI через View
        RefreshColorPanel();
    }
    
    private void HandleColorPurchase()
    {
        string carName = $"Car_{_currentCarIndex}";
        bool success = _customizationModule.PurchaseColor(carName, _selectedColorIndex);
        
        if (success)
        {
            RefreshColorPanel();
        }
    }
    
    private void HandleColorSelect()
    {
        var colorData = _customizationModule.GetColorData(_selectedColorIndex);
        if (colorData != null)
        {
            string carName = $"Car_{_currentCarIndex}";
            _customizationModule.PaintCar(carName, colorData.color);
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
        bool success = _customizationModule.UnlockWheel(_selectedWheelIndex);
        
        if (success)
        {
            RefreshWheelsPanel();
        }
    }
    
    private void HandleWheelSelect()
    {
        string carName = $"Car_{_currentCarIndex}";
        _customizationModule.ChangeWheels(carName, _selectedWheelIndex);
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
        string carName = $"Car_{_currentCarIndex}";
        bool success = false;
        
        // Определяем какое улучшение покупаем (последнее выбранное)
        // В идеале нужно добавить флаг активного типа улучшения
        success = _customizationModule.PurchaseEngineUpgrade(carName, _selectedEngineLevel);
        
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
        string carName = $"Car_{_currentCarIndex}";
        bool success = _customizationModule.PurchaseSpoiler(carName, _selectedSpoilerIndex);
        
        if (success)
        {
            RefreshSpoilerPanel();
        }
    }
    
    private void HandleSpoilerSelect()
    {
        var spoilerData = _customizationModule.GetSpoilerData(_selectedSpoilerIndex);
        if (spoilerData != null)
        {
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
        string carName = $"Car_{_currentCarIndex}";
        
        // Обновляем все кнопки цветов
        for (int i = 0; i < _customizationModule.GetColorCount(); i++)
        {
            bool isUnlocked = _customizationModule.IsColorUnlocked(carName, i);
            _view.UpdateColorButtonState(i, isUnlocked);
        }
        
        // Обновляем информацию о выбранном цвете
        var colorData = _customizationModule.GetColorData(_selectedColorIndex);
        if (colorData != null)
        {
            bool isUnlocked = _customizationModule.IsColorUnlocked(carName, _selectedColorIndex);
            _view.UpdateColorUI(colorData.price, isUnlocked, colorData.color);
        }
    }
    
    private void RefreshWheelsPanel()
    {
        string carName = $"Car_{_currentCarIndex}";
        
        for (int i = 0; i < _customizationModule.GetWheelCount(); i++)
        {
            bool isUnlocked = _customizationModule.IsWheelUnlocked(i);
            _view.UpdateWheelButtonState(i, isUnlocked);
        }
        
        var wheelData = _customizationModule.GetWheelData(_selectedWheelIndex);
        if (wheelData != null)
        {
            bool isUnlocked = _customizationModule.IsWheelUnlocked(_selectedWheelIndex);
            _view.UpdateWheelUI(wheelData.price, isUnlocked);
        }
    }
    
    private void RefreshUpgradePanel()
    {
        string carName = $"Car_{_currentCarIndex}";
        
        // Обновляем кнопки двигателя
        for (int i = 0; i < _customizationModule.GetEngineUpgradeCount(); i++)
        {
            bool isUnlocked = _customizationModule.IsEngineUpgradeUnlocked(carName, i);
            _view.UpdateEngineButtonState(i, isUnlocked);
        }
        
        // Обновляем кнопки тормозов
        for (int i = 0; i < _customizationModule.GetBrakeUpgradeCount(); i++)
        {
            bool isUnlocked = _customizationModule.IsBrakeUpgradeUnlocked(carName, i);
            _view.UpdateBrakeButtonState(i, isUnlocked);
        }
        
        // Обновляем кнопки нитро
        for (int i = 0; i < _customizationModule.GetNitroUpgradeCount(); i++)
        {
            bool isUnlocked = _customizationModule.IsNitroUpgradeUnlocked(carName, i);
            _view.UpdateNitroButtonState(i, isUnlocked);
        }
        
        // Обновляем информацию о выбранном улучшении
        var upgradeData = _customizationModule.GetEngineUpgradeData(_selectedEngineLevel);
        if (upgradeData != null)
        {
            bool isUnlocked = _customizationModule.IsEngineUpgradeUnlocked(carName, _selectedEngineLevel);
            _view.UpdateUpgradeUI(upgradeData.price, isUnlocked);
        }
    }
    
    private void RefreshSpoilerPanel()
    {
        string carName = $"Car_{_currentCarIndex}";
        
        for (int i = 0; i < _customizationModule.GetSpoilerCount(); i++)
        {
            bool isUnlocked = _customizationModule.IsSpoilerUnlocked(carName, i);
            _view.UpdateSpoilerButtonState(i, isUnlocked);
        }
        
        var spoilerData = _customizationModule.GetSpoilerData(_selectedSpoilerIndex);
        if (spoilerData != null)
        {
            bool isUnlocked = _customizationModule.IsSpoilerUnlocked(carName, _selectedSpoilerIndex);
            _view.UpdateSpoilerUI(spoilerData.price, isUnlocked);
        }
    }
    
    #endregion
    
    #region Save/Load
    
    private void SaveCarCustomization()
    {
        if (_customizationModule == null) return;
        
        _customizationModule.SaveCarCustomizationByIndex(_currentCarIndex);
        Debug.Log($"[CustomizationController] Saved customization for car {_currentCarIndex}");
    }
    
    #endregion
}
