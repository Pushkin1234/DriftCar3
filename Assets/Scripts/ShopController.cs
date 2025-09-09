using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private List<bool> _isBuy;

    [SerializeField] private List<int> _prices;

    [SerializeField] private ShopUIView _shopUIView;

    [SerializeField] private List<GameObject> _cars;

    private int _countCoins;

    [SerializeField] private int _loadIndex;

    public int LoadIndex => _loadIndex;

    [SerializeField] private TextMeshProUGUI _countCoinsText;

    private void Start()
    {
        _shopUIView.LeftNextCarButton.onClick.AddListener(() => ChangeCar(Direction.Left));
        _shopUIView.RightNextCarButton.onClick.AddListener(() => ChangeCar(Direction.Right));
        _shopUIView.BuyButton.onClick.AddListener(Buy);
        LoadCars();
    }

    private void UpdateCoins()
    {
        _countCoins = SaveData.Instance.Data.Coins;
        _countCoinsText.text = _countCoins.ToString();
        _isBuy = SaveData.Instance.Data.IsBuyShop;
    }

    private void Buy()
    {
        if(SaveData.Instance.Data.Coins > _prices[_loadIndex])
        {
            _isBuy[_loadIndex] = true;

            SaveData.Instance.Data.Coins = SaveData.Instance.Data.Coins - _prices[_loadIndex];
            SaveData.Instance.Data.IsBuyShop[_loadIndex] = true;
            _isBuy = SaveData.Instance.Data.IsBuyShop;
            SaveData.Instance.SaveYandex();

            UpdateCoins();
            LoadCars();
        }
    }
    private void LoadCars()
    {
        foreach (GameObject car in _cars)
        {
            car.SetActive(false);
        }
        _cars[_loadIndex].SetActive(true);
        if (_isBuy[_loadIndex])
        {
            _shopUIView.BuyButton.gameObject.SetActive(false);
            _shopUIView.PriceText.gameObject.SetActive(false);
            //_mainMenuView.StartButton.gameObject.SetActive(true);
        }
        else
        {
            _shopUIView.PriceText.text = _prices[_loadIndex].ToString();
            _shopUIView.PriceText.gameObject.SetActive(true);
            _shopUIView.BuyButton.gameObject.SetActive(true);
            //_mainMenuView.StartButton.gameObject.SetActive(false);
        }
    }
    public void ChangeCar(Direction direction)
    {
        if (direction == Direction.Left)
        {
            _loadIndex = (_loadIndex - 1 + _cars.Count) % _cars.Count;
        }
        else if (direction == Direction.Right)

        {
            _loadIndex = (_loadIndex + 1) % _cars.Count;
        }
        LoadCars();

    }

    public enum Direction
    {
        Left,

        Right
    }
}
