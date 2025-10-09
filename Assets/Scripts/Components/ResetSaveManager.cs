using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Менеджер для сброса сохранений игры по нажатию клавиши R
/// Добавьте этот компонент на любой GameObject в сцене (например, на GameController)
/// </summary>
public class ResetSaveManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode _resetKey = KeyCode.R;
    [SerializeField] private bool _requireConfirmation = true;
    [SerializeField] private bool _reloadSceneAfterReset = true;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages = true;
    
    private bool _waitingForConfirmation = false;
    private float _confirmationTimeout = 3f;
    private float _confirmationTimer = 0f;
    
    private void Update()
    {
        HandleResetInput();
    }
    
    private void HandleResetInput()
    {
        // Первое нажатие R
        if (!_waitingForConfirmation && Input.GetKeyDown(_resetKey))
        {
            if (_requireConfirmation)
            {
                StartConfirmation();
            }
            else
            {
                PerformReset();
            }
        }
        // Повторное нажатие R для подтверждения
        else if (_waitingForConfirmation && Input.GetKeyDown(_resetKey))
        {
            PerformReset();
        }
        
        // Таймер подтверждения
        if (_waitingForConfirmation)
        {
            _confirmationTimer -= Time.deltaTime;
            
            if (_confirmationTimer <= 0f)
            {
                CancelConfirmation();
            }
        }
    }
    
    private void StartConfirmation()
    {
        _waitingForConfirmation = true;
        _confirmationTimer = _confirmationTimeout;
        
        if (_showDebugMessages)
        {
            Debug.LogWarning($"⚠️ ВНИМАНИЕ! Нажмите R еще раз в течение {_confirmationTimeout} секунд для сброса ВСЕХ сохранений!");
        }
        
        // Можно добавить UI уведомление
        ShowConfirmationUI();
    }
    
    private void CancelConfirmation()
    {
        _waitingForConfirmation = false;
        
        if (_showDebugMessages)
        {
            Debug.Log("Сброс отменен");
        }
        
        HideConfirmationUI();
    }
    
    private void PerformReset()
    {
        _waitingForConfirmation = false;
        
        if (_showDebugMessages)
        {
            Debug.LogWarning("🔥 СБРОС ВСЕХ СОХРАНЕНИЙ! 🔥");
        }
        
        // Получаем DataModule
        var dataModule = ModuleManager.Instance?.GetModule<DataModule>();
        
        if (dataModule != null)
        {
            // Вызываем сброс данных
            dataModule.ResetData();
            
            if (_showDebugMessages)
            {
                Debug.Log("✅ Все сохранения успешно удалены!");
            }
            
            // Перезагружаем сцену если нужно
            if (_reloadSceneAfterReset)
            {
                ReloadCurrentScene();
            }
        }
        else
        {
            Debug.LogError("❌ DataModule не найден! Убедитесь что ModuleManager инициализирован.");
        }
        
        HideConfirmationUI();
    }
    
    private void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (_showDebugMessages)
        {
            Debug.Log($"🔄 Перезагрузка сцены: {currentSceneName}");
        }
        
        SceneManager.LoadScene(currentSceneName);
    }
    
    private void ShowConfirmationUI()
    {
        // TODO: Показать UI уведомление
        // Например:
        // UIManager.ShowMessage("Нажмите R еще раз для подтверждения сброса");
    }
    
    private void HideConfirmationUI()
    {
        // TODO: Скрыть UI уведомление
        // Например:
        // UIManager.HideMessage();
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Test - Reset Save Data")]
    private void TestReset()
    {
        Debug.Log("Тестовый сброс сохранений из Editor...");
        _requireConfirmation = false;
        PerformReset();
    }
    
    [ContextMenu("Show Current Save Data")]
    private void ShowCurrentData()
    {
        var dataModule = ModuleManager.Instance?.GetModule<DataModule>();
        
        if (dataModule != null)
        {
            Debug.Log("=== Текущие сохранения ===");
            Debug.Log($"Монеты: {dataModule.Data.coins}");
            Debug.Log($"Рекорд дрифта: {dataModule.Data.recordDriftScore}");
            Debug.Log($"Выбранная машина: {dataModule.Data.appliedCarIndex}");
            Debug.Log($"Куплено машин: {dataModule.Data.isBuyShop.FindAll(x => x).Count}/5");
            
            for (int i = 0; i < dataModule.Data.isBuyShop.Count; i++)
            {
                Debug.Log($"  Машина {i}: {(dataModule.Data.isBuyShop[i] ? "✅ Куплена" : "❌ Не куплена")}");
            }
        }
    }
    #endif
}

