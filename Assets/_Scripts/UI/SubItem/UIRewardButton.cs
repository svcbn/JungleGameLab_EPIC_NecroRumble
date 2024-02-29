using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity;
using System;
using DG.Tweening;
using Honeti;

public class UIRewardButton : UIBase
{
	private uint _skillId = 0;

	enum Images
	{
		Border,
		RareParticleEffect,
		EpicParticleEffect,
		Content,
		IconImage,
	}

	enum Texts
	{
		NameText,
		DescriptionText,
		Detail1Text,
		Detail2Text,
		RankText,

	}

	Image _border;
	Image _content;
	Image _rareParticleEffect;
	Image _epicParticleEffect;
	UIRewardPopup _uIRewardPopup;
	UIUnitCard _UIUnitCard;
	Action<uint> _actionClicked;
	int _order;
	float _shineRad = 0f;
	private int _curPosOrder;
	public int CurPosOrder { get => _curPosOrder; set => _curPosOrder = value; }

	List<SkillData> _allSkillData;

	protected override void Init()
	{
		Bind<Image, Images>();
		Bind<TMP_Text, Texts>();

		RegisterPointerEvent(OnSelect, UIEventType.PointerEnter);
		RegisterPointerEvent(OnDeselect, UIEventType.PointerExit);
		RegisterPointerEvent(OnSubmit, UIEventType.Click);

		RegisterBaseEvent(OnSelect, UIEventType.Select);
		RegisterBaseEvent(OnDeselect, UIEventType.Deselect);
		RegisterBaseEvent(OnSubmit, UIEventType.Submit);

		_border = Get<Image>((int)Images.Border);
		_border.gameObject.SetActive(false);

		_content = Get<Image>((int)Images.Content);

		_epicParticleEffect = Get<Image>((int)Images.EpicParticleEffect);
		_epicParticleEffect.gameObject.SetActive(false);
		
		_rareParticleEffect = Get<Image>((int)Images.RareParticleEffect);
		_rareParticleEffect.gameObject.SetActive(false);

		_allSkillData = ManagerRoot.Skill.AllSkillData;
	}

	public void SetParticleEffect(bool isOn_, bool isEpic)
	{
		if (isEpic)
		{
			_epicParticleEffect.gameObject.SetActive(isOn_);
			return;
		}
		else
		{
			_rareParticleEffect.gameObject.SetActive(isOn_);
			return;
		}
	}

	public void SetContentColor(Color color_)
	{
		Get<Image>((int)Images.Content).color = color_;
	}

	public void SetRewardGrade()
	{
		if (GetNormalizedRank() >= UIRewardPopup.EPIC_RANK)
		{
			SetContentColor(ColorConverter.ConvertToColor32(724191));
			_border.rectTransform.sizeDelta = new Vector2(940, 220);
			SetParticleEffect(true, isEpic: true);
		}
		else if (GetNormalizedRank() >= UIRewardPopup.RARE_RANK)
		{
			// #6e3a21
			Color32 color = new Color32(80, 40, 20, 255);
			SetContentColor(color);
			Debug.Log($"UIRewardButton | SetRewardGrade | {name} is rare");
			_border.rectTransform.sizeDelta = new Vector2(940, 220);
			SetParticleEffect(true, isEpic: false);
		}
		else
		{
			Color32 color = new Color32(40, 40, 90, 255);
			SetContentColor(color);
		}
	}

	public void Update()
	{
		if (GetNormalizedRank() >= UIRewardPopup.RARE_RANK)
		{
			Shine();
		}
	}

	public void SetParent(UIRewardPopup uIRewardPopup_)
	{
		_uIRewardPopup = uIRewardPopup_;
	}

	public void SetIconInfo(Sprite sprite_)
	{
		if (sprite_ == null) { return; }

		Get<Image>((int)Images.IconImage).sprite = sprite_;
	}
	
	public void SetTextInfo(string name_, string descriptionText_, string detail1Text_, string detail2Text_, int rankText_)
	{
		Get<TMP_Text>((int)Texts.NameText).text = name_;
		Get<TMP_Text>((int)Texts.DescriptionText).text = descriptionText_;
		Get<TMP_Text>((int)Texts.Detail1Text).text = detail1Text_;
		Get<TMP_Text>((int)Texts.Detail2Text).text = detail2Text_;
		Get<TMP_Text>((int)Texts.RankText).text = rankText_.ToString();

		name = name_;
	}

	public string GetDescriptionText()
	{
		return Get<TMP_Text>((int)Texts.DescriptionText).text;
	}
	
	public void SetDescriptionText(string descriptionText_)
	{
		//gameObject.FindChild("DescriptionText").GetComponent<I18NText>().updateTranslation(false);
		Get<TMP_Text>((int)Texts.DescriptionText).text = descriptionText_;
		// gameObject.FindChild("DescriptionText").GetComponent<TextMeshProUGUI>().text = descriptionText_;
		// gameObject.FindChild("DescriptionText").GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
	}
	
	public string SetLocalizeText(string prefix_, string rewardName_)
	{
		var prefix18d = ManagerRoot.I18N.getValue(prefix_);
		var name18d = ManagerRoot.I18N.getValue(rewardName_);
		
		
		Get<TMP_Text>((int)Texts.DescriptionText).gameObject.GetOrAddComponent<I18NText>();
		
		ManagerRoot.I18N.setLanguage(ManagerRoot.Settings.CurrentLanguage.ToString());

		return prefix18d + name18d;
	}

	public void SetSkillId(uint skillId_)
	{
		_skillId = skillId_;
	}

	public void SetIndex(int index_)
	{
		_curPosOrder = index_;
	}
	
	public int GetNormalizedRank() //Rank를 100으로 나눈 나머지
	{
		var rank = int.Parse(Get<TMP_Text>((int)Texts.RankText).text);
		return rank % 100;
	}

	public void SetClickedAction(Action<uint> actionClicked_)
	{
		_actionClicked = actionClicked_;
	}

	public void SetCanvasSortOrder(int order_)
	{
		_order = order_;
	}

	public void OnSelect(BaseEventData eventData)
	{
		// if (ManagerRoot.UI.GetOrder() != _order)
		// {
		// 	//Debug.Log($"GetOrder({ManagerRoot.UI.GetOrder()}) _order({_order})  ");
		// 	return;
		// }
		// else
		// {
		// 	//Debug.Log($"GetOrder({ManagerRoot.UI.GetOrder()}) _order({_order})  ");

		// }

		if (eventData.selectedObject == null)
		{
			eventData.selectedObject = gameObject; // 화면 처음 들어갔을때 필요
		}

		ShowUnitCard();

		_border.gameObject.SetActive(true);

		ManagerRoot.Sound.PlaySfx("Clicks Sound (button hover) 1");
	}

	public void OnDeselect(BaseEventData eventData)
	{
		// if (ManagerRoot.UI.GetOrder() != _order)
		// {
		// 	Debug.Log($"GetOrder({ManagerRoot.UI.GetOrder()}) _order({_order})  ");
		// 	return;
		// }

		HideUnitCard();

		_border.gameObject.SetActive(false);

	}

	public void OnSubmit(BaseEventData eventData)
	{
		// if (ManagerRoot.UI.GetOrder() != _order)
		// {
		// 	return;
		// }

		_actionClicked(_skillId);

		//시간 멈췄으면 돌려놓기
		Time.timeScale = 1;
		GameUIManager.Instance.IsReward = false;

		Debug.Log($"UIRewardButton | OnSubmit | Reward {name} Attained !!");
		ManagerRoot.Sound.PlaySfx("Simple Click Sound 1");
		ManagerRoot.Sound.PlaySfx("Open Loot Box Sound 1");

		if (GetNormalizedRank() >= UIRewardPopup.EPIC_RANK)
		{
			if (SceneManagerEx.CurrentScene.SceneType is SceneType.Tutorial)
			{
				TutorialManager.Instance.EndOpenBoxEvent();
			}
		}
	}

	public SkillData GetCurSkillData()
	{
		SkillData skillData = _allSkillData.Find(skill => skill.Id == _skillId);
		if (skillData == null) { Debug.Log($"skillData is null"); return null; }

		return skillData;
	}
	
	void ShowUnitCard()
	{
		SkillData skillData = _allSkillData.Find(skill => skill.Id == _skillId);
		if (skillData == null) { Debug.Log($"skillData is null"); return; }

		if (skillData.Id >= 100)
		{ // if summon

			// _UIUnitCard = ManagerRoot.UI.ShowPopupUI<UIUnitCard>();
			// _UIUnitCard.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );

			// _UIUnitCard.transform.SetParent(transform);
			// _UIUnitCard.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 400);
			// _UIUnitCard.transform.localPosition = new Vector3(700, 0, 0);

			// _UIUnitCard.SetContent( skillData.IconSprite, skillData.unitType );

			// _UIUnitCard.gameObject.SetActive(true);
		}
	}

	void HideUnitCard()
	{
		if (_UIUnitCard)
		{
			//_UIUnitCard.gameObject.SetActive(false);
			//ManagerRoot.UI.ClosePopupUI(_UIUnitCard);
			ManagerRoot.UI.ClosePopupUI();
		}
	}


	public void PlayAppearAnimation(bool isRare_)
	{
		if (TryGetComponent(out UIRewardAnimationsInit uIRewardAnimationsInit))
		{
			if (isRare_)
			{
				uIRewardAnimationsInit.PlayRareRewardAnimation();
			}
			else
			{
				uIRewardAnimationsInit.StartAppearAnimation();
			}
		}
		else
		{
			//gameObject.GetOrAddComponent<UIRewardAnimationsInit>().StartAppearAnimation();
			Debug.Log("UIRewardAnimationsInit is null");
		}
	}

	public void Shine()
	{
		if (_rareParticleEffect.gameObject.activeSelf)
		{
			if (_rareParticleEffect.material.HasProperty("_ShineRotate"))
			{
				_rareParticleEffect.material.SetFloat("_ShineRotate", _shineRad);
			}
		}

		if (_epicParticleEffect.gameObject.activeSelf)
		{
			if (_epicParticleEffect.material.HasProperty("_ShineRotate"))
			{
				_epicParticleEffect.material.SetFloat("_ShineRotate", _shineRad);
			}
		}

		_shineRad += Time.unscaledDeltaTime;
	}

	public void OnDisable()
	{
		_shineRad = 0f;
		if (_epicParticleEffect.material.HasProperty("_ShineRotate"))
		{
			_epicParticleEffect.material.SetFloat("_ShineRotate", _shineRad);
		}
		
		if (_rareParticleEffect.material.HasProperty("_ShineRotate"))
		{
			_rareParticleEffect.material.SetFloat("_ShineRotate", _shineRad);
		}
	}
}
