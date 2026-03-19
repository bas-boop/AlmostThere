using System.Collections;
using UnityEngine;

public class UIAnimationSetter : MonoBehaviour
{
    public static UIAnimationSetter Instance;

    [Header("Easing Types")]
    public AnimationCurve ease_in_out;
    public AnimationCurve ease_overshoot;
    public AnimationCurve ease_anticipate;
    public AnimationCurve ease_shake;
    public void SimpleUIAnimationPosition(RectTransform objectToAnimate, AnimationCurve easingType, Vector2 fromAmount, Vector2 To_amount, float timer/*, float timerMult)*/)
    {
        float curve = easingType.Evaluate(timer);

        objectToAnimate.anchoredPosition = Vector2.LerpUnclamped(fromAmount, To_amount, curve);
    }

    public void SimpleUIAnimationScale(RectTransform objectToAnimate, AnimationCurve easingType, Vector2 from_amount, Vector2 to_amount, float timer/*, float timerMult)*/)
    {
        float curve = easingType.Evaluate(timer);

        objectToAnimate.localScale = Vector3.LerpUnclamped(from_amount, to_amount, curve);
    }

    public void SimpleUIAnimationRotation(RectTransform objectToAnimate, AnimationCurve easingType, Quaternion from_amount, Quaternion to_amount, float timer/*, float timerMult)*/)
    {
        float curve = easingType.Evaluate(timer);

        objectToAnimate.rotation = Quaternion.LerpUnclamped(from_amount, to_amount, curve);
    }

    public IEnumerator SimpleUIAnimationRotateAroundZ(RectTransform objectToAnimate, float rotateSpeed)
    {
        float angle = objectToAnimate.rotation.z;

        while (true)
        {
            angle += rotateSpeed * Time.deltaTime;
            objectToAnimate.rotation = Quaternion.Euler(0,0,angle);
            yield return null;
        }
    }
    public IEnumerator ScaleUpAndDown(RectTransform objectToAnimate, AnimationCurve easingType, Vector2 to_amount, float time, System.Action onComplete = null)
    {
        float timer = 0;

        Vector2 orgScale = objectToAnimate.localScale;

        while (timer < 1)
        {
            timer += Time.deltaTime / time;

            float curve = easingType.Evaluate(timer);

            objectToAnimate.localScale = Vector3.one * curve;
            yield return null;
        }

        objectToAnimate.localScale = to_amount;
        onComplete?.Invoke();
    }
}
