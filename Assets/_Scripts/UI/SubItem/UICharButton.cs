using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity.Managers;
using System;

public class UICharButton : UIBase
{
    enum Images
    {
        Border,
        CharImage,
        SelectedImage,
    }

    // enum TmpTexts
    // {
    //     CharText,
    // }

    Action<string> _actionClicked;
    Action<string> _actionCanceled;

    private Image _border;
    private Image _selectedImage;
    //private string _btnText;
    private int _order;


    protected override void Init()
    {
        Bind<Image, Images>();
        // Bind<TMP_Text, TmpTexts>();

        RegisterPointerEvent( OnSelect,   UIEventType.PointerEnter);
        RegisterPointerEvent( OnDeselect, UIEventType.PointerExit);
        RegisterPointerEvent( OnSubmit,   UIEventType.Click);

        RegisterBaseEvent( OnSelect,   UIEventType.Select);
        RegisterBaseEvent( OnDeselect, UIEventType.Deselect);
        RegisterBaseEvent( OnSubmit,   UIEventType.Submit);

        RegisterBaseEvent( OnCancel,   UIEventType.Cancel);


        _border = Get<Image>((int)Images.Border);
        _border.gameObject.SetActive(false);
        
        _selectedImage = Get<Image>((int)Images.SelectedImage);
        _selectedImage.gameObject.SetActive(false);
    }

    // public void SetCharText(string text_)
    // {
    //     Get<TMP_Text>((int)TmpTexts.CharText).text = text_;
    // }

    public void SetCharIcon(Sprite sprite_)
    {
        if( sprite_ == null ){ return; }
        
        Get<Image>((int)Images.CharImage).sprite = sprite_;
    }

    public void SetCanvasSortOrder(int order_)
    {
        _order = order_;
    }

    public void SetClickedAction(Action<string> actionClicked_)
    {
        _actionClicked = actionClicked_;
    }
    public void SetCanceledAction(Action<string> actionCanceled_)
    {
        _actionCanceled = actionCanceled_;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        if( ManagerRoot.UI.GetOrder() != _order){ 
            return; 
        }

        if(eventData.selectedObject == null){
            eventData.selectedObject = gameObject; // 화면 처음 들어갔을때 필요
        }
        
        _border.gameObject.SetActive(true);

        if (ManagerRoot.Input.LastUsedInputType == DeviceChangeListener.ControlDeviceType.Gamepad)
        {
            if (CharSelectPanelController._charGridPanel == null)
            {
                CharSelectPanelController._charGridPanel = transform.parent.gameObject;
                CharSelectPanelController._charDescriptionPanel = transform.parent.parent.Find("CharDescriptionPanel").gameObject;
            }
            if (transform.name == "Char 1") //첫번째
            {
                ManagerRoot.Unit.CurCharacter = UnitManager.CharacterType.Necromancer;
                CharSelectPanelController.SelectFirstChar();
                StartCoroutine(OnSubmitCoroutine());
            }
            else if (transform.name == "Char 2") //두번째
            {
                ManagerRoot.Unit.CurCharacter = UnitManager.CharacterType.CutePlayer;
                CharSelectPanelController.SelectSecondChar();
                StartCoroutine(OnSubmitCoroutine());
            }
        }
        ManagerRoot.Sound.PlaySfx("Clicks Sound (button hover) 1");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if( ManagerRoot.UI.GetOrder() != _order){ 
            return; 
        }
        
        _border.gameObject.SetActive(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if( ManagerRoot.UI.GetOrder() != _order){ 
            return; 
        }
        
        StartCoroutine(OnSubmitCoroutine());
        
        _actionClicked?.Invoke("");
        
        ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");

        if (ManagerRoot.Input.LastUsedInputType == DeviceChangeListener.ControlDeviceType.Gamepad)
        {
            UICharSelecPopup.OnGameStart("");
        }
    }
    public IEnumerator OnSubmitCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        _selectedImage.gameObject.SetActive(true);
    }

    public void OnCancel(BaseEventData eventData)
    {
        
        _actionCanceled?.Invoke("");

        ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
    }

}
