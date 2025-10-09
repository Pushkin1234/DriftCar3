using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private ShopUIView _shopUIView;
    [SerializeField] private List<GameObject> _cars;
    [SerializeField] private TextMeshProUGUI _countCoinsText;
    
    private ShopModule _shopModule;
    private DataModule _dataModule;
    
    private void Start()
    {
        _shopModule = ModuleManager.Instance.GetModule<ShopModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        // Настройка кнопок
        _shopUIView.LeftNextCarButton.onClick.AddListener(() => ChangeCar(ShopModule.Direction.Left));
        _shopUIView.RightNextCarButton.onClick.AddListener(() => ChangeCar(ShopModule.Direction.Right));
        _shopUIView.BuyButton.onClick.AddListener(Buy);
        
        LoadCars();
    }
    
    private void Update()
    {
        // Обновляем UI только при изменении
        UpdateCoinsDisplay();
    }
    
    private void ChangeCar(ShopModule.Direction direction)
    {
        _shopModule.ChangeCar(direction);
        LoadCars();
    }
    
    private void Buy()
    {
        if (_shopModule.BuyCar(_shopModule.SelectedCarIndex))
        {
            LoadCars();
        }
    }
    
    private void LoadCars()
    {
        // Деактивируем все машины
        foreach (GameObject car in _cars)
        {
            car.SetActive(false);
        }
        
        // Активируем выбранную
        _cars[_shopModule.SelectedCarIndex].SetActive(true);
        
        // Обновляем UI
        bool isBought = _dataModule.Data.isBuyShop[_shopModule.SelectedCarIndex];
        
        if (isBought)
        {
            _shopUIView.BuyButton.gameObject.SetActive(false);
            _shopUIView.PriceText.gameObject.SetActive(false);
            _mainMenuView.PlayButton.gameObject.SetActive(true);
            _mainMenuView.TuningButton.gameObject.SetActive(true);
        }
        else
        {
            _shopUIView.PriceText.text = _shopModule.prices[_shopModule.SelectedCarIndex].ToString();
            _shopUIView.PriceText.gameObject.SetActive(true);
            _shopUIView.BuyButton.gameObject.SetActive(true);
            _mainMenuView.PlayButton.gameObject.SetActive(false);
            _mainMenuView.TuningButton.gameObject.SetActive(false);
        }
    }
    
    private void UpdateCoinsDisplay()
    {
        _countCoinsText.text = _dataModule.Data.coins.ToString();
    }
}