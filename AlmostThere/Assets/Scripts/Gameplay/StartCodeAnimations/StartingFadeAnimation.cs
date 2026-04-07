using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gameplay.StartCodeAnimation
{
    public class StartingFadeAnimation : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField, Range (0, 1.5f)] private float duration;
        [SerializeField] private LeanTweenType fadeType;
        [SerializeField] private StartingBuildingAnimations StartingBuildingAnimations;

        private float fadeInAmount = 1f;
        private float fadeOutAmount = 0f;

        //removed the addlistener as you implied (bas), the fade logic works but at the moment its not linked to the animation because of this removal
        public void FadeIn()
        {
            LeanTween.alphaCanvas(fadePanel, fadeInAmount, duration).setEase(fadeType).setOnComplete(() =>
            {
                fadePanel.alpha = fadeInAmount;
            });
        }

        public void FadeOut()
        {
            LeanTween.alphaCanvas(fadePanel, fadeOutAmount, duration).setEase(fadeType).setOnComplete(() =>
            {
                fadePanel.alpha = fadeOutAmount;
            });
        }
    }
}
