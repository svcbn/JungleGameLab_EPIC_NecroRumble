using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity;
using Honeti;

public class UIUnitContainer : UIBase
{
    enum Images
    {
        Border,
        IconImageBackground,
        IconImage,
    }

    enum TmpTexts
    {
        NameText,
        LvText,
        //ExpText,
    }

    enum Sliders
    {
        ExpSlider,
    }

    TMP_Text _nameText, _lvText;


    protected override void Init()
    {
        Bind<Image, Images>();
        Bind<TMP_Text, TmpTexts>();
        Bind<Slider, Sliders>();

        // RegisterPointerEvent( OnPointerEntered, UIEventType.PointerEnter);
        // RegisterPointerEvent( OnPointerExited,  UIEventType.PointerExit);


        //Image border = Get<Image>((int)Images.Border);
        //border.gameObject.SetActive(false);
        
        _nameText = Get<TMP_Text>((int)TmpTexts.NameText);
        _nameText.text = "Unit Name";
        
        _lvText = Get<TMP_Text>((int)TmpTexts.LvText);
        _lvText.text = "Lv 1";

        // TMP_Text expText = Get<TMP_Text>((int)TmpTexts.ExpText);
        // expText.text = "0/3";

        Slider expSlider = Get<Slider>((int)Sliders.ExpSlider);
        expSlider.value = 0f;
    }


    public void SetIcon(Sprite sprite_)
    {
        if( sprite_ == null ){ return; }
        
        Get<Image>((int)Images.IconImage).sprite = sprite_;
    }
    
    public void SetNameText(string text_)
    {
        _nameText.text = text_;
    }
    
    public void SetLvText(string text_)
    {
        _lvText.text = text_;
    }

    public void SetLocalizeText()
    {
		_nameText.gameObject.GetOrAddComponent<I18NText>(); 
    }
    // public void SetExpText(string text_)
    // {
    //     Get<TMP_Text>((int)TmpTexts.ExpText).text = text_;
    // }

    public void SetExpSlider(float value_)
    {
        Get<Slider>((int)Sliders.ExpSlider).value = value_;
    }

    public void SetAlpha(float alpha_)
    {
        Image border         = Get<Image>((int)Images.Border);
        Image icon           = Get<Image>((int)Images.IconImage);
        Image iconBackground = Get<Image>((int)Images.IconImageBackground);
        TMP_Text nameText    = Get<TMP_Text>((int)TmpTexts.NameText);
        TMP_Text lvText      = Get<TMP_Text>((int)TmpTexts.LvText);
        //TMP_Text expText     = Get<TMP_Text>((int)TmpTexts.ExpText);


        border.color = new Color(border.color.r, border.color.g, border.color.b, alpha_);
        icon.color   = new Color(icon.color.r, icon.color.g, icon.color.b, alpha_);
        iconBackground.color = new Color(iconBackground.color.r, iconBackground.color.g, iconBackground.color.b, alpha_);
        
        nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, alpha_);
        lvText.color  = new Color(lvText.color.r,  lvText.color.g,  lvText.color.b,  alpha_);
        //expText.color = new Color(expText.color.r, expText.color.g, expText.color.b, alpha_);
    }


    public void OnPointerEntered(PointerEventData data)
    {
	    Image border = Get<Image>((int)Images.Border);
        border.gameObject.SetActive(true);
    }
    
    public void OnPointerExited(PointerEventData data)
    {
        Image border = Get<Image>((int)Images.Border);
        border.gameObject.SetActive(false);
    }
    
    public void Blink()
    {
        Vector3 originalScale = GetComponent<RectTransform>().localScale;
        transform.DOScale(originalScale * 1.5f, 0.3f) // 확대
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, 0.5f) // 축소
                    .SetEase(Ease.OutQuad);
            });
    }

    public void BlinkTwice()
    {
        Vector3 originalScale = GetComponent<RectTransform>().localScale;

        Sequence blinkSequence = DOTween.Sequence();

        blinkSequence.Append(transform.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale * 1.2f, 0.3f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuad));
    }
}