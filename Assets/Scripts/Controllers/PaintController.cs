using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Простой контроллер покраски - сохраняет цвет в PlayerPrefs и применяет визуально
/// </summary>
public class PaintController : MonoBehaviour
{
    [Header("Машины (индексы: 0=e46, 1=e30, 2=skyline, 3=CTR, 4=coupe)")]
    [SerializeField] private Car _e46Car;
    [SerializeField] private Car _e30Car;
    [SerializeField] private Car _skylineCar;
    [SerializeField] private Car _CTRCar;
    [SerializeField] private Car _coupeCar;

    [Header("Кнопки цветов")]
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

    [Header("PaintManagers")]
    [SerializeField] private PaintManager _paintManagerE46;
    [SerializeField] private PaintManager _paintManagerE30;
    [SerializeField] private PaintManager _paintManagerSkyline;
    [SerializeField] private PaintManager _paintManagerCTR;
    [SerializeField] private PaintManager _paintManagerCoupe;

    private DataModule _dataModule;

    private void Awake()
    {
        // Получаем DataModule для определения индекса машины
        _dataModule = ModuleManager.Instance?.GetModule<DataModule>();
    }

    private void Start()
    {
        // Настраиваем кнопки
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

    /// <summary>
    /// Покрасить машину и сохранить цвет в PlayerPrefs
    /// </summary>
    public void Paint(Color color)
    {
        // Определяем индекс активной машины
        int carIndex = GetCurrentCarIndex();

        if (carIndex == -1)
        {
            Debug.LogWarning("[PaintController] Не удалось определить индекс машины!");
            return;
        }

        // Применяем цвет визуально
        ApplyPaintVisual(color);

        // Сохраняем цвет в PlayerPrefs (простой способ)
        SaveColorToPlayerPrefs(carIndex, color);

        Debug.Log($"[PaintController] Машина {carIndex} покрашена в цвет {color} и сохранена");
    }

    /// <summary>
    /// Применить цвет визуально к активной машине
    /// </summary>
    private void ApplyPaintVisual(Color color)
    {
        if (_e46Car != null && _e46Car.gameObject.activeSelf && _paintManagerE46 != null)
        {
            _paintManagerE46.Paint(color);
        }
        else if (_e30Car != null && _e30Car.gameObject.activeSelf && _paintManagerE30 != null)
        {
            _paintManagerE30.Paint(color);
        }
        else if (_skylineCar != null && _skylineCar.gameObject.activeSelf && _paintManagerSkyline != null)
        {
            _paintManagerSkyline.Paint(color);
        }
        else if (_CTRCar != null && _CTRCar.gameObject.activeSelf && _paintManagerCTR != null)
        {
            _paintManagerCTR.Paint(color);
        }
        else if (_coupeCar != null && _coupeCar.gameObject.activeSelf && _paintManagerCoupe != null)
        {
            _paintManagerCoupe.Paint(color);
        }
    }

    /// <summary>
    /// Получить индекс текущей активной машины
    /// </summary>
    private int GetCurrentCarIndex()
    {
        // Используем appliedCarIndex из DataModule (приоритет)
        if (_dataModule != null && _dataModule.Data != null)
        {
            return _dataModule.Data.appliedCarIndex;
        }

        // Fallback: определяем по активной машине
        if (_e46Car != null && _e46Car.gameObject.activeSelf)
            return 0;
        else if (_e30Car != null && _e30Car.gameObject.activeSelf)
            return 1;
        else if (_skylineCar != null && _skylineCar.gameObject.activeSelf)
            return 2;
        else if (_CTRCar != null && _CTRCar.gameObject.activeSelf)
            return 3;
        else if (_coupeCar != null && _coupeCar.gameObject.activeSelf)
            return 4;

        return -1;
    }

    /// <summary>
    /// Сохранить цвет в PlayerPrefs (простой способ - через R, G, B, A)
    /// </summary>
    private void SaveColorToPlayerPrefs(int carIndex, Color color)
    {
        string keyR = $"CarColor_{carIndex}_R";
        string keyG = $"CarColor_{carIndex}_G";
        string keyB = $"CarColor_{carIndex}_B";
        string keyA = $"CarColor_{carIndex}_A";

        PlayerPrefs.SetFloat(keyR, color.r);
        PlayerPrefs.SetFloat(keyG, color.g);
        PlayerPrefs.SetFloat(keyB, color.b);
        PlayerPrefs.SetFloat(keyA, color.a);
        PlayerPrefs.Save();
    }
}
