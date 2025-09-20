using TMPro;
using UnityEngine;

public class DriftController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countCoinsNumberText;
    [SerializeField] private TextMeshProUGUI _countDriftScoreNumberText;
    [SerializeField] private TextMeshProUGUI _recordCountDriftScoreNumberText;
    [SerializeField] private TextMeshProUGUI _driftScoreMultiplier;
    [SerializeField] private TextMeshProUGUI _driftScoreText;
    [SerializeField] private TextMeshProUGUI _driftScoreTitle;
    
    private DriftModule _driftModule;
    private DataModule _dataModule;
    private int _countDriftScoreEarnedPerLevel = 0;
    private int _countCoinsEarnedPerLevel = 0;
    
    private void Start()
    {
        _driftModule = ModuleManager.Instance.GetModule<DriftModule>();
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        
        // Инициализация UI
        _driftScoreMultiplier.gameObject.SetActive(false);
        _driftScoreText.gameObject.SetActive(false);
        _driftScoreTitle.gameObject.SetActive(false);
        
        UpdateUI();
    }
    
    private void Update()
    {
        if (_driftModule == null) return;
        
        // Обновляем UI только при изменении
        if (_driftModule.IsDrifting)
        {
            _driftScoreText.text = "+" + Mathf.RoundToInt(_driftModule.CurrentDriftScore).ToString();
            _driftScoreText.gameObject.SetActive(true);
            _driftScoreTitle.gameObject.SetActive(true);
        }
        else
        {
            _driftScoreText.gameObject.SetActive(false);
            _driftScoreTitle.gameObject.SetActive(false);
        }
    }
    
    private void UpdateUI()
    {
        _countCoinsNumberText.text = _countCoinsEarnedPerLevel.ToString();
        _countDriftScoreNumberText.text = _countDriftScoreEarnedPerLevel.ToString();
        _recordCountDriftScoreNumberText.text = _dataModule.Data.recordDriftScore.ToString();
    }
}