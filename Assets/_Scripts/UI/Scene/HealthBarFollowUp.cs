using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFollowUp : MonoBehaviour
{
    private const float INITIAL_DELAY_TIME = 0.08f;
    private const float FOLLOW_UP_SPEED = .35f;   
    
    private HealthBar _followUpBar;
    private HealthBar _originalBar;
    private bool _isFollowingUp;
    private readonly WaitForSeconds _initialDelay = new WaitForSeconds(INITIAL_DELAY_TIME);

    private void Awake()
    {
        _followUpBar = GetComponent<HealthBar>();
        _originalBar = transform.parent.GetComponent<HealthBar>();
    }

    private void Update()
    {
        CheckValueChange();
    }

    private void CheckValueChange()
    {
        //원본 체력바 수치가 더 높으면 흰바는 그에 맞춰 수치가 높아짐.
        if (_followUpBar.HealthNormalized < _originalBar.HealthNormalized)
        {
            _isFollowingUp = false;
            _followUpBar.HealthNormalized = _originalBar.HealthNormalized;
            StopAllCoroutines();
            return;
        }
        
        //원본 체력바 수치가 더 낮고 이미 떨어지기 시작한 뒤가 아니면 코루틴 실행.
        if (_followUpBar.HealthNormalized > _originalBar.HealthNormalized && !_isFollowingUp)
        {
            _isFollowingUp = true;
            StartCoroutine(FollowUpRoutine());
        }
    }

    private IEnumerator FollowUpRoutine()
    {
        //흰 바는 잠깐 멈췄다가 따라가기 시작함.
        yield return _initialDelay;
        
        //흰 바가 따라가기 시작.
        while (_followUpBar.HealthNormalized > _originalBar.HealthNormalized)
        {
            _followUpBar.HealthNormalized -= FOLLOW_UP_SPEED * Time.unscaledDeltaTime;
            _followUpBar.HealthNormalized = Mathf.Clamp01(_followUpBar.HealthNormalized);
            yield return null;
        }
        _isFollowingUp = false;
    }
}
