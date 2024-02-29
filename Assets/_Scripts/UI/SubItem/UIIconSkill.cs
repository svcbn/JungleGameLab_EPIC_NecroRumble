using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity.Managers;
using System;
using LOONACIA.Unity;

public class UIIconSkill : UIBase
{
    enum Images
    {
        Border,
        IconImage,
        KeyImage,
    }
    
    enum Texts
	{
		KeyText,
	}

	Image _border;
	Image _keyImage;
	TMP_Text _keyText;
	
    Action<string> _actionCanceled;

    uint _idx;
    SkillDataAttribute _skillDataAttribute;
    UIShortExplain _uIShortExp;
    float _fivotX = 200;
    protected override void Init()
    {
        Bind<Image, Images>();


		RegisterPointerEvent(OnSelect, UIEventType.PointerEnter);
		RegisterPointerEvent(OnDeselect, UIEventType.PointerExit);
		RegisterPointerEvent(OnSubmit, UIEventType.Click);

		RegisterBaseEvent(OnSelect, UIEventType.Select);
		RegisterBaseEvent(OnDeselect, UIEventType.Deselect);
		RegisterBaseEvent(OnSubmit, UIEventType.Submit);

        RegisterBaseEvent(OnCancel, UIEventType.Cancel);

        _border = Get<Image>((int)Images.Border);
        _border.gameObject.SetActive(false);
        
        _keyImage = Get<Image>((int)Images.KeyImage);
        _keyText = Get<TMP_Text>((int)Texts.KeyText);
    }


    public void SetIconInfo(Sprite sprite_)
    {
        if( sprite_ == null ){ return; }
        
        Get<Image>((int)Images.IconImage).sprite = sprite_;
    }
    public void SetSkillId( SkillDataAttribute skillDataAttribute_)
    {
        _skillDataAttribute = skillDataAttribute_;
    }
    public void SetIndex( uint idx_ )
    {
        _idx = idx_;
    }

    public void ResetKeys()
    {
	    _keyImage.gameObject.SetActive(false);
	    _keyText.gameObject.SetActive(false);
    }
    
    public void SetKeyImage( Sprite sprite_ )
	{
		if( sprite_ == null ){ return; }
		ResetKeys();
		_keyImage.gameObject.SetActive(true);	
		_keyImage.sprite = sprite_;
	}
    
    public void SetKeyText( string text_ )
	{
		if( string.IsNullOrEmpty(text_) ){ return; }
		ResetKeys();
		_keyText.gameObject.SetActive(true);
		_keyText.text = text_;
	}
    
    public void SetFivotX(float fivotX_)
	{
        _fivotX = fivotX_;
	}

    public void SetCanceledAction(Action<string> actionCanceled_)
    {
        _actionCanceled = actionCanceled_;
    }

	public void OnSelect(BaseEventData eventData)
	{
        Debug.Log($"UIIconSkill | On Select | {_keyText}");

        _border.gameObject.SetActive(true);

        
        if( _uIShortExp != null){
            ManagerRoot.UI.ClosePopupUI(_uIShortExp);
            _uIShortExp = null;
        }
       
        _uIShortExp = ManagerRoot.UI.ShowPopupUI<UIShortExplain>(paranet:transform);
        _uIShortExp.SetSize(_fivotX);

        _uIShortExp.SetContent(_skillDataAttribute);

		ManagerRoot.Sound.PlaySfx("Clicks Sound (button hover) 1");
	}

	public void OnDeselect(BaseEventData eventData)
	{
        Debug.Log($"UIIconSkill | On DeSelect | {_keyText}");

        _border.gameObject.SetActive(false);

        if( _uIShortExp != null){
            ManagerRoot.UI.ClosePopupUI(_uIShortExp);
            _uIShortExp = null;
        }
	}

	public void OnSubmit(BaseEventData eventData)
	{
        Debug.Log($"UIIconSkill | On Submit | {_keyText}");

		ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
	}

	public void OnCancel(BaseEventData eventData)
	{
        Debug.Log($"UIIconSkill | On Cancel |");

		ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
        _actionCanceled?.Invoke("");

	}

}
