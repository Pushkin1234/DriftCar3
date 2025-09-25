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
    
    // Кэширование для оптимизации UI
    private float _lastDriftScore = 0f;
    private float _lastCoinsEarned = 0f;
    
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
        if (_driftModule == null) 
        {
            Debug.LogError("DriftModule is null");
            return;
        }
        
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
        
        // Оптимизированное обновление UI - только при изменении значений
        if (ShouldUpdateUI())
        {
            UpdateUI();
        }
    }
    
    private bool ShouldUpdateUI()
    {
        float currentDriftScore = _driftModule.DriftScoreEarnedCurrentLevel;
        float currentCoinsEarned = _driftModule.CoinsEarnedPerLevel;
        
        if (Mathf.Abs(currentDriftScore - _lastDriftScore) > 0.1f || 
            Mathf.Abs(currentCoinsEarned - _lastCoinsEarned) > 0.1f)
        {
            _lastDriftScore = currentDriftScore;
            _lastCoinsEarned = currentCoinsEarned;
            return true;
        }
        return false;
    }
    
    private void UpdateUI()
    {
        _countCoinsNumberText.text = _driftModule.CoinsEarnedPerLevel.ToString();
        _countDriftScoreNumberText.text = _driftModule.DriftScoreEarnedCurrentLevel.ToString();
        _recordCountDriftScoreNumberText.text = _dataModule.Data.recordDriftScore.ToString();
    }
    
    // Публичные методы для управления
    public void ResetLevelStats()
    {
        if (_driftModule != null)
        {
            _driftModule.ResetLevelStats();
            UpdateUI();
        }
    }
    
    public void ResetCurrentDrift()
    {
        if (_driftModule != null)
        {
            _driftModule.ResetCurrentDrift();
        }
    }
}