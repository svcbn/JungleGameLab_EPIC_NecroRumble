using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity.Managers;
using System;
using Honeti;
using LOONACIA.Unity;

public class UIPopupButton : UIBase
{
    enum Images
    {
        Border,
    }

    enum Texts
    {
        BtnText,
    }

    Action<string> _actionClicked;
    Action<string> _actionCanceled;

    private Image _border;
    private TMP_Text _btnText;
    private int _order;

    protected override void Init()
    {
        Bind<Image, Images>();
        Bind<TMP_Text, Texts>();
    
    
        RegisterEvent();
    

        _border = Get<Image>((int)Images.Border);
        _border.gameObject.SetActive(false);

        _btnText = Get<TMP_Text>((int)Texts.BtnText);
        Get<TMP_Text>((int)Texts.BtnText).text = "^text_title_btn_Start";
    }

    public void RegisterEvent()
    {
        RegisterPointerEvent( OnSelect,   UIEventType.PointerEnter);
        RegisterPointerEvent( OnDeselect, UIEventType.PointerExit);
        RegisterPointerEvent( OnSubmit,   UIEventType.Click);

        RegisterBaseEvent( OnSelect,   UIEventType.Select);
        RegisterBaseEvent( OnDeselect, UIEventType.Deselect);
        RegisterBaseEvent( OnSubmit,   UIEventType.Submit);

        RegisterBaseEvent( OnCancel,   UIEventType.Cancel);
    }

    public void SetButtonText(string text_)
    {
        name = text_;
        Get<TMP_Text>((int)Texts.BtnText).text = text_;
    }

    public void SetClickedAction(Action<string> actionClicked_)
    {
        _actionClicked = actionClicked_;
    }
    public void SetCanceledAction(Action<string> actionCanceled_)
    {
        _actionCanceled = actionCanceled_;
    }

    public void SetCanvasSortOrder(int order_)
    {
        _order = order_;
    }

    public void SetLocalizeText()
    {
		_btnText.gameObject.GetOrAddComponent<I18NText>(); 
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(eventData.selectedObject == null){
            eventData.selectedObject = gameObject; // 화면 처음 들어갔을때 필요
        }
        

        _border.gameObject.SetActive(true);
        _btnText.color = Color.black;

        ManagerRoot.Sound.PlaySfx("Clicks Sound (button hover) 1");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _border.gameObject.SetActive(false);
        _btnText.color = Color.white;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnDeselect(eventData);

        _actionClicked?.Invoke(_btnText.text);

        ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
    }

	public void OnCancel(BaseEventData eventData)
	{
		Debug.Log($"UIPopupButton | OnCancel");

        _actionCanceled?.Invoke(_btnText.text);
	}
}
