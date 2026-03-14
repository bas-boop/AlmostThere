using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    enum PhoneUIState {FirstTime, Open, Close}
    private PhoneUIState _current_state;

    [Header("References")]
    [SerializeField] private Image _phone_icon;
    [SerializeField] private RectTransform _phone_icon_holder;
    [SerializeField] private RectTransform _phone_input_holder;
    [Space(10)]
    [SerializeField] private Image _map_placeholder;
    [Space(10)]
    [SerializeField] private RectTransform _phone_transform;
    [SerializeField] private RectTransform _phone_open_transform;
    [SerializeField] private RectTransform _phone_closed_transform;

    [Tooltip("De telefoon is al een keer gebruikt")]
    [SerializeField] private bool _phone_has_been_openend = false;

    [Header("Animatie settings")]
    [Header("Phone Icon")]
    [SerializeField] private AnimationCurve _tween_shake_curve;
    [SerializeField] private AnimationCurve _tween_punch_curve;
    [SerializeField] private AnimationCurve _tween_overshoot_curve;
    [SerializeField] private AnimationCurve _tween_anticipate_curve;
    [Space(10)]
    [SerializeField] private float _phone_icon_animation_speed;
    [SerializeField] private float _phone_icon_rotation_amount = 10f;
    [SerializeField] private float _phone_icon_scale_amount = 1.2f;
    [Space(10)]
    [SerializeField] private float _phone_icon_interval = 2f;
    [SerializeField] private int _before_loop_amount = 2;
    [Space(10)]
    [SerializeField] private float _phone_icon_closing_animation_speed;

    [Header("Phone Position")]
    [SerializeField] private float _phone_position_animation_speed;

    private Coroutine _current_state_toggle_coroutine;
    private Coroutine _current_icon_scale_coroutine;
    private Coroutine _current_icon_input_scale_coroutine;

    private void Start()
    {
        if (!_phone_has_been_openend)
        {
            _current_state = PhoneUIState.FirstTime;
            _current_icon_scale_coroutine = StartCoroutine(Phone_icon_animation());
        }

        if (_current_state != PhoneUIState.Open)
        {
            _phone_transform.SetPositionAndRotation(_phone_closed_transform.position, _phone_closed_transform.rotation);
        }
    }
    public void StateToggler()
    {
        if (_current_state == PhoneUIState.FirstTime)
        {
            _phone_has_been_openend = true;
            UpdateState(PhoneUIState.Open);
        }
        else if (_current_state == PhoneUIState.Open)
        {
            UpdateState(PhoneUIState.Close);
        }
        else if (_current_state == PhoneUIState.Close)
        {
            UpdateState(PhoneUIState.Open);
        }
    }

    private void UpdateState(PhoneUIState state)
    {
        switch (state) 
        { 
            case PhoneUIState.Open:
                _current_state = PhoneUIState.Open;
                Phone_Icons_Animation();
                break;
            case PhoneUIState.Close:
                _current_state = PhoneUIState.Close;
                Phone_Icons_Animation();
                break;
        }
    }

    private void Phone_Icons_Animation()
    {
        if (_current_state_toggle_coroutine != null)
            StopCoroutine(_current_state_toggle_coroutine);

        _current_state_toggle_coroutine = StartCoroutine(PhoneStateAnimation(_current_state));

        if (_current_icon_scale_coroutine != null)
            StopCoroutine(_current_icon_scale_coroutine);

        _current_icon_scale_coroutine = StartCoroutine(Close_Open_Icon_Anim(_phone_icon_holder));

        if (_current_icon_input_scale_coroutine != null)
            StopCoroutine(_current_icon_input_scale_coroutine);

        _current_icon_input_scale_coroutine = StartCoroutine(Close_Open_Icon_Anim(_phone_input_holder, .3f));
    }

    private IEnumerator PhoneStateAnimation(PhoneUIState state)
    {
        float timer = 0;

        Transform _tranform_to = (state == PhoneUIState.Open) ? _phone_open_transform : _phone_closed_transform;

        while (_phone_transform.position != _phone_open_transform.position)
        {
            timer = Time.deltaTime * _phone_position_animation_speed;

            Vector3 org_pos_phone = _phone_transform.position;
            Quaternion org_rot_phone = _phone_transform.rotation;

            _phone_transform.position = Vector3.Lerp(org_pos_phone, _tranform_to.position, timer);
            _phone_transform.rotation = Quaternion.Slerp(org_rot_phone, _tranform_to.rotation, timer);
            yield return null;
        }

        _phone_transform.SetPositionAndRotation(_tranform_to.position, _tranform_to.rotation);
        
    }

    private IEnumerator Phone_icon_animation()
    {
        float timer = 0;
        int nul = 0;
        
        while (!_phone_has_been_openend)
        {
            timer += Time.deltaTime * _phone_icon_animation_speed;

            float rotation_phone_icon_mult = _tween_shake_curve.Evaluate(timer);
            float scale_phone_icon_mult = _tween_punch_curve.Evaluate(timer / (_before_loop_amount / 2));

            float rotation_phone_icon = 0 + (_phone_icon_rotation_amount * rotation_phone_icon_mult);
            float scale_phone_icon = 1 + (_phone_icon_scale_amount * scale_phone_icon_mult);

            _phone_icon.rectTransform.rotation = Quaternion.Euler(0, 0, rotation_phone_icon);
            _phone_icon.rectTransform.localScale = new Vector3(scale_phone_icon, scale_phone_icon, scale_phone_icon);

            if (timer > 1)
            {
                int _current_shakes = nul++;

                if (_current_shakes == _before_loop_amount)
                {
                    yield return new WaitForSeconds(_phone_icon_interval);
                    nul = 0;
                }

                timer = 0;
            }

            yield return null;
        }
    }

    private IEnumerator Close_Open_Icon_Anim(RectTransform _transform, float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        float timer = 0;

        Vector3 EndScale = (_current_state == PhoneUIState.Open) ? Vector3.zero : Vector3.one;
        AnimationCurve curve = (_current_state == PhoneUIState.Open) ? _tween_anticipate_curve : _tween_overshoot_curve;
        Vector3 orgScale = _transform.localScale;

        while (timer < 1)
        {
            timer += Time.deltaTime * _phone_icon_closing_animation_speed;

            float curve_time = curve.Evaluate(timer);

            _transform.localScale = new Vector3(curve_time, curve_time, curve_time);

            yield return null;
        }

        _transform.localScale = EndScale;
       
    }
}
