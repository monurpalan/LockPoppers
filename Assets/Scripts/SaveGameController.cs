using UnityEngine;
using System.IO;

public class SaveGameController : MonoBehaviour
{
    #region Constants
    private const string SAVE_FILE_NAME = "savegame.json";
    #endregion

    #region Public Properties

    public bool IsReady { get; private set; }

    public int CurrentLevel { get; private set; }
    #endregion

    #region Private Fields
    private string saveFilePath;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSaveSystem();
        LoadGameData();
    }
    #endregion

    #region Public Methods

    public void SaveProgress(int level)
    {
        try
        {
            CurrentLevel = level;
            var saveGame = new SaveGame { currentLevel = CurrentLevel };
            string json = JsonUtility.ToJson(saveGame, prettyPrint: true);  // SaveGame nesnesini JSON string'e dönüştürür, prettyPrint ile okunaklı hale getirir.
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"Game progress saved: Level {CurrentLevel}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game progress: {e.Message}");
        }
    }
    #endregion

    #region Private Methods

    private void InitializeSaveSystem()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);  // Kalıcı veri yolunu SAVE_FILE_NAME ile birleştirerek dosya yolunu oluşturur.
        IsReady = true;
    }

    private void LoadGameData()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                var saveGame = JsonUtility.FromJson<SaveGame>(json);  // JSON string'i SaveGame nesnesine dönüştürür.
                CurrentLevel = saveGame.currentLevel;

                Debug.Log($"Game data loaded: Level {CurrentLevel}");
            }
            else
            {
                CurrentLevel = 1;
                Debug.Log("No save file found, starting at level 1");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            CurrentLevel = 1;
        }
    }
    #endregion
}