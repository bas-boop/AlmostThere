using System;
using UnityEngine;

public class MapSnapper : MonoBehaviour
{
    [Header("refrences")]
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private RectTransform _map;
    [SerializeField] private float _snap_radius;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform[] icons;

    public event Action<RectTransform> on_snapped;
    public event Action on_released;
    public RectTransform snapped_trasform => _snapped_icon;
    private RectTransform _snapped_icon;

    private void Update()
    {
        if (_snapped_icon == null)
        {
            foreach (var icon in icons)
            {
                Vector2 cursorPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _cursor.position);
                Vector2 iconPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, icon.position);

                float distance = Vector2.Distance(cursorPos, iconPos);

                if (distance < _snap_radius)
                {
                    SnapToIcon(icon, cursorPos, iconPos);
                    break;
                }
            }
        }
        else
        {
            Vector2 cursorPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _cursor.position);
            Vector2 iconPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _snapped_icon.position);

            float distance = Vector2.Distance(cursorPos, iconPos);

            if (distance > _snap_radius)
            {
                _snapped_icon = null;
                on_released?.Invoke();
            }
        }
    }

    private void SnapToIcon(RectTransform icon, Vector2 cursorPos, Vector2 iconPos)
    {
        Vector2 ScreenOffset = cursorPos - iconPos;
        Vector2 localOffset = ScreenOffset / _canvas.scaleFactor;
        
        _map.anchoredPosition += localOffset;

        _snapped_icon = icon;
        on_snapped?.Invoke(icon);

        Debug.Log("snapped to " +  icon.name);
    }
}
