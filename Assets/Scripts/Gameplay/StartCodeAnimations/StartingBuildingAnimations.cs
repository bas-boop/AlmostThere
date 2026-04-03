using UnityEngine;

public class StartingBuildingAnimations : MonoBehaviour
{
    public bool isStartingPoint = false;
    [SerializeField] private LeanTweenType tweeningType = LeanTweenType.easeOutCubic;
    [SerializeField, Range(1, 3)] private int amount = 2;
    [SerializeField, Range(0, 2)] private float duration = .4f;
    [SerializeField, Range(0, 2)] private float yScaleToMult = 1.2f;
    [SerializeField, Range(0, 2)] private float xZScaleToMult = 0.8f;

    private Vector3 _orgscale;

    private int loopedAmount;

    private void Start()
    {
        _orgscale = transform.localScale;
        loopedAmount = 0;

        if (isStartingPoint)
        {
            SquishAndStretch();
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
                }
            });
        });
    }
}