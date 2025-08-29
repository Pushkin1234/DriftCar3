using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private int _countCoins;

    [SerializeField] private TextMeshProUGUI _countCoinsText;

    private void Start()
    {
        _countCoins = SaveData.Instance.Data.Coins;
        _countCoinsText.text = _countCoins.ToString();
    }
}
