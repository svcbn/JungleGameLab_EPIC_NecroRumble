using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity;
using Honeti;
using LOONACIA.Unity.Managers;
using System.Collections.Generic;
using System;

public class UIUnitContainerPopup : UIBase
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

	// enum Sliders
	// {
	//     ExpSlider,
	// }

	TMP_Text _nameText, _lvText;
	TMP_Text _unitInfoNameText;

	Image _border;

	GridLayoutGroup _unitSkillGridPanel;
	GridLayoutGroup _unitInfoGridPanel;
	
	UnitType _unitType = UnitType.SwordMan;

    Action<string> _actionCanceled;

	float _fivotX = -200;
	protected override void Init()
	{
		Debug.Log("UIUnitContainerPopup | Init");
		Bind<Image, Images>();
		Bind<TMP_Text, TmpTexts>();
		
		RegisterPointerEvent(OnSelect, UIEventType.PointerEnter);
		RegisterPointerEvent(OnDeselect, UIEventType.PointerExit);
		// RegisterPointerEvent(OnSubmit, UIEventType.Click);

		RegisterBaseEvent(OnSelect, UIEventType.Select);
		RegisterBaseEvent(OnDeselect, UIEventType.Deselect);
		// RegisterBaseEvent(OnSubmit, UIEventType.Submit);

		RegisterBaseEvent(OnCancel, UIEventType.Cancel);

		_border = Get<Image>((int)Images.Border);
		//border.gameObject.SetActive(false);
		
	}

    public void SetCanceledAction(Action<string> actionCanceled_)
    {
        _actionCanceled = actionCanceled_;
    }

	public void SetIcon(Sprite sprite_)
	{
		if( sprite_ == null ){ return; }
		
		Get<Image>((int)Images.IconImage).sprite = sprite_;
	}
	
	public void SetNameText(UnitType unitType_, string text_)
	{
		_nameText = Get<TMP_Text>((int)TmpTexts.NameText);
		_unitType = unitType_;
		_nameText.text = text_;
	}
	
	public void SetLvText(string text_)
	{
		
		_lvText = Get<TMP_Text>((int)TmpTexts.LvText);
		_lvText.text = text_;
	}

	public void SetLocalizeText()
	{
		_nameText.gameObject.GetOrAddComponent<I18NText>(); 
	}

	public void SetAlpha(float alpha_)
	{
		Image border         = Get<Image>((int)Images.Border);
		Image icon           = Get<Image>((int)Images.IconImage);
		Image iconBackground = Get<Image>((int)Images.IconImageBackground);
		TMP_Text nameText    = Get<TMP_Text>((int)TmpTexts.NameText);
		TMP_Text lvText      = Get<TMP_Text>((int)TmpTexts.LvText);

		border.color = new Color(border.color.r, border.color.g, border.color.b, alpha_);
		icon.color   = new Color(icon.color.r, icon.color.g, icon.color.b, alpha_);
		iconBackground.color = new Color(iconBackground.color.r, iconBackground.color.g, iconBackground.color.b, alpha_);
		
		nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, alpha_);
		lvText.color  = new Color(lvText.color.r,  lvText.color.g,  lvText.color.b,  alpha_);
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

		DG.Tweening.Sequence blinkSequence = DOTween.Sequence();

		blinkSequence.Append(transform.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.OutQuad))
			.Append(transform.DOScale(originalScale * 1.2f, 0.3f).SetEase(Ease.OutQuad))
			.Append(transform.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.OutQuad))
			.Append(transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuad));
	}
	
	public void OnSelect(BaseEventData eventData)
	{
		Debug.Log($"UIUnitContainerPopup | On Select | {_unitType}");

		// _border.gameObject.SetActive(true);
		_border.color = Color.white;
		string unitName = _unitType switch
									{
										UnitType.SwordMan => "^text_panel_exp_Swordman",
										UnitType.ArcherMan => "^text_panel_exp_Archer",
										UnitType.Assassin => "^text_panel_exp_Assassin",
									} ;
		_unitInfoNameText.text = ManagerRoot.I18N.getValue(unitName);
		SetUnitSkillPanel(_unitType);
		SetUnitInfoPanel(_unitType);

		// _uIShortExp = ManagerRoot.UI.MakeSubItem<UIShortExplain>( this.transform);

		// float fivotX = 250;
		// // if(_idx % 2 == 0){ fivotX = -250; }
		// // else{              fivotX =  250; }   

		// _uIShortExp.transform.localPosition = new Vector3(fivotX, 0, 0);
		// _uIShortExp.SetContent(_skillId);

		// _uIShortExp.transform.SetParent(this.transform.parent.parent);

		// // _explainPanel = ManagerRoot.UI.ShowPopupUI<UIUnitCard>();
		// // _explainPanel.transform.SetParent(transform);
		// // _explainPanel.transform.localPosition = new Vector3(700, 0, 0);
		// // _explainPanel.transform.localScale = new Vector3(1f, 1f, 1f);
		// // _explainPanel.SetText($"dummy word ", $"dummy Description");
		// // _explainPanel.gameObject.SetActive(true);

		ManagerRoot.Sound.PlaySfx("Clicks Sound (button hover) 1");
		ManagerRoot.I18N.setLanguage(ManagerRoot.Settings.CurrentLanguage.ToString());
	}

	public void OnDeselect(BaseEventData eventData)
	{
		Debug.Log($"UIUnitContainerPopup | On DeSelect | {_unitType}");


		// _border.gameObject.SetActive(false);
		_border.color = Color.black;

		// if( _uIShortExp != null){
		//     Destroy(_uIShortExp.gameObject);
		//     _uIShortExp = null;
		// }
	}

	public void OnCancel(BaseEventData eventData)
	{
		Debug.Log($"UIUnitContainerPopup | On Cancel | {_unitType}");

		_actionCanceled?.Invoke("");
	}

	// public void OnSubmit(BaseEventData eventData)
	// {
	//     Debug.Log("On Submit");

	// 	ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
	// }
	
	public void SetUnitGrid(GridLayoutGroup unitSkillGridPanel_, GridLayoutGroup unitInfoGridPanel_, TMP_Text unitInfoNameText_){
		Debug.Log($"UIUnitContainerPopup | SetSkillGrid | {unitSkillGridPanel_}" );
		_unitSkillGridPanel = unitSkillGridPanel_;
		_unitInfoGridPanel = unitInfoGridPanel_;
		_unitInfoNameText = unitInfoNameText_;
	}
	
	void SetUnitSkillPanel(UnitType unitType_)
	{
		foreach (Transform child in _unitSkillGridPanel.transform) {
			Destroy(child.gameObject);
		}

		List<SkillDataAttribute> unitSkill = ManagerRoot.Skill.GetAllUpgradesByUnitType(unitType_);
		if(unitSkill == null)  return;

		Debug.Log($"UIUnitContainerPopup | SetUnitSkillPanel | unitSkill.Count {unitSkill.Count}");

		for (int i = 0; i < unitSkill.Count; i++)
		{
			UIIconSkill uIIconSkill = ManagerRoot.UI.MakeSubItem<UIIconSkill>(_unitSkillGridPanel.transform);
			uIIconSkill.SetFivotX(_fivotX);
			uIIconSkill.SetCanceledAction(_actionCanceled);
			
			// SkillData skillData = allSkillData.Find(skill => skill.Id == unitSkill[i].Id);
			SkillDataAttribute skillDataAttribute = unitSkill[i];
			if (skillDataAttribute != null)
			{
				Sprite _iconSprite = skillDataAttribute.IconSprite;
				uIIconSkill.SetIconInfo(_iconSprite);
				uIIconSkill.SetSkillId(skillDataAttribute);
				uIIconSkill.SetIndex((uint)i);
			}
		}

		SetUnitSkillGrid();
	}
	void SetUnitSkillGrid()
	{
		Rect parentRect = _unitSkillGridPanel.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width * 5/17, parentRect.height * 5/22);
		Vector4 padding  = new( parentRect.width * 1/34, parentRect.width * 1/34, parentRect.height * 1/28, parentRect.height * 1/28 );
		Vector2 spacing  = new( parentRect.width * 1/34, parentRect.height * 1/88 );

		SetGridSize( _unitSkillGridPanel, cellSize, padding, spacing);

		for (int i = 0; i < _unitSkillGridPanel.transform.childCount; i++)
		{
			_unitSkillGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}
	 
	enum EnumUnitInfoStat{
		HP = 0,
		ATK_DMG,
		ATK_VEL,
		HEALTH_DRAIN,
		MOVE_SPEED,
	}
	void SetUnitInfoPanel(UnitType unitType_)
	{
		Transform child;
		float modifiedBase_, percentageAddition_;
		TMP_Text valueText, infoText;

		child = _unitInfoGridPanel.transform.GetChild((int)EnumUnitInfoStat.HP);
		child.gameObject.SetActive(true);
		infoText = child.transform.Find("InfoText").GetComponent<TMP_Text>();
		infoText.text = "^text_panel_info_Hp";
		valueText = child.transform.Find("ValueText").GetComponent<TMP_Text>();
		valueText.text = $"{Math.Round(ManagerRoot.UnitUpgrade.GetDisplayStat(unitType_, StatType.MaxHp, out modifiedBase_, out percentageAddition_))}";
		
		child = _unitInfoGridPanel.transform.GetChild((int)EnumUnitInfoStat.ATK_DMG);
		child.gameObject.SetActive(true);
		infoText = child.transform.Find("InfoText").GetComponent<TMP_Text>();
		infoText.text = "^text_panel_info_Attack";
		valueText = child.transform.Find("ValueText").GetComponent<TMP_Text>();
		valueText.text = $"{Math.Round(ManagerRoot.UnitUpgrade.GetDisplayStat(unitType_, StatType.AttackDamage, out modifiedBase_, out percentageAddition_))}";
		
		child = _unitInfoGridPanel.transform.GetChild((int)EnumUnitInfoStat.ATK_VEL);
		child.gameObject.SetActive(true);
		infoText = child.transform.Find("InfoText").GetComponent<TMP_Text>();
		infoText.text = "^text_panel_info_AttackSpeed";
		valueText = child.transform.Find("ValueText").GetComponent<TMP_Text>();
		valueText.text = ManagerRoot.UnitUpgrade.GetDisplayStat(unitType_, StatType.AttackPerSec, out modifiedBase_, out percentageAddition_).ToString("0.00");
		
		child = _unitInfoGridPanel.transform.GetChild((int)EnumUnitInfoStat.HEALTH_DRAIN);
		child.gameObject.SetActive(true);
		infoText = child.transform.Find("InfoText").GetComponent<TMP_Text>();
		infoText.text = "^text_panel_info_Lifesteal";
		valueText = child.transform.Find("ValueText").GetComponent<TMP_Text>();
		valueText.text = $"{Math.Round(ManagerRoot.UnitUpgrade.GetDisplayStat(unitType_, StatType.HealthDrain, out modifiedBase_, out percentageAddition_))}";
		
		child = _unitInfoGridPanel.transform.GetChild((int)EnumUnitInfoStat.MOVE_SPEED);
		child.gameObject.SetActive(true);
		infoText = child.transform.Find("InfoText").GetComponent<TMP_Text>();
		infoText.text = "^text_panel_info_MoveSpeed";
		valueText = child.transform.Find("ValueText").GetComponent<TMP_Text>();
		valueText.text = ManagerRoot.UnitUpgrade.GetDisplayStat(unitType_, StatType.MoveSpeed, out modifiedBase_, out percentageAddition_).ToString("0.00");



		SetUnitInfoGrid();
	}
	void SetUnitInfoGrid()
	{
		Rect parentRect = _unitInfoGridPanel.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width, parentRect.height * 1/5);
		Vector4 padding  = new( 0,0,0,0 );
		Vector2 spacing  = new( 0,0 );

		SetGridSize( _unitInfoGridPanel, cellSize, padding, spacing);

		for (int i = 0; i < _unitInfoGridPanel.transform.childCount; i++)
		{
			_unitInfoGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}
	
	public void SetGridSize( GridLayoutGroup grid_, Vector2 cellSize_, Vector4 padding_, Vector2 spacing_ )
	{
		grid_.cellSize = cellSize_;

		grid_.padding.left   = (int)padding_.x;
		grid_.padding.right  = (int)padding_.y;
		grid_.padding.top    = (int)padding_.z;
		grid_.padding.bottom = (int)padding_.w;

		grid_.spacing = spacing_;
	}

}