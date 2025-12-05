using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PaintController : MonoBehaviour
{
    [SerializeField] private Car _e46Car;
    [SerializeField] private Car _e30Car;
    [SerializeField] private Car _skylineCar;
    [SerializeField] private Car _CTRCar;
    [SerializeField] private Car _coupeCar;

    [SerializeField] private Button _redButton;
    [SerializeField] private Button _blueButton;
    [SerializeField] private Button _greenButton;
    [SerializeField] private Button _yellowButton;
    [SerializeField] private Button _purpleButton;
    [SerializeField] private Button _blackButton;
    [SerializeField] private Button _whiteButton;
    [SerializeField] private Button _cyanButton;
    [SerializeField] private Button _grayButton;
    [SerializeField] private Button _pinkButton;
    [SerializeField] private Button _orangeButton;
    [SerializeField] private Button _brownButton;
    [SerializeField] private Button _goldButton;
    [SerializeField] private Button _silverButton;
    [SerializeField] private Button _bronzeButton;
    [SerializeField] private Button _salatButton;
    [SerializeField] private Button _turquoiseButton;
    [SerializeField] private Button _crimsonButton;
    [SerializeField] private Button _limeButton;

    [SerializeField] private PaintManager _paintManagerE46;
    [SerializeField] private PaintManager _paintManagerE30;
    [SerializeField] private PaintManager _paintManagerSkyline;
    [SerializeField] private PaintManager _paintManagerCTR;
    [SerializeField] private PaintManager _paintManagerCoupe;

    private void Start()
    {
        _redButton.onClick.AddListener(() => Paint(Color.red));
        _blueButton.onClick.AddListener(() => Paint(Color.blue));
        _greenButton.onClick.AddListener(() => Paint(Color.green));
        _yellowButton.onClick.AddListener(() => Paint(Color.yellow));
        _purpleButton.onClick.AddListener(() => Paint(new Color(0.5f, 0f, 0.5f))); // Purple
        _blackButton.onClick.AddListener(() => Paint(Color.black));
        _whiteButton.onClick.AddListener(() => Paint(Color.white));
        _cyanButton.onClick.AddListener(() => Paint(Color.cyan));
        _grayButton.onClick.AddListener(() => Paint(Color.gray));
        _pinkButton.onClick.AddListener(() => Paint(new Color(1f, 0.75f, 0.8f))); // Pink
        _orangeButton.onClick.AddListener(() => Paint(new Color(1f, 0.5f, 0f))); // Orange
        _brownButton.onClick.AddListener(() => Paint(new Color(0.6f, 0.3f, 0f))); // Brown
        _goldButton.onClick.AddListener(() => Paint(new Color(1f, 0.84f, 0f))); // Gold
        _silverButton.onClick.AddListener(() => Paint(new Color(0.75f, 0.75f, 0.75f))); // Silver
        _bronzeButton.onClick.AddListener(() => Paint(new Color(0.8f, 0.5f, 0.2f))); // Bronze
        _salatButton.onClick.AddListener(() => Paint(new Color(0.5f, 1f, 0.5f))); // Salat (light green)
        _turquoiseButton.onClick.AddListener(() => Paint(new Color(0.25f, 0.88f, 0.82f))); // Turquoise
        _crimsonButton.onClick.AddListener(() => Paint(new Color(0.86f, 0.08f, 0.24f))); // Crimson
        _limeButton.onClick.AddListener(() => Paint(new Color(0.75f, 1f, 0f))); // Lime
    }

    public void Paint(Color color)
    {
        if (_e46Car.gameObject.activeSelf)
        {
            _paintManagerE46.Paint(color);
        }
        else if (_e30Car.gameObject.activeSelf)
        {
            _paintManagerE30.Paint(color);
        }
        else if (_skylineCar.gameObject.activeSelf)
        {
            _paintManagerSkyline.Paint(color);
        }
        else if (_CTRCar.gameObject.activeSelf)
        {
            _paintManagerCTR.Paint(color);
        }
        else if (_coupeCar.gameObject.activeSelf)
        {
            _paintManagerCoupe.Paint(color);
        }
    }
}
