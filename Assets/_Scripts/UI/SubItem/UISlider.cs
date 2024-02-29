using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity.Managers;
using System;
using Honeti;
using LOONACIA.Unity;

public class UISlider : UIBase
{

    Action<string> _actionCanceled;


    protected override void Init()
    {
        RegisterEvent();

    }

    public void RegisterEvent()
    {

        RegisterBaseEvent( OnCancel,   UIEventType.Cancel);
    }

    public void SetCanceledAction(Action<string> actionCanceled_)
    {
        _actionCanceled = actionCanceled_;
    }

    public void OnCancel(BaseEventData eventData)
    {
		Debug.Log($"UISlider | OnCancel");

        _actionCanceled?.Invoke("");

    }
}
