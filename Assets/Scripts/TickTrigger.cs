using System;
using UnityEngine;

public class TickTrigger : MonoBehaviour
{
    #region Enums

    private enum TickState
    {
        Moving,
        InsideBall,
        ExitingBall
    }
    #endregion

    #region Public Properties

    public bool InsideBall => currentState == TickState.InsideBall;


    public bool MissedBall => currentState == TickState.ExitingBall;

    public Action OnTickExit;
    #endregion

    #region Private Fields
    private TickState currentState = TickState.Moving;
    #endregion

    #region Public Methods

    public void Reset()
    {
        currentState = TickState.Moving;
    }
    #endregion

    #region Unity Collision Events

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentState = TickState.InsideBall;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentState = TickState.ExitingBall;
        OnTickExit?.Invoke();
    }
    #endregion
}