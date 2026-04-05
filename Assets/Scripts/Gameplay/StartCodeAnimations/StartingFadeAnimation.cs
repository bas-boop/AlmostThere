using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.StartCodeAnimation
{
    public class StartingFadeAnimation : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField, Range(0,1)] private float fadeTo;
        [SerializeField, Range (0, 1.5f)] private float duration;
        [SerializeField] private LeanTweenType fadeType;
        [SerializeField] private StartingBuildingAnimations StartingBuildingAnimations;

        public event Action onFadeComplete;

        private void OnEnable()
        {
            if (StartingBuildingAnimations != null)
                StartingBuildingAnimations.onAnimationComplete += FadeInOut;
        }

        private void FadeInOut()
        {
            LeanTween.alphaCanvas(fadePanel, fadeTo, duration).setEase(fadeType).setOnComplete(() =>
            {
                fadePanel.alpha = fadeTo;
                onFadeComplete?.Invoke();
            });
        }
    }
}
