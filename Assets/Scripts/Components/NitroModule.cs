using UnityEngine;

/// <summary>
/// Кастомная система нитро для интеграции с UpgradeModule.
/// Применяет бонусы от прокачки нитро к характеристикам машины.
/// </summary>
public class NitroModule : MonoBehaviour
{
    [Header("Nitro Configuration")]
    [SerializeField] private KeyCode _nitroKey = KeyCode.LeftShift;
    [SerializeField] private float _nitroDuration = 3f;
    [SerializeField] private float _nitroRechargeTime = 10f;
    [SerializeField] private float _nitroTorqueMultiplier = 1.5f;
    [SerializeField] private float _nitroSpeedMultiplier = 1.3f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem[] _nitroEffects;
    [SerializeField] private AudioSource _nitroAudioSource;
    [SerializeField] private AudioClip _nitroSound;
    
    [Header("Car Model Configuration")]
    [SerializeField] private string _carModelName = "SportsCar";
    
    // Компоненты
    private RCC_CarControllerV3 _carController;
    private UpgradeModule _upgradeModule;
    
    // Состояние нитро
    private bool _isNitroActive = false;
    private bool _canUseNitro = true;
    private float _nitroTimer = 0f;
    private float _rechargeTimer = 0f;
    
    // Базовые характеристики (для восстановления)
    private float _originalMaxTorque;
    private float _originalMaxSpeed;
    
    // Текущие бонусы от прокачки
    private float _nitroUpgradeBonus = 0f;
    
    // События
    public System.Action<bool> OnNitroStateChanged;
    public System.Action<float> OnNitroChargeChanged;
    
    public bool IsNitroActive => _isNitroActive;
    public bool CanUseNitro => _canUseNitro;
    public float NitroCharge => _canUseNitro ? 1f : (_rechargeTimer / _nitroRechargeTime);
    public float NitroUpgradeBonus => _nitroUpgradeBonus;
    
    private void Awake()
    {
        _carController = GetComponent<RCC_CarControllerV3>();
        if (_carController == null)
        {
            Debug.LogError("NitroModule requires RCC_CarControllerV3 component!");
            enabled = false;
            return;
        }
        
        // Сохраняем базовые характеристики
        _originalMaxTorque = _carController.maxEngineTorque;
        _originalMaxSpeed = _carController.maxspeed;
    }
    
    private void Start()
    {
        // Получаем модуль прокачки
        _upgradeModule = ModuleManager.Instance?.GetModule<UpgradeModule>();
        
        if (_upgradeModule != null)
        {
            // Применяем бонусы от прокачки нитро
            ApplyNitroUpgrades();
            
            // Подписываемся на события прокачки
            _upgradeModule.OnCarUpgraded += OnCarUpgraded;
        }
        
        // Настраиваем аудио
        SetupAudio();
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        if (_upgradeModule != null)
        {
            _upgradeModule.OnCarUpgraded -= OnCarUpgraded;
        }
    }
    
    private void Update()
    {
        if (_carController == null) return;
        
        HandleInput();
        UpdateNitroState();
    }
    
    private void HandleInput()
    {
        // Активация нитро
        if (Input.GetKeyDown(_nitroKey) && _canUseNitro && !_isNitroActive)
        {
            ActivateNitro();
        }
        
        // Деактивация нитро (опционально - по отпусканию клавиши)
        if (Input.GetKeyUp(_nitroKey) && _isNitroActive)
        {
            // Можно добавить возможность досрочного отключения
            // DeactivateNitro();
        }
    }
    
    private void UpdateNitroState()
    {
        if (_isNitroActive)
        {
            // Обновляем таймер нитро
            _nitroTimer -= Time.deltaTime;
            
            if (_nitroTimer <= 0f)
            {
                DeactivateNitro();
            }
        }
        else if (!_canUseNitro)
        {
            // Обновляем таймер перезарядки
            _rechargeTimer += Time.deltaTime;
            
            if (_rechargeTimer >= _nitroRechargeTime)
            {
                _canUseNitro = true;
                _rechargeTimer = 0f;
                OnNitroChargeChanged?.Invoke(1f);
            }
            else
            {
                OnNitroChargeChanged?.Invoke(_rechargeTimer / _nitroRechargeTime);
            }
        }
    }
    
    private void ActivateNitro()
    {
        if (!_canUseNitro || _isNitroActive) return;
        
        _isNitroActive = true;
        _nitroTimer = _nitroDuration;
        
        // Применяем бонусы нитро
        float totalNitroBonus = 1f + _nitroUpgradeBonus;
        _carController.maxEngineTorque = _originalMaxTorque * _nitroTorqueMultiplier * totalNitroBonus;
        _carController.maxspeed = _originalMaxSpeed * _nitroSpeedMultiplier * totalNitroBonus;
        
        // Визуальные и звуковые эффекты
        ActivateEffects();
        
        // Уведомляем подписчиков
        OnNitroStateChanged?.Invoke(true);
        
        Debug.Log($"Nitro activated! Torque: {_carController.maxEngineTorque:F0}, Speed: {_carController.maxspeed:F0}");
    }
    
    private void DeactivateNitro()
    {
        if (!_isNitroActive) return;
        
        _isNitroActive = false;
        _canUseNitro = false;
        _rechargeTimer = 0f;
        
        // Восстанавливаем базовые характеристики (с учетом прокачки)
        RestoreBaseStats();
        
        // Отключаем эффекты
        DeactivateEffects();
        
        // Уведомляем подписчиков
        OnNitroStateChanged?.Invoke(false);
        OnNitroChargeChanged?.Invoke(0f);
        
        Debug.Log($"Nitro deactivated. Torque restored to: {_carController.maxEngineTorque:F0}");
    }
    
    private void ApplyNitroUpgrades()
    {
        if (_upgradeModule == null) return;
        
        var carUpgrades = _upgradeModule.GetCarUpgrades(_carModelName);
        _nitroUpgradeBonus = carUpgrades.GetCurrentValue(UpgradeModule.UpgradeType.Nitro) / 100f; // Конвертируем в процентный бонус
        
        Debug.Log($"Applied nitro upgrade bonus: {_nitroUpgradeBonus * 100f:F1}%");
    }
    
    private void RestoreBaseStats()
    {
        if (_upgradeModule == null)
        {
            // Если нет модуля прокачки, просто восстанавливаем оригинальные значения
            _carController.maxEngineTorque = _originalMaxTorque;
            _carController.maxspeed = _originalMaxSpeed;
        }
        else
        {
            // Восстанавливаем с учетом прокачки двигателя
            var carUpgrades = _upgradeModule.GetCarUpgrades(_carModelName);
            _carController.maxEngineTorque = carUpgrades.GetCurrentValue(UpgradeModule.UpgradeType.Engine);
            
            // Скорость восстанавливаем к оригинальной (обычно она не прокачивается отдельно)
            _carController.maxspeed = _originalMaxSpeed;
        }
    }
    
    private void ActivateEffects()
    {
        // Включаем партиклы
        if (_nitroEffects != null)
        {
            foreach (var effect in _nitroEffects)
            {
                if (effect != null)
                    effect.Play();
            }
        }
        
        // Воспроизводим звук
        if (_nitroAudioSource != null && _nitroSound != null)
        {
            _nitroAudioSource.clip = _nitroSound;
            _nitroAudioSource.Play();
        }
    }
    
    private void DeactivateEffects()
    {
        // Отключаем партиклы
        if (_nitroEffects != null)
        {
            foreach (var effect in _nitroEffects)
            {
                if (effect != null)
                    effect.Stop();
            }
        }
        
        // Останавливаем звук
        if (_nitroAudioSource != null)
        {
            _nitroAudioSource.Stop();
        }
    }
    
    private void SetupAudio()
    {
        if (_nitroAudioSource == null)
        {
            _nitroAudioSource = gameObject.AddComponent<AudioSource>();
            _nitroAudioSource.playOnAwake = false;
            _nitroAudioSource.loop = false;
            _nitroAudioSource.volume = 0.7f;
        }
    }
    
    private void OnCarUpgraded(string carModelName, UpgradeModule.UpgradeType upgradeType, int newLevel)
    {
        // Обновляем бонусы при прокачке нитро или двигателя
        if (carModelName == _carModelName && (upgradeType == UpgradeModule.UpgradeType.Nitro || upgradeType == UpgradeModule.UpgradeType.Engine))
        {
            ApplyNitroUpgrades();
            
            // Если нитро не активно, обновляем базовые характеристики
            if (!_isNitroActive)
            {
                RestoreBaseStats();
            }
        }
    }
    
    /// <summary>
    /// Устанавливает модель машины для получения прокачек
    /// </summary>
    public void SetCarModel(string carModelName)
    {
        _carModelName = carModelName;
        ApplyNitroUpgrades();
    }
    
    /// <summary>
    /// Принудительно активирует нитро (для внешнего управления)
    /// </summary>
    public void ForceActivateNitro()
    {
        if (_canUseNitro)
            ActivateNitro();
    }
    
    /// <summary>
    /// Принудительно деактивирует нитро
    /// </summary>
    public void ForceDeactivateNitro()
    {
        if (_isNitroActive)
            DeactivateNitro();
    }
    
    /// <summary>
    /// Мгновенно перезаряжает нитро (для читов/бонусов)
    /// </summary>
    public void InstantRecharge()
    {
        _canUseNitro = true;
        _rechargeTimer = 0f;
        OnNitroChargeChanged?.Invoke(1f);
    }
}
