using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity.Managers;
using System;

public class UICreditButton : UIBase
{
    private GameObject _creditCanvas;
    enum Images
    {
        Light,
    }


    Action<string> _actionClicked;

    private Image _light;
    private TMP_Text _btnText;

    Vector3 _scale;

    
    protected override void Init()
    {
        Bind<Image, Images>();
    
    
        RegisterEvent();
    

        _light = Get<Image>((int)Images.Light);
        _light.gameObject.SetActive(false);

        _scale = transform.localScale;
        _creditCanvas = ManagerRoot.Resource.Load<GameObject>("Prefabs/UI/CreditCanvas");
    }

    public void RegisterEvent()
    {
        RegisterPointerEvent( OnSelect,   UIEventType.PointerEnter);
        RegisterPointerEvent( OnDeselect, UIEventType.PointerExit);
        RegisterPointerEvent( OnSubmit,   UIEventType.Click);

        RegisterBaseEvent( OnSelect,   UIEventType.Select);
        RegisterBaseEvent( OnDeselect, UIEventType.Deselect);
        RegisterBaseEvent( OnSubmit,   UIEventType.Submit);
    }


    public void SetClickedAction(Action<string> actionClicked_)
    {
        _actionClicked = actionClicked_;
    }


    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("UICreditButton | OnSelect");

        if(eventData.selectedObject == null){
            eventData.selectedObject = gameObject; // 화면 처음 들어갔을때 필요
        }
        
        _light.gameObject.SetActive(true);
        transform.localScale = _scale * 1.1f;

        ManagerRoot.Sound.PlaySfx("Clicks Sound (button hover) 1");
    }

    public void OnDeselect(BaseEventData eventData)
    {

        _light.gameObject.SetActive(false);
        transform.localScale = _scale;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnDeselect(eventData);

        _actionClicked?.Invoke(_btnText.text);

        // Todo : 오리 꿱 소리로 바꾸기
        ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
        GameObject creditCanvas = Instantiate(_creditCanvas);
        

        (SceneManagerEx.CurrentScene as TitleScene).DisableUITitleScene();
    }
}
