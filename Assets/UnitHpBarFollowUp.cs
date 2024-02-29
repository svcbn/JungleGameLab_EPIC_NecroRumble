using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHpBarFollowUp : MonoBehaviour
{
    private const float INITIAL_DELAY_TIME = 0.08f;
    private const float FOLLOW_UP_SPEED = .7f;   
    
    public RectTransform _originalBar;
    private readonly WaitForSecondsRealtime _initialDelay = new WaitForSecondsRealtime(INITIAL_DELAY_TIME);

    private void Update()
    {
        CheckValueChange();
    }

    private void CheckValueChange()
    {
        //Null Check
        if (_originalBar == null)
        {
            Debug.LogWarning("Original Bar is null");
            return;
        }
        
        float currentValue = transform.localScale.x;
        float targetValue = _originalBar.localScale.x;
        //원본 체력바 수치가 더 높으면 흰바는 그에 맞춰 수치가 높아짐.
        if (currentValue < targetValue)
        {
            Debug.Log("currentValue" + currentValue + " targetValue" + targetValue);
            SetXScale(targetValue);
        }
        else if (currentValue > targetValue)
        {
            var currentX = transform.localScale.x;
            currentX -= FOLLOW_UP_SPEED * Time.unscaledDeltaTime;
            SetXScale(Mathf.Clamp01(currentX));
        }
    }
    
    private void SetXScale(float xScale_)
    {
        var t = transform;
        
        Vector3 scale = t.localScale;
        scale.x = xScale_;
        t.localScale = scale;
    }
    
}
