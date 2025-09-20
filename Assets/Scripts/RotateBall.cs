using System;
using System.Collections;
using UnityEngine;

public class RotateBall : MonoBehaviour
{
    #region Public Fields
    [Header("Fade In Animation")]
    public AnimationCurve scaleIn;
    public AnimationCurve opacityIn;

    [Header("Fade Out Animation")]
    public AnimationCurve scaleOut;
    public AnimationCurve opacityOut;
    #endregion

    #region Public Methods

    public void StartFadeIn(Action onFadeComplete, float duration, float direction, float tickRotation)
    {
        StartCoroutine(FadeRoutine(onFadeComplete, scaleIn, opacityIn, duration, direction, tickRotation, true));
    }

    public void StartFadeOut(Action onFadeComplete, float duration, float direction, float tickRotation)
    {
        StartCoroutine(FadeRoutine(onFadeComplete, scaleOut, opacityOut, duration, direction, tickRotation, false));
    }
    #endregion

    #region Private Methods

    private IEnumerator FadeRoutine(
        Action onFadeComplete,
        AnimationCurve scaleCurve,
        AnimationCurve opacityCurve,
        float duration,
        float direction,
        float tickRotation,
        bool shouldMoveBall)
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        var ballTransform = spriteRenderer.transform;

        if (shouldMoveBall)
        {
            PositionBallForAnimation(direction, tickRotation);
        }

        yield return StartCoroutine(AnimateFade(spriteRenderer, ballTransform, scaleCurve, opacityCurve, duration));

        onFadeComplete?.Invoke();
    }

    private void PositionBallForAnimation(float direction, float tickRotation)
    {
        float randomOffset = UnityEngine.Random.Range(0, 45) * direction;  // Yönüne göre rastgele bir açı ofseti ekler (0-45 derece arası).
        float rotationZ = tickRotation + 45 + randomOffset;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    private IEnumerator AnimateFade(
        SpriteRenderer spriteRenderer,
        Transform ballTransform,
        AnimationCurve scaleCurve,
        AnimationCurve opacityCurve,
        float duration)
    {
        float time = 0f;
        while (time < 1f)
        {
            float currentScale = scaleCurve.Evaluate(time);
            float currentOpacity = opacityCurve.Evaluate(time);

            ballTransform.localScale = Vector3.one * currentScale;
            spriteRenderer.color = new Color(1, 1, 1, currentOpacity);

            time += Time.deltaTime / duration;  // Zamanı süreye göre normalize eder, animasyon eğrisini doğru hızda uygular.
            yield return null;
        }
    }
    #endregion
}