using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.StartCodeAnimation
{
    public class StartingBuildingAnimations : MonoBehaviour
    {
        public bool isStartingPoint = false;
        [SerializeField] private LeanTweenType tweeningType = LeanTweenType.easeOutCubic;
        [SerializeField, Range(1, 3)] private int amount = 2;
        [SerializeField, Range(0, 2)] private float duration = .4f;
        [SerializeField, Range(0, 2)] private float yScaleToMult = 1.2f;
        [SerializeField, Range(0, 2)] private float xZScaleToMult = 0.8f;

        //Deze action moet in een ander script uiteindelijk
        public UnityEvent onStart = new UnityEvent();


        public UnityEvent onAnimationComplete = new UnityEvent();

        private Vector3 _orgscale;

        private int loopedAmount;

        private void Start()
        {
            //voor nu een test start functie, dit moet uiteindelijk in een ander script

            //zet dit event in een ander script om het aan te roepen
            onStart.AddListener(SquishAndStretch);

            _orgscale = transform.localScale;
            loopedAmount = 0;

            if (isStartingPoint)
            {
                onStart?.Invoke();
            }
        }

        public void SquishAndStretch()
        {
            LeanTween.scale(gameObject, new Vector3(_orgscale.x * xZScaleToMult, _orgscale.y * yScaleToMult, _orgscale.z * xZScaleToMult), duration).setEase(tweeningType).setOnComplete(() =>
            {
                LeanTween.scale(gameObject, new Vector3(_orgscale.x / xZScaleToMult, _orgscale.y / yScaleToMult, _orgscale.z / xZScaleToMult), duration).setEase(tweeningType).setOnComplete(() =>
                {
                    if (loopedAmount <= amount)
                    {
                        loopedAmount++;
                        SquishAndStretch();
                    }
                    else
                    {
                        LeanTween.scale(gameObject, new Vector3(_orgscale.x, _orgscale.y, _orgscale.z), duration).setEase(tweeningType);
                        onAnimationComplete.Invoke();
                    }
                });
            });
        }
    }
}