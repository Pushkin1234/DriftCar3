using UnityEngine;
using YG;
using YG.Utils;
using YG.Insides;

public class YandexModule : BaseGameModule
{
    public override string ModuleName => "Yandex";
    
    private DataModule _dataModule;
    
    public override void Initialize()
    {
        _dataModule = ModuleManager.Instance.GetModule<DataModule>();
        base.Initialize();
    }
    
    public void SaveToYandex()
    {
        //Интеграция с Яндекс.Игры SDK
        if (YG2.isSDKEnabled)
        {
            //Сохранение в облако через YG2 API
            YG2.SaveProgress();
        }
    }
    
    public void LoadFromYandex()
    {
        if (YG2.isSDKEnabled)
        {
            // Загрузка из облака через YG2 API
            YGInsides.LoadProgress();
        }
    }
    
    public void SendScoreToLeaderboard(int score)
    {
        if (YG2.isSDKEnabled)
        {
            //YG2.leaderboard.SetScore("drift_leaderboard", score);
        }
    }
    
    // Альтернативные методы с использованием LocalStorage
    public void SaveToLocalStorage()
    {
        if (YG2.isSDKEnabled)
        {
            // Сохранение в локальное хранилище
            string jsonData = JsonUtility.ToJson(_dataModule.Data);
            LocalStorage.SetKey("GameData", jsonData);
        }
    }
    
    public void LoadFromLocalStorage()
    {
        if (YG2.isSDKEnabled)
        {
            // Загрузка из локального хранилища
            if (LocalStorage.HasKey("GameData"))
            {
                string jsonData = LocalStorage.GetKey("GameData");
                var gameData = JsonUtility.FromJson<DataModule.GameData>(jsonData);
                if (gameData != null)
                {
                    // Применяем загруженные данные
                    _dataModule.Data.coins = gameData.coins;
                    _dataModule.Data.recordDriftScore = gameData.recordDriftScore;
                    _dataModule.Data.appliedCarIndex = gameData.appliedCarIndex;
                    _dataModule.Data.muteMusic = gameData.muteMusic;
                    _dataModule.Data.isBuyShop = gameData.isBuyShop;
                }
            }
        }
    }
}