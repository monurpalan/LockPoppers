using System;
using UnityEngine;

public class RotateTick : MonoBehaviour
{
    #region Public Fields
    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation in degrees per second")]
    public float angleSpeed = -45f;

    [Tooltip("Direction multiplier for rotation (1 or -1)")]
    public float direction = 1f;
    #endregion

    #region Public Properties

    public bool InsideBall => tickTrigger != null && tickTrigger.InsideBall;

    public float ZRotation => transform.rotation.eulerAngles.z;

    public Action OnMissedTheBall;
    #endregion

    #region Private Fields
    private bool isRotating = true;
    private TickTrigger tickTrigger;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeTickTrigger();
    }

    private void Update()
    {
        if (isRotating)
        {
            PerformRotation();
        }
    }
    #endregion

    #region Public Methods

    public void Rotate(bool shouldRotate)
    {
        isRotating = shouldRotate;
    }


    public void Reset()
    {
        tickTrigger?.Reset();
    }
    #endregion

    #region Private Methods

    private void InitializeTickTrigger()
    {
        tickTrigger = GetComponentInChildren<TickTrigger>();
        if (tickTrigger != null)
        {
            tickTrigger.OnTickExit = HandleMissedBall;
        }
        else
        {
            Debug.LogWarning("RotateTick: No TickTrigger found in children!");
        }
    }

    private void PerformRotation()
    {
        float rotationAmount = angleSpeed * Time.deltaTime * direction;  // Dönüş miktarını hesaplar, deltaTime ile frame-independent hale getirir.
        transform.Rotate(Vector3.forward, rotationAmount);
    }

    private void HandleMissedBall()
    {
        OnMissedTheBall?.Invoke();
    }
    #endregion
}