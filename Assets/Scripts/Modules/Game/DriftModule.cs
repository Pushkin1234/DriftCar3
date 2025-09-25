using UnityEngine;

public class DriftModule : BaseGameModule
{
    public override string ModuleName => "Drift";
    
    // Конфигурация
    public float driftThreshold = 10f;
    public float requiredSpeed = 10f;
    public float timeToEndDrift = 1f;
    public float[] scoreMilestones = { 500f, 1000f, 1500f, 2000f, 2500f };
    public float[] multipliers = { 2f, 3f, 4f, 5f, 6f };
    
    // Состояние
    private float _driftScore = 0f;
    private bool _isDrifting = false;
    private int _currentMultiplierIndex = 0;
    private bool _isActivateMultiplier = false;
    private float _timeWithoutDrift = 0;


    private float _driftScoreEarnedCurrentLevel = 0f;
    public float DriftScoreEarnedCurrentLevel => _driftScoreEarnedCurrentLevel;

    private float _coinsEarnedPerLevel = 0f;
    public float CoinsEarnedPerLevel => _coinsEarnedPerLevel;
    
    // Компоненты
    private RCC_CarControllerV3 _carController;
    private DataModule _dataModule;
    
    public float CurrentDriftScore => _driftScore;
    public bool IsDrifting => _isDrifting;
    
    // Методы для управления счетчиками уровня
    public void ResetLevelStats()
    {
        _driftScoreEarnedCurrentLevel = 0f;
        _coinsEarnedPerLevel = 0f;
    }
    
    public void ResetCurrentDrift()
    {
        _driftScore = 0f;
        _isDrifting = false;
        _currentMultiplierIndex = 0;
        _isActivateMultiplier = false;
        _timeWithoutDrift = 0f;
    }
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        base.Initialize();
    }
    
    public override void Update()
    {
        if (_carController == null) 
        {
            _carController = FindObjectOfType<RCC_CarControllerV3>();
            return;
        }
        
        float driftAngle = Vector3.Angle(_carController.transform.forward, _carController.Rigid.velocity);
        
        if (driftAngle > driftThreshold && _carController.speed > requiredSpeed)
        {
            if (!_isDrifting)
            {
                StartDrift();
            }
            UpdateDrift(driftAngle);
            _timeWithoutDrift = 0;
        }
        else
        {
            _timeWithoutDrift += Time.deltaTime;
            
            if (_isDrifting && _timeWithoutDrift > timeToEndDrift)
            {
                Debug.Log("EndDrift");
                EndDrift();
            }
        }
    }
    
    private void StartDrift()
    {
        _isDrifting = true;
    }
    
    private void UpdateDrift(float driftAngle)
    {
        if (_isActivateMultiplier)
        {
            _driftScore += driftAngle * Time.deltaTime * multipliers[_currentMultiplierIndex];
        }
        else
        {
            _driftScore += driftAngle * Time.deltaTime;
        }
        
        CheckForMilestones();
    }
    
    private void CheckForMilestones()
    {
        if (_currentMultiplierIndex < (scoreMilestones.Length - 1) && 
            _driftScore >= scoreMilestones[_currentMultiplierIndex])
        {
            _isActivateMultiplier = true;
            _currentMultiplierIndex++;
        }
    }
    
    private void EndDrift()
    {
        _isDrifting = false;
        _timeWithoutDrift = 0;
        
        // Обновляем данные
        int coinsEarned = Mathf.RoundToInt(_driftScore * 0.5f);
        _driftScoreEarnedCurrentLevel += _driftScore;
        _coinsEarnedPerLevel += coinsEarned;
        _dataModule.Data.coins += coinsEarned;
        _dataModule.Data.recordDriftScore = Mathf.Max(_dataModule.Data.recordDriftScore, Mathf.RoundToInt(_driftScore));
        
        // Сбрасываем состояние
        _currentMultiplierIndex = 0;
        _isActivateMultiplier = false;
        _driftScore = 0;
        
        // Сохраняем
        _dataModule.SaveData();
    }
}