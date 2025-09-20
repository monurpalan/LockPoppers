using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Enums
    private enum GameState
    {
        Playing,
        Interstitial
    }
    #endregion

    #region Constants
    private const float BALL_FADE_TIME = 0.25f;
    private const float SHAKE_DURATION = 0.5f;
    private const float SHAKE_INTENSITY = 0.5f;
    #endregion

    #region Public Fields
    [Header("Game Objects")]
    public Transform lockBody;
    public RotateBall ballPivot;
    public RotateTick tickPivot;

    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI dialText;

    [Header("Animations")]
    public LockHoopAnimation lockHoop;

    [Header("Game Settings")]
    public float tickSpeed = 60f;
    #endregion

    #region Private Fields
    private SaveGameController saveGameController;
    private GameState currentState = GameState.Interstitial;
    private float rotationDirection = 1f;
    private int currentLevel = 1;
    private int remainingTaps = 1;
    #endregion

    #region Unity Lifecycle
    private IEnumerator Start()
    {
        InitializeSaveSystem();
        yield return WaitForSaveSystemReady();  // Save sisteminin hazır olmasını bekler, hazır olana kadar null yield eder.

        LoadGameData();
        SetupGameComponents();
        UpdateUI();
        currentState = GameState.Interstitial;
    }
    #endregion

    #region Input Handling
    private void OnMouseDown()
    {
        if (currentState == GameState.Interstitial)
        {
            StartNewRound();
            return;
        }

        if (!tickPivot.InsideBall)
        {
            HandleMissedTap();
            return;
        }

        ProcessSuccessfulTap();
    }
    #endregion

    #region Game Logic
    private void StartNewRound()
    {
        ballPivot.StartFadeIn(ShowTick, BALL_FADE_TIME, 1f, tickPivot.ZRotation);
        currentState = GameState.Playing;
    }

    private void ProcessSuccessfulTap()
    {
        remainingTaps--;

        if (remainingTaps <= 0)
        {
            CompleteLevel();
        }
        else
        {
            PrepareNextTap();
        }
    }

    private void CompleteLevel()
    {
        tickPivot.Rotate(false);
        ballPivot.StartFadeOut(DoCelebration, BALL_FADE_TIME, 1f, tickPivot.ZRotation);
    }

    private void PrepareNextTap()
    {
        currentState = GameState.Interstitial;
        rotationDirection *= -1f;
        tickPivot.direction = rotationDirection;
        ballPivot.StartFadeOut(OnBallFadedOut, BALL_FADE_TIME, 1f, tickPivot.ZRotation);
        UpdateUI();
    }

    private void HandleMissedTap()
    {
        StartCoroutine(ShakeLock());
    }
    #endregion

    #region Animation Callbacks
    private void ShowTick()
    {
        tickPivot.Rotate(true);
        tickPivot.angleSpeed = tickSpeed;
        tickPivot.gameObject.SetActive(true);
        currentState = GameState.Playing;
    }

    private void DoCelebration()
    {
        currentState = GameState.Interstitial;
        tickPivot.gameObject.SetActive(false);
        currentLevel++;
        lockHoop.ShowUnlock(() => OnUnlockFinished(), 1f);  // Unlock animasyonu tamamlandığında OnUnlockFinished'i çağırır.
    }

    private void OnUnlockFinished()
    {
        ResetGameComponents();
        saveGameController.SaveProgress(currentLevel);
        remainingTaps = currentLevel;
        UpdateUI();
    }

    private void OnBallFadedOut()
    {
        tickPivot.Reset();
        ballPivot.StartFadeIn(ShowTick, BALL_FADE_TIME, 1f, tickPivot.ZRotation);
    }

    private void OnMissedTheBall()
    {
        if (currentState == GameState.Interstitial)
            return;
        StartCoroutine(ShakeLock());
    }
    #endregion

    #region UI Management
    private void UpdateUI()
    {
        levelText.text = $"Level {currentLevel}";
        dialText.text = remainingTaps.ToString();
    }
    #endregion

    #region Helper Methods
    private void InitializeSaveSystem()
    {
        saveGameController = GetComponent<SaveGameController>();
    }

    private IEnumerator WaitForSaveSystemReady()
    {
        while (!saveGameController.IsReady)
            yield return null;
    }

    private void LoadGameData()
    {
        currentLevel = saveGameController.CurrentLevel;
        remainingTaps = currentLevel;
    }

    private void SetupGameComponents()
    {
        tickPivot.OnMissedTheBall = OnMissedTheBall;
    }

    private void ResetGameComponents()
    {
        tickPivot.Reset();
        lockHoop.Reset();
    }
    #endregion

    #region Visual Effects
    private IEnumerator ShakeLock()
    {
        currentState = GameState.Interstitial;
        tickPivot.Rotate(false);
        tickPivot.gameObject.SetActive(false);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            float shakeX = Random.Range(-SHAKE_INTENSITY, SHAKE_INTENSITY);
            lockBody.localPosition = new Vector3(shakeX, 0f, 0f);
            elapsed += Time.deltaTime / SHAKE_DURATION;  // Zamanı SHAKE_DURATION'a göre normalize eder, sarsıntı efekti için.
            yield return null;
        }

        lockBody.localPosition = Vector3.zero;
        OnUnlockFinished();
    }
    #endregion
}