using System;
using System.Collections;
using UnityEngine;

public class LockHoopAnimation : MonoBehaviour
{
    #region Public Fields
    [Header("Animation Settings")]
    public AnimationCurve heightCurve;
    public Transform child;
    #endregion

    #region Public Methods

    public void Reset()
    {
        child.localPosition = Vector3.zero;
    }

    public void ShowUnlock(Action onFinished, float duration)
    {
        StartCoroutine(UnlockRoutine(onFinished, duration));
    }
    #endregion

    #region Private Methods

    private IEnumerator UnlockRoutine(Action onFinished, float duration)
    {
        float normalizedTime = 0f;

        while (normalizedTime < 1f)
        {
            float currentHeight = heightCurve.Evaluate(normalizedTime);
            child.localPosition = new Vector3(0, currentHeight, 0);

            normalizedTime += Time.deltaTime / duration;  // Zamanı süreye göre normalize eder, animasyon eğrisini doğru hızda uygular.
            yield return null;
        }

        child.localPosition = new Vector3(0, heightCurve.Evaluate(1f), 0);
        onFinished?.Invoke();
    }
    #endregion
}