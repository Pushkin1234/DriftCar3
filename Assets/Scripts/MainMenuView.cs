using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    public Button PlayButton => _playButton;

    [SerializeField] private Button _tuningButton;
    [SerializeField] private GameObject _tuningCanvas;
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _shopCanvas;

    private void Start()
    {
        _tuningButton.onClick.AddListener(ActivateTuning);
    }
    private void ActivateTuning()
    {
        
        _tuningCanvas.gameObject.SetActive(true);
        _mainMenuCanvas.gameObject.SetActive(false);
        _shopCanvas.gameObject.SetActive(false);
    }
}
