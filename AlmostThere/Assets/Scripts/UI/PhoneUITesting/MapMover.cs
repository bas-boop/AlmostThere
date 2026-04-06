using System.Collections;
using UnityEngine;

namespace UI.Phonetesting
{
    public class MapMover : MonoBehaviour
    {
        [SerializeField] private PhoneUIIndicator indicator;
        [SerializeField] private MapSnapper mapSnapper;
        [SerializeField] private RectTransform mapPlaceholder;
        [SerializeField, Range(150, 350)] private float mapMoveSpeed = 175f;
        [SerializeField, Range(.2f, 1f)] private float movementDisabledTime = .3f;

        private bool disableMovementDirection = false;  

        private void Start()
        {
            mapSnapper.isSnapped += DisableMoveMomentOnSnapped;
        }

        public void SetMoveDirection(Vector2 direction)
        {
            if (disableMovementDirection)
            {
                direction = Vector2.zero;
            }

            if (direction != Vector2.zero)
                mapPlaceholder.anchoredPosition -= direction * (mapMoveSpeed * Time.deltaTime);

            indicator.CanUpdate();
        }

        private void DisableMoveMomentOnSnapped()
        {
            StartCoroutine(TemporaryDisableMovement());
        }

        private IEnumerator TemporaryDisableMovement()
        {
            disableMovementDirection = true;

            yield return new WaitForSeconds(movementDisabledTime);

            disableMovementDirection = false;
        }
    }
}
