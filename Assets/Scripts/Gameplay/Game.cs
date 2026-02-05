using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Gameplay
{
    public sealed class Game : MonoBehaviour
    {
        [SerializeField] private GameObject timer;
        [SerializeField] private GameObject buttons;
        [SerializeField] private GameObject nextButton;
        
        [Header("text")]
        [SerializeField] private GameObject startText;
        [SerializeField] private GameObject gameText;
        [SerializeField] private GameObject winText;
        [SerializeField] private GameObject busText;
        [SerializeField] private GameObject bus2Text;
        [SerializeField] private GameObject walkText;
        [SerializeField] private GameObject walk2Text;
        [SerializeField] private GameObject bikeText;
        [SerializeField] private GameObject bike2Text;

        [Header("time")]
        [SerializeField] private float showStartTime;
        [SerializeField] private float chooseTime;
        [SerializeField] private float winTime;

        private void Start() => StartCoroutine(Begin());

        public void BusSequence() => StartCoroutine(Bus());
        public void WalkSequence() => StartCoroutine(Walk());
        public void BikeSequence() => StartCoroutine(Bike());

        public void No()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        private IEnumerator Begin()
        {
            yield return new WaitForSeconds(showStartTime);
            startText.SetActive(false);
            gameText.SetActive(true);
            timer.SetActive(true);
            buttons.SetActive(true);
        }

        private IEnumerator Walk()
        {
            buttons.SetActive(false);
            gameText.SetActive(false);
            walkText.SetActive(true);
            yield return new WaitForSeconds(chooseTime);
            walkText.SetActive(false);
            walk2Text.SetActive(true);
            timer.SetActive(false);
            yield return new WaitForSeconds(winTime);
            walk2Text.SetActive(false);
            winText.SetActive(true);
            nextButton.SetActive(true);
        }

        private IEnumerator Bus()
        {
            buttons.SetActive(false);
            gameText.SetActive(false);
            busText.SetActive(true);
            yield return new WaitForSeconds(chooseTime);
            busText.SetActive(false);
            bus2Text.SetActive(true);
            timer.SetActive(false);
            yield return new WaitForSeconds(winTime);
            bus2Text.SetActive(false);
            winText.SetActive(true);
            nextButton.SetActive(true);
        }

        private IEnumerator Bike()
        {
            buttons.SetActive(false);
            gameText.SetActive(false);
            bikeText.SetActive(true);
            yield return new WaitForSeconds(chooseTime);
            bikeText.SetActive(false);
            bike2Text.SetActive(true);
            timer.SetActive(false);
            yield return new WaitForSeconds(winTime);
            bike2Text.SetActive(false);
            winText.SetActive(true);
            nextButton.SetActive(true);
        }
    }
}