using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIView : MonoBehaviour
{
    [SerializeField] private Button _leftNextCarButton;
    [SerializeField] private Button _rightNextCarButton;
    [SerializeField] private Button _buyButton;

    [SerializeField] private TextMeshProUGUI _priceText;

    public Button LeftNextCarButton => _leftNextCarButton;
    public Button RightNextCarButton => _rightNextCarButton;
    public Button BuyButton => _buyButton;
    public TextMeshProUGUI PriceText => _priceText;
}
