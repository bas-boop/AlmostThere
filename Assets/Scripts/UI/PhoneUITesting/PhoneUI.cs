using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    private enum IndicatorState { Hidden, Visible, Hiding, NearIcon }
    private IndicatorState _indicatorState = IndicatorState.Hidden;

    [Header("References")]
    [SerializeField] private UIStateMachine _StateMachine;
    [SerializeField] private UIAnimationSetter UIAnimations;
    [SerializeField] private Image _phone_icon;
    [SerializeField] private RectTransform _phone_icon_holder;
    [SerializeField] private RectTransform _phone_input_holder;
    [Space(10)]
    [SerializeField] private RectTransform _staringNotification;
    [SerializeField] private RectTransform _map_placeholder;
    [SerializeField] private MapSnapper _map_snapper;
    [SerializeField] private RectTransform _icon_placeholder;
    [SerializeField] private RectTransform _MapParent;
    [SerializeField] private RectTransform _PlayerIcon;
    [SerializeField] private RectTransform _MapMask;
    [SerializeField] private RectTransform _OffscreenIndicator;
    [SerializeField] private RectTransform _OffscreenIndicatorBackground;
    [SerializeField] private RectTransform _OffscreenIndicatorSpinner;
    [SerializeField] private RectTransform _Cursor;
    [SerializeField] private Canvas _Canvas;
    [Space(10)]
    [SerializeField] private RectTransform _RoutePlannerPanel;
    [Space(10)]
    [SerializeField] private RectTransform _phone_transform;
    [SerializeField] private RectTransform _phone_open_transform;
    [SerializeField] private RectTransform _phone_closed_transform;
    [Space(10)]
    [SerializeField] private PlayerInput _playerInput;


    [Tooltip("De telefoon is al een keer gebruikt")]
    [SerializeField] private bool _phone_has_been_openend = false;

    [Header("Animatie settings")]
    [Header("Phone Icon")]
    [SerializeField] private float _phone_icon_animation_speed;
    [SerializeField] private float _phone_icon_rotation_amount = 10f;
    [SerializeField] private float _phone_icon_scale_amount = 1.2f;
    [Space(10)]
    [SerializeField] private float _phone_icon_interval = 2f;
    [SerializeField] private int _before_loop_amount = 2;
    [Space(10)]
    [SerializeField] private float _phone_icon_closing_animation_speed;
    [Space(10)]
    [SerializeField] private float _map_move_speed = 10f;
    [SerializeField] private float _IndicatorMaxDistance = 500;
    [SerializeField] private float _IndicatorMinScale = .3f;
    [SerializeField] private float _IndicatorTreshold = 50f;
    [SerializeField] private float _IndicatorHideThreshold = 150f;

    [Header("Phone Position")]
    [SerializeField] private float _phone_position_animation_speed;

    private Coroutine _current_state_toggle_coroutine;
    private Coroutine _current_icon_scale_coroutine;
    private Coroutine _current_icon_input_scale_coroutine;
    private Coroutine _current_notification;
    private Coroutine _Route_planner_open_close;
    private Coroutine _IndicatorSpinner;
    private Coroutine _IndicatorAppear;

    private InputAction _map_move_input;

    private Vector2 _MapMaskBounds;

    private bool isIndicatorVisible = false;
    private bool _indicatorInitialized = false;

    private void OnEnable()
    {
        _StateMachine.OnPhoneStateChanged += HandlePhoneState;
        _StateMachine.OnRoutePlannerStateChanged += HandleRoutePlannerState;
        _map_snapper.on_snapped += HandleSnapped;
        _map_snapper.on_released += HandleReleased;
    }

    private void OnDisable()
    {
        _StateMachine.OnPhoneStateChanged -= HandlePhoneState;
        _StateMachine.OnRoutePlannerStateChanged -= HandleRoutePlannerState;
        _map_snapper.on_snapped -= HandleSnapped;
        _map_snapper.on_released -= HandleReleased;
    }

    private void Awake()
    {
        var actionMap = _playerInput.actions.FindActionMap("Action map");
        _map_move_input = actionMap.FindAction("Map_Move_Test");
    }

    private void Start()
    {
        _OffscreenIndicator.localScale = Vector3.zero;

        if (!_phone_has_been_openend)
        {
            _current_icon_scale_coroutine = StartCoroutine(Phone_icon_animation());
            _icon_placeholder.localScale = Vector3.zero;
        }

        if (_StateMachine.CurrentPhoneUIState != UIStateMachine.PhoneUIState.Open)
        {
            _phone_transform.SetPositionAndRotation(
                _phone_closed_transform.position,
                _phone_closed_transform.rotation);
        }
    }

    private void Update()
    {
        if (_StateMachine.CurrentPhoneUIState != UIStateMachine.PhoneUIState.Open)
            return;

        Vector2 direction = _map_move_input.ReadValue<Vector2>();

        if (direction != Vector2.zero)
            _map_placeholder.anchoredPosition -= direction * (_map_move_speed * Time.deltaTime);

        if (_indicatorInitialized)
            UpdateOffscreenIndicator();
    }

    private void UpdateOffscreenIndicator()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, _icon_placeholder.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_MapMask, screenPos, null, out Vector2 localpoint);

        float padding = 55f;
        Rect rectMask = _MapMask.rect;
        float halfWidth = rectMask.width / 2f - padding;
        float halfHeight = rectMask.height / 2f - padding;

        bool isOutsideX = localpoint.x < -halfWidth || localpoint.x > halfWidth;
        bool isOutsideY = localpoint.y < -halfHeight || localpoint.y > halfHeight;
        bool isOutside = isOutsideX || isOutsideY;

        float distanceX = Mathf.Max(0, Mathf.Abs(localpoint.x) - halfWidth);
        float distanceY = Mathf.Max(0, Mathf.Abs(localpoint.y) - halfHeight);
        float distanceToEdge = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        Vector2 screenPosCursor = RectTransformUtility.WorldToScreenPoint(null, _Cursor.position);
        Vector2 screenPosIcon = RectTransformUtility.WorldToScreenPoint(null, _icon_placeholder.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_MapMask, screenPosCursor, null, out Vector2 cursorLocalPoint);
        float distanceToIcon = Vector2.Distance(screenPosCursor, screenPosIcon) / _Canvas.scaleFactor;
        Debug.Log($"distance: {distanceToIcon:F0} | hide: {_IndicatorHideThreshold} | state: {_indicatorState}");

        if ((distanceToIcon < _IndicatorHideThreshold || !isOutside) && _indicatorState == IndicatorState.Visible)
        {
            _indicatorState = IndicatorState.Hiding;

            if (_IndicatorSpinner != null)
            {
                StopCoroutine(_IndicatorSpinner);
                _IndicatorSpinner = null;
            }

            if (_IndicatorAppear != null) StopCoroutine(_IndicatorAppear);
            _IndicatorAppear = StartCoroutine(UIAnimations.ScaleUpAndDown(
                _OffscreenIndicator, UIAnimations.ease_anticipate, Vector2.zero, 0.4f,
                () => { _indicatorState = IndicatorState.NearIcon;
                        _IndicatorAppear = null; 
                        _OffscreenIndicator.localScale = Vector3.zero;
                }));
        }
        else if (distanceToIcon >= _IndicatorTreshold && isOutside &&
            (_indicatorState == IndicatorState.Hidden || _indicatorState == IndicatorState.NearIcon))
        {
            _indicatorState = IndicatorState.Visible;

            if (_IndicatorSpinner == null)
                _IndicatorSpinner = StartCoroutine(UIAnimations.SimpleUIAnimationRotateAroundZ(_OffscreenIndicatorSpinner, 150));

            float scaleFactor = Mathf.Lerp(1f, _IndicatorMinScale, Mathf.Clamp01(distanceToEdge / _IndicatorMaxDistance));

            if (_IndicatorAppear != null) StopCoroutine(_IndicatorAppear);
            _IndicatorAppear = StartCoroutine(UIAnimations.ScaleUpAndDown(
                _OffscreenIndicator, UIAnimations.ease_overshoot, Vector2.one * scaleFactor, .3f,
                () => _IndicatorAppear = null));
        }

        if (_indicatorState == IndicatorState.Visible && _IndicatorAppear == null)
        {
            float scaleFactor = Mathf.Lerp(1f, _IndicatorMinScale, Mathf.Clamp01(distanceToEdge / _IndicatorMaxDistance));
            _OffscreenIndicator.localScale = Vector3.one * scaleFactor;
        }

        if (isOutside && _indicatorState != IndicatorState.NearIcon && _indicatorState != IndicatorState.Hidden)
        {
            Vector2 dir = (localpoint - cursorLocalPoint).normalized;

            Vector2 indicatorPos = GetEdgeIntersection(cursorLocalPoint, dir, halfWidth, halfHeight);

            _OffscreenIndicator.anchoredPosition = indicatorPos;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _OffscreenIndicatorBackground.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnFirstTimeAnimationComplete()
    {
        _indicatorInitialized = true;
        UpdateOffscreenIndicator();
    }

    private void HandleReleased()
    {
        _StateMachine.SetRoutePlanner(UIStateMachine.RoutePlannerSate.Close);
    }

    private void HandlePhoneState(UIStateMachine.PhoneUIState state)
    {
        if (state == UIStateMachine.PhoneUIState.Open && !_phone_has_been_openend)
        {
            _current_notification = StartCoroutine(StartingNotification());
        }

        Phone_Icons_Animation(state);
    }

    private void HandleRoutePlannerState(UIStateMachine.RoutePlannerSate state)
    {
        if (_Route_planner_open_close != null)
            StopCoroutine(_Route_planner_open_close);

        bool open = state == UIStateMachine.RoutePlannerSate.Open;
        _Route_planner_open_close = StartCoroutine(RoutePlannerPanelAnim(open));
    }

    private void HandleSnapped(RectTransform icon)
    {
        if (icon == null)
            return;

        StartCoroutine(TemporaryDisableMapInput(.6f));

        _StateMachine.SetRoutePlanner(UIStateMachine.RoutePlannerSate.Open);
    }

    private IEnumerator TemporaryDisableMapInput(float time)
    {
        _map_move_input.Disable();

        yield return new WaitForSeconds(time);

        _map_move_input.Enable();
    }

    private IEnumerator StartingNotification()
    {
        Vector2 orgAnchorPos = _staringNotification.anchoredPosition;

        yield return new WaitForSeconds(1f);

        float timer = 0f;
        Vector2 anchorPos = _staringNotification.anchoredPosition -= new Vector2(0, 200);

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            UIAnimations.SimpleUIAnimationPosition(_staringNotification, UIAnimations.ease_overshoot, orgAnchorPos, anchorPos, timer);
            yield return null;
        }

        yield return new WaitForSeconds(2);

        anchorPos = _staringNotification.anchoredPosition;
        float timer2 = 0f;

        while (timer2 < 1f)
        {
            timer2 += Time.deltaTime;
            UIAnimations.SimpleUIAnimationPosition(_staringNotification, UIAnimations.ease_overshoot, anchorPos, orgAnchorPos, timer2);
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        float timer3 = 0f;
        Vector2 startScale = Vector2.zero;

        while (timer3 < 1f)
        {
            timer3 += Time.deltaTime * 1.5f;
            UIAnimations.SimpleUIAnimationScale(_icon_placeholder, UIAnimations.ease_overshoot, startScale, Vector2.one, timer3);
            yield return null;
        }

        _phone_has_been_openend = true;
        OnFirstTimeAnimationComplete();
    }

    private void Phone_Icons_Animation(UIStateMachine.PhoneUIState state)
    {
        if (_current_state_toggle_coroutine != null)
            StopCoroutine(_current_state_toggle_coroutine);
        _current_state_toggle_coroutine = StartCoroutine(PhoneStateAnimation(state));

        if (_current_icon_scale_coroutine != null)
            StopCoroutine(_current_icon_scale_coroutine);
        _current_icon_scale_coroutine = StartCoroutine(Close_Open_Icon_Anim(_phone_icon_holder));

        if (_current_icon_input_scale_coroutine != null)
            StopCoroutine(_current_icon_input_scale_coroutine);
        _current_icon_input_scale_coroutine = StartCoroutine(Close_Open_Icon_Anim(_phone_input_holder, .3f));
    }

    private IEnumerator PhoneStateAnimation(UIStateMachine.PhoneUIState state)
    {
        float timer = 0;
        RectTransform transformTo = (state == UIStateMachine.PhoneUIState.Open)
            ? _phone_open_transform
            : _phone_closed_transform;

        Vector3 orgPos = _phone_transform.anchoredPosition;
        Quaternion orgRot = _phone_transform.rotation;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            UIAnimations.SimpleUIAnimationPosition(_phone_transform, UIAnimations.ease_in_out, orgPos, transformTo.anchoredPosition, timer);
            UIAnimations.SimpleUIAnimationRotation(_phone_transform, UIAnimations.ease_in_out, orgRot, transformTo.rotation, timer);
            yield return null;
        }
    }

    private IEnumerator Phone_icon_animation()
    {
        float timer = 0;
        int nul = 0;

        while (!_phone_has_been_openend)
        {
            timer += Time.deltaTime * _phone_icon_animation_speed;

            float rotation_mult = UIAnimations.ease_shake.Evaluate(timer);
            float scale_mult = UIAnimations.ease_overshoot.Evaluate(timer / (_before_loop_amount / 2));

            _phone_icon.rectTransform.rotation = Quaternion.Euler(0, 0, _phone_icon_rotation_amount * rotation_mult);
            _phone_icon.rectTransform.localScale = Vector3.one * (1 + _phone_icon_scale_amount * scale_mult);

            if (timer > 1)
            {
                if (nul++ == _before_loop_amount)
                {
                    yield return new WaitForSeconds(_phone_icon_interval);
                    nul = 0;
                }
                timer = 0;
            }

            yield return null;
        }
    }

    private IEnumerator Close_Open_Icon_Anim(RectTransform target, float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        float timer = 0;
        bool isOpen = _StateMachine.CurrentPhoneUIState == UIStateMachine.PhoneUIState.Open;

        Vector3 endScale = isOpen ? Vector3.zero : Vector3.one;
        AnimationCurve curve = isOpen ? UIAnimations.ease_anticipate : UIAnimations.ease_overshoot;
        Vector3 orgScale = target.localScale;

        while (timer < 1)
        {
            timer += Time.deltaTime * _phone_icon_closing_animation_speed;
            UIAnimations.SimpleUIAnimationScale(target, curve, orgScale, endScale, timer);
            yield return null;
        }

        target.localScale = endScale;
    }

    private IEnumerator RoutePlannerPanelAnim(bool open)
    {
        float moveAmountPannel = 150;
        Vector2 startPosRoutePlannerPanel = _RoutePlannerPanel.anchoredPosition;
        Vector2 startPosMapParent = _MapParent.anchoredPosition;
        Vector3 startScaleMapParent = _MapParent.localScale;

        Vector2 transformToPannel = open ? new Vector2(moveAmountPannel, 0) : new Vector2(-moveAmountPannel, 0);
        Vector2 targetPosMapParent = startPosMapParent;
        Vector3 targetScaleMapParent = startScaleMapParent;

        if (open)
        {
            CalculateMapFrame(out targetPosMapParent, out targetScaleMapParent);
        }
        else
        {
            targetPosMapParent = Vector2.zero;
            targetScaleMapParent = Vector3.one;
        }

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            UIAnimations.SimpleUIAnimationPosition(_RoutePlannerPanel, UIAnimations.ease_in_out, startPosRoutePlannerPanel, transformToPannel, timer);
            UIAnimations.SimpleUIAnimationPosition(_MapParent, UIAnimations.ease_in_out, startPosMapParent, targetPosMapParent, timer * 1.5f);
            UIAnimations.SimpleUIAnimationScale(_MapParent, UIAnimations.ease_in_out, startScaleMapParent, targetScaleMapParent, timer * 1.8f);
            yield return null;
        }
    }

    private void CalculateMapFrame(out Vector2 pos, out Vector3 scale)
    {
        float panelWidth = _RoutePlannerPanel.rect.width;
        float usableWidth = _MapParent.rect.width - panelWidth;
        float mapHeight = _MapParent.rect.height;


        Vector2 iconPos = WorldToMap(_icon_placeholder);
        Vector2 playerPos = WorldToMap(_PlayerIcon);

        Vector2 center = (iconPos + playerPos) / 2;

        float boundingBoxX = Mathf.Abs(iconPos.x - playerPos.x);
        float boundingBoxY = Mathf.Abs(iconPos.y - playerPos.y);

        float padding = 450f;
        boundingBoxX += padding;
        boundingBoxY += padding;

        float scaleX = usableWidth / boundingBoxX;
        float scaleY = usableWidth / boundingBoxY;
        float newScale = Mathf.Min(scaleX, scaleY, 1f);

        float usableCenterX = (panelWidth / 2f);
        Vector2 mapcenter = new Vector2(usableCenterX, 0);

        pos = mapcenter - center * newScale;
        scale = Vector3.one * newScale;
    }

    private Vector2 WorldToMap(RectTransform target)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_MapParent, screenPos, null, out Vector2 localPoint);
        return localPoint;
    }

    private Vector2 GetEdgeIntersection(Vector2 origin, Vector2 dir, float halfWidth, float halfHeight)
    {
        float tX = dir.x != 0 ? (Mathf.Sign(dir.x) * halfWidth - origin.x) / dir.x : float.MaxValue;
        float tY = dir.y != 0 ? (Mathf.Sign(dir.y) * halfHeight - origin.y) / dir.y : float.MaxValue;
        float t = Mathf.Min(tX, tY);

        Vector2 hit = origin + dir * t;

        hit.x = Mathf.Clamp(hit.x, -halfWidth, halfWidth);
        hit.y = Mathf.Clamp(hit.y, -halfHeight, halfHeight);
        return hit;
    }
}