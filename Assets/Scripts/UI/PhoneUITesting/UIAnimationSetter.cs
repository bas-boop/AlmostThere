using System;
using System.Collections;
using UnityEngine;

namespace AlmostThere.UI
{
    public class UIAnimationSetter : MonoBehaviour
    {
        #region Public fields

        [Tooltip("Smooth ease in and out curve.")]
        public AnimationCurve easeInOut;

        [Tooltip("Curve that overshoots the target value.")]
        public AnimationCurve easeOvershoot;

        [Tooltip("Curve that anticipates before moving.")]
        public AnimationCurve easeAnticipate;

        [Tooltip("Curve that shakes back and forth.")]
        public AnimationCurve easeShake;

        #endregion

        #region Public methods

        /// <summary>
        /// Animates the anchored position of a RectTransform using an AnimationCurve.
        /// </summary>
        /// <param name="objectToAnimate">The RectTransform to animate.</param>
        /// <param name="easingType">The curve to evaluate.</param>
        /// <param name="fromAmount">Start position.</param>
        /// <param name="toAmount">End position.</param>
        /// <param name="timer">Current progress from 0 to 1.</param>
        public void AnimatePosition(
            RectTransform objectToAnimate,
            AnimationCurve easingType,
            Vector2 fromAmount,
            Vector2 toAmount,
            float timer)
        {
            float curve = easingType.Evaluate(timer);
            objectToAnimate.anchoredPosition = Vector2.LerpUnclamped(fromAmount, toAmount, curve);
        }

        /// <summary>
        /// Animates the local scale of a RectTransform using an AnimationCurve.
        /// </summary>
        /// <param name="objectToAnimate">The RectTransform to animate.</param>
        /// <param name="easingType">The curve to evaluate.</param>
        /// <param name="fromAmount">Start scale.</param>
        /// <param name="toAmount">End scale.</param>
        /// <param name="timer">Current progress from 0 to 1.</param>
        public void AnimateScale(
            RectTransform objectToAnimate,
            AnimationCurve easingType,
            Vector2 fromAmount,
            Vector2 toAmount,
            float timer)
        {
            float curve = easingType.Evaluate(timer);
            objectToAnimate.localScale = Vector3.LerpUnclamped(fromAmount, toAmount, curve);
            //objectToAnimate.localScale = Vector3.one * curve;
        }

        /// <summary>
        /// Animates the rotation of a RectTransform using an AnimationCurve.
        /// </summary>
        /// <param name="objectToAnimate">The RectTransform to animate.</param>
        /// <param name="easingType">The curve to evaluate.</param>
        /// <param name="fromAmount">Start rotation.</param>
        /// <param name="toAmount">End rotation.</param>
        /// <param name="timer">Current progress from 0 to 1.</param>
        public void AnimateRotation(
            RectTransform objectToAnimate,
            AnimationCurve easingType,
            Quaternion fromAmount,
            Quaternion toAmount,
            float timer)
        {
            float curve = easingType.Evaluate(timer);
            objectToAnimate.rotation = Quaternion.LerpUnclamped(fromAmount, toAmount, curve);
        }

        /// <summary>
        /// Infinitely rotates a RectTransform around the Z axis.
        /// </summary>
        /// <param name="objectToAnimate">The RectTransform to rotate.</param>
        /// <param name="rotateSpeed">Degrees per second.</param>
        public IEnumerator RotateAroundZ(RectTransform objectToAnimate, float rotateSpeed)
        {
            float angle = objectToAnimate.eulerAngles.z;

            while (true)
            {
                angle += rotateSpeed * Time.deltaTime;
                objectToAnimate.rotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }
        }

        /// <summary>
        /// Animates the scale of a RectTransform using a curve directly as the scale value.
        /// </summary>
        /// <param name="objectToAnimate">The RectTransform to animate.</param>
        /// <param name="easingType">The curve whose evaluated value is used as the scale.</param>
        /// <param name="toAmount">The final scale to snap to after the animation.</param>
        /// <param name="time">Duration of the animation in seconds.</param>
        /// <param name="onComplete">Optional callback when the animation finishes.</param>
        public IEnumerator ScaleFromCurve(
            RectTransform objectToAnimate,
            AnimationCurve easingType,
            Vector2 toAmount,
            float time,
            Action onComplete = null)
        {
            float timer = 0;

            while (timer < 1)
            {
                timer += Time.deltaTime / time;

                float curve = easingType.Evaluate(timer);

                objectToAnimate.localScale = Vector3.one * curve;
                yield return null;
            }

            objectToAnimate.localScale = toAmount;
            onComplete?.Invoke();
        }

        #endregion
    }
}