using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using System;
using Coffee.UIExtensions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using TMPro;
using Honeti;
using UnityEngine.Rendering;

public class UIRewardPopup : UIPopup
{
	const int RewardSlotMAX = 3;
	const int DISPERSION = 2;
	public const int COMMON_RANK = 1;
	public const int RARE_RANK = 3;
	public const int EPIC_RANK = 5;

	public static UnitType upgradedUnit;
	
	enum Images
	{
		Blocker,
		ChestImage,
		UnitInfoPanel,
	}
	
	enum GridPanels
	{
		RewardGridPanel,
		SkillGridPanel,
		//UnitInfoPanel,
		UnitInfoGridPanel,
		UnitSkillGridPanel,
	}
	
	enum Texts
	{
		TitleText,
		UnitInfoNameText,
	}

	GridLayoutGroup _rewardGridPanel, _skillGridPanel, _unitInfoGridPanel, _unitSkillGridPanel;
	Image chestImage, _unitInfoPanel;
	TMP_Text _titleText;
	List<UIRewardButton> _UIRewardButtons = new List<UIRewardButton>();
	EventSystem _eventSystem;

	List<UIUnitContainer>        _unitContainers = new ();
	Dictionary<UnitType, Sprite> _unitSpriteDic  = new ();
	
	// reward popup 하나밖에 안열리겠지..? 여러개 열리면 버그날수도 있음
	public static bool isRewardSelected = false;

	protected override void Init()
	{
		base.Init();

		Bind<Image, Images>();        
		Image blocker = Get<Image>((int)Images.Blocker);
		chestImage = Get<Image>((int)Images.ChestImage);
		
		_unitInfoPanel = Get<Image>((int)Images.UnitInfoPanel);
		_unitInfoPanel.gameObject.SetActive(false);

		Bind<GridLayoutGroup, GridPanels>();
		_rewardGridPanel = Get<GridLayoutGroup>((int)GridPanels.RewardGridPanel);
		_skillGridPanel  = Get<GridLayoutGroup>((int)GridPanels.SkillGridPanel);
		_rewardGridPanel.gameObject.SetActive(false);
		_skillGridPanel.gameObject.SetActive(false);


		_unitInfoGridPanel = Get<GridLayoutGroup>((int)GridPanels.UnitInfoGridPanel);
		_unitSkillGridPanel  = Get<GridLayoutGroup>((int)GridPanels.UnitSkillGridPanel);
		_unitSkillGridPanel.gameObject.SetActive(false);
		
		Bind<TMP_Text, Texts>(); 
		_titleText = Get<TMP_Text>((int)Texts.TitleText);
	}   

	private void OnEnable()
	{
		int rank       = GameUIManager.Instance._rewardRank; // todo 구조개선 필요, 로직을 UI밖으로 빼내는게 좋을듯
		int dispersion = GameUIManager.Instance._rewardDispersion;


		// LoadSprite();
		// SetUnitGridPanel();

		StartCoroutine(SetRewardSlots( rank, dispersion ));
	}

	private void OnDisable() 
	{
		ClearGridLayoutGroup(_skillGridPanel.transform);
		ClearGridLayoutGroup(_rewardGridPanel.transform);
		_UIRewardButtons.Clear();
	}

	private IEnumerator SetRewardSlots(int rank_, int dispersion_)
	{
		List<RewardSlot> rewards = GetRewardsByGrade(rank_, dispersion_);
		Debug.Log($"UIRewardPopup | SetRewardSlots | Rank {rank_} Dispersion {dispersion_}");
		
		bool isEpicRank = false;
		foreach(RewardSlot reward in rewards)
		{
			var normalizedRank = reward.rank % 100;
			if(normalizedRank >= EPIC_RANK){
				isEpicRank = true;  
				break;
			}
		}
		
		if (isEpicRank)
		{
			_titleText.text = "^text_panel_reward_Title";
			_titleText.gameObject.GetOrAddComponent<I18NText>();

			ManagerRoot.Sound.PlaySfx("Powerup upgrade 22", 2f);
			
			float jumpHeight = 250f;
			int jumpCount = 1;
			float jumpDuration = 1f;
			
			chestImage.rectTransform.anchoredPosition = Vector2.zero;
			chestImage.rectTransform.DOShakeRotation(1f, new Vector3(0f, 0f, 30f), 50, 90f).SetUpdate(true);
			chestImage.rectTransform.DOLocalJump(new Vector2(chestImage.rectTransform.anchoredPosition.x, chestImage.rectTransform.anchoredPosition.y), jumpHeight, jumpCount, jumpDuration).SetEase(Ease.OutBounce).SetUpdate(true);
			
			yield return new WaitForSecondsRealtime(1.2f);
			
			chestImage.GetComponent<Animator>().Play("ChestOpen");
			
			yield return new WaitForSecondsRealtime(.3f);
			//ManagerRoot.Sound.PlaySfx("Open Loot Box Sound 5", 1f);
			ManagerRoot.Sound.PlaySfx("Impact Horn 2", 1f);
			
			//Create Particle
			GameObject particleSparklePrefab = ManagerRoot.Resource.Instantiate("Effects/UI_VFXRareSparkleEffect", transform);
			particleSparklePrefab.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			particleSparklePrefab.AddComponent<ParticleSimulation>();
			Destroy(particleSparklePrefab, 3f);
		
			GameObject particleConfettiPrefab = ManagerRoot.Resource.Instantiate("Effects/UI_VFXGlowConfetti", transform);
			particleConfettiPrefab.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			particleConfettiPrefab.AddComponent<ParticleSimulation>();
			Destroy(particleConfettiPrefab, 3f);
		
			yield return new WaitForSecondsRealtime(1.5f);

			_rewardGridPanel.gameObject.SetActive(true);
			_rewardGridPanel.GetComponent<Image>().color = new Color32(255,255,187,255);
			SetSkillPanel();
		}
		else
		{
			_rewardGridPanel.gameObject.SetActive(true);
			_rewardGridPanel.GetComponent<Image>().color = new Color32(205,205,205,255);
			chestImage.gameObject.SetActive(false);
		}

		UIRewardButton uIrewardToBinding1 = null;
		UIRewardButton uIrewardToBinding2 = null;
		
		for (int i = 0; i < rewards.Count; i++)
		{
			yield return new WaitForSecondsRealtime(.2f);

			UIRewardButton uIreward = ManagerRoot.UI.MakeSubItem<UIRewardButton>(_rewardGridPanel.transform);
			SetInfoUI(rewards[i], uIreward, i);
			uIreward.PlayAppearAnimation(isEpicRank);
			_UIRewardButtons.Add(uIreward);
			// if ((int) ManagerRoot.I18N.gameLang != (int) ManagerRoot.Settings.CurrentLanguage)
			// {
			ManagerRoot.I18N.setLanguage(ManagerRoot.Settings.CurrentLanguage.ToString());
			// }	

			//추가 바인딩 설정
			if (uIreward.GetCurSkillData().Id == 15) //찢겨진 흑마도서
			{
				uIrewardToBinding1 = uIreward;
			}

			if (uIreward.GetCurSkillData().Id == 307) // 전투 훈련
			{
				uIrewardToBinding2 = uIreward;
			}
		}

		if (uIrewardToBinding1 != null)
		{
			string text = uIrewardToBinding1.GetDescriptionText();
			text = text.Replace("{x}", BlackGrimoire.GetUndeadNumAddition(1f).ToString());
			Debug.Log($"text: {text}");
			uIrewardToBinding1.SetDescriptionText(text);
		}
		
		if (uIrewardToBinding2 != null)
		{
			CombatTraining ctScript = ManagerRoot.Skill.PlayerSkill.Find(x => x.GetType() == typeof(CombatTraining)) as CombatTraining;
			var ctStr = ctScript?.CurrentDamageIncreaseAmount.ToString() ?? "";
			string text = uIrewardToBinding2.GetDescriptionText();
			text = text.Replace("{x}", ctStr);
			uIrewardToBinding2.SetDescriptionText(text);
		}
		
		if(_UIRewardButtons.Count > 0){
			_eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem;
			_eventSystem.SetSelectedGameObject(_UIRewardButtons[0].gameObject);
		}else{
			Debug.Log("RewardSlot is Empty. Close RewardPopup");
			ManagerRoot.UI.ClearAllPopup( );
		}
		
	}

	void SetInfoUI(RewardSlot reward, UIRewardButton uIreward_, int index_)
	{
		uIreward_.SetParent(this);
		uIreward_.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		uIreward_.SetIconInfo(reward.iconSprite);
		uIreward_.SetIndex(index_);
		
		//유닛 업그레이드일 경우 보상 이름 앞에 유닛 이름 붙이기
		var rewardName = "";
		if (upgradedUnit != default) //리워드가 유닛 업그레이드라면
		{
			switch (upgradedUnit)
			{
				case UnitType.SwordMan:
					_titleText.text = "^text_panel_reward_swordman_Title";
					rewardName = "^text_panel_reward_swordman_Prefix";
					MakeUnitContainer(UnitType.SwordMan);
				break;
				case UnitType.ArcherMan:
					_titleText.text = "^text_panel_reward_archer_Title";
					rewardName = "^text_panel_reward_archer_Prefix";
					MakeUnitContainer(UnitType.ArcherMan);
				break;
				case UnitType.Assassin:
					_titleText.text = "^text_panel_reward_assassin_Title";
					rewardName = "^text_panel_reward_assassin_Prefix";
					MakeUnitContainer(UnitType.Assassin);
				break;
				default:
					rewardName = "";
				break;
			}
			_titleText.gameObject.GetOrAddComponent<I18NText>();
				
		}
	
		uIreward_.SetTextInfo(rewardName, reward.description, reward.detail1, reward.detail2, reward.rank);
		var name = uIreward_.SetLocalizeText(rewardName, reward.name);
		uIreward_.SetTextInfo(name, reward.description, reward.detail1, reward.detail2, reward.rank);
		
		uIreward_.SetSkillId(reward.skillId);
		uIreward_.SetClickedAction(_clickedAction);
		uIreward_.SetRewardGrade();
	}

	void MakeUnitContainer(UnitType unitType_)
	{		
		_unitInfoPanel.gameObject.SetActive(true);
		_unitSkillGridPanel.gameObject.SetActive(true);

		string unitName = unitType_ switch
									{
										UnitType.SwordMan => "^text_panel_exp_Swordman",
										UnitType.ArcherMan => "^text_panel_exp_Archer",
										UnitType.Assassin => "^text_panel_exp_Assassin",
									} ;
		Get<TMP_Text>((int)Texts.UnitInfoNameText).text = ManagerRoot.I18N.getValue(unitName);
		SetUnitSkillPanel(unitType_);
		SetUnitInfoPanel(unitType_);
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
			uIIconSkill.SetFivotX(-200);

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
		Vector2 spacing  = new( parentRect.width * 1/34, parentRect.height * 1/36 );

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
	Action<uint> _clickedAction = (uint skillId_) => 
	{
		isRewardSelected = true;

		ManagerRoot.Skill.AddOrUpgradeSkill(skillId_, upgradedUnit);

		//check gamestate is round
		Debug.Log($"Gamestate is {GameManager.Instance.State}");
		if (GameManager.Instance.State != GameManager.GameState.Round)
		{
			Debug.Log("Gamestate is not Round. Change Gamestate to Round");
			GameManager.Instance.ChangeState(GameManager.GameState.Prepare);
		}
		
		//ManagerRoot.UI.ClosePopupUI( );
		ManagerRoot.UI.ClearAllPopup( );
		// 여기 Game Input복원
		ManagerRoot.Input.DisableUIMode();
	};


	public void SetGridSize( GridLayoutGroup grid_, Vector2 cellSize_, Vector4 padding_, Vector2 spacing_ )
	{
		grid_.cellSize = cellSize_;

		grid_.padding.left   = (int)padding_.x;
		grid_.padding.right  = (int)padding_.y;
		grid_.padding.top    = (int)padding_.z;
		grid_.padding.bottom = (int)padding_.w;

		grid_.spacing = spacing_;
	}

	void SetSkillGrid()
	{
		Rect parentRect = _skillGridPanel.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width * 5/17, parentRect.height * 1/6);
		Vector4 padding  = new( parentRect.width * 1/34, parentRect.width * 1/34, parentRect.height * 1/28, parentRect.height * 1/28 );
		Vector2 spacing  = new( parentRect.width * 1/34, parentRect.height * 1/36 );

		SetGridSize( _skillGridPanel, cellSize, padding, spacing);

		for (int i = 0; i < _skillGridPanel.transform.childCount; i++)
		{
			_skillGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}

	void SetSkillPanel()
	{
		List<SkillData> skillDataList = ManagerRoot.Skill.GetSkillListByType(UnitType.None);
		if(skillDataList.Count > 0)
			_skillGridPanel.gameObject.SetActive(true);
		int i = 0;
		foreach(SkillData skillData in skillDataList)
		{
			UIIconSkill uIIconSkill = ManagerRoot.UI.MakeSubItem<UIIconSkill>(_skillGridPanel.transform);
			if(skillData != null)
			{
				SkillDataAttribute _skillDataAttribute = skillData.GetCurSkillData(0);
				uIIconSkill.SetIconInfo( _skillDataAttribute.IconSprite);
				uIIconSkill.SetSkillId(_skillDataAttribute);
				uIIconSkill.SetIndex((uint)i++);
			}
		}

		SetSkillGrid();
	}

	public void ClearGridLayoutGroup(Transform gridLayoutGroupTransform)
	{
		foreach (Transform child in gridLayoutGroupTransform)
		{
			Destroy(child.gameObject);
		}
	}


	public class RewardSlot
	{
		public uint skillId;
		public Sprite iconSprite;
		public string name;
		public string description;
		public string detail1;
		public string detail2;
		public int rank;
		public uint unitId;
		public UnitType unitType;
	}

	public static List<SkillData> GetListsByGrade(int grade_, int dispersion_) // 분산을 고려해서 등급에 맞는 스킬들을 반환
	{
		List<SkillData> allSkillData = ManagerRoot.Skill.AllSkillData;
		List<SkillBase> playerSkills = ManagerRoot.Skill.PlayerSkill;
		List<SkillData> skillsByGrade = new();

		for (int i = 0; i < allSkillData.Count; i++)
		{
			if (Mathf.Abs(allSkillData[i].SkillGrade - grade_) <= dispersion_)
			{
				if (allSkillData[i].SkillGrade <= 0){ continue; } // 뼈창같은 액티브 스킬들 예외처리

				if (playerSkills.Exists(pSkill => pSkill.Id == allSkillData[i].Id))
				{
					//최대 레벨인 스킬은 제외
					if (playerSkills.Find(pSkill => pSkill.Id == allSkillData[i].Id).SkillLevel >= allSkillData[i].GetMaxSkillLevel())
						continue;
				}

				skillsByGrade.Add(allSkillData[i]);
			}
		}
		
		return skillsByGrade;
	}
	
	public List<SkillData> GetListRandomSelect(List<SkillData> skillsByGrade_) //랜덤하게 n개의 스킬을 뽑아서 반환
	{
		List<SkillData> randomSkills = new();
		int maxCount = RewardSlotMAX > skillsByGrade_.Count ? skillsByGrade_.Count : RewardSlotMAX;

		for (int i = 0; i < maxCount; i++)
		{
			int randomIdx = UnityEngine.Random.Range(0, skillsByGrade_.Count);
			randomSkills.Add(skillsByGrade_[randomIdx]);
			skillsByGrade_.RemoveAt(randomIdx);
		}
		return randomSkills;
	}
	
	public List<RewardSlot> TransformToRewardSlot(List<SkillData> skills_)
	{
		List<RewardSlot> rewardSlots = new();
		foreach(SkillData skill in skills_)
		{
			RewardSlot rewardSlot = new();
			
			var skillLevel = ManagerRoot.Skill.PlayerSkill.Find(pSkill => pSkill.Id == skill.Id)?.SkillLevel + 1 ?? 1;
			Debug.Log($"UIRewardPopup | TransformToRewardSlot | SkillLevel: {skillLevel}");
			
			if (skill.GetCurSkillData(skillLevel).IconSprite == null){ 
				rewardSlot.iconSprite = ManagerRoot.Resource.Load<Sprite>("Sprites/Skills/Default"); }
			else {
				rewardSlot.iconSprite = skill.GetCurSkillData(skillLevel).IconSprite;
			}
			rewardSlot.name        = skill.GetCurSkillData(skillLevel).DisplayName;
			rewardSlot.description = GetDescription(skill, skillLevel);
			rewardSlot.rank        = skill.SkillGrade;
			rewardSlot.skillId     = skill.Id;
		
			rewardSlots.Add(rewardSlot);
		}
		return rewardSlots;
	}
	
	public List<RewardSlot> GetRewardsByGrade(int grade_, int dispersion_) // 등급과 분산을 받아서 RewardSlot 리스트로 반환
	{
		List<SkillData>  skillsByGrade = GetListsByGrade(grade_, dispersion_);
		List<SkillData>  randomSkills  = GetListRandomSelect(skillsByGrade);
		List<RewardSlot> rewardSlots   = TransformToRewardSlot(randomSkills);
		return rewardSlots;
	}

	private string GetDescription(SkillData skill_, int skillLevel_)
	{
		var description = skill_.GetCurSkillData(skillLevel_).Description;
		
		//변수 바인딩
		switch (skill_)
		{
			case BlackGrimoireData data:
				var bgStr = BlackGrimoire.GetUndeadNumAddition(data.maxUnitNumMultiflier).ToString();
				description = description.Replace("{x}", bgStr);
				break;
			case CombatTrainingData:
				if (skillLevel_ < 2) break;
				CombatTraining ctScript = ManagerRoot.Skill.PlayerSkill.Find(x => x.GetType() == typeof(CombatTraining)) as CombatTraining;
				var ctStr = ctScript?.CurrentDamageIncreaseAmount.ToString() ?? "";
				description = description.Replace("{x}", ctStr);
				break;
		}
		
		return description;
	}

	// public void SetUnitGridPanel() // todo: 코드개선 필요, 일단 4종류 유닛고정이라서 그냥 이렇게 함
	// {
	// 	_unitGridPanel = Get<GridLayoutGroup>((int)GridPanels.UnitGridPanel);

	// 	MakeUnitContainer(UnitType.SpearMan);
	// 	MakeUnitContainer(UnitType.SwordMan);
	// 	MakeUnitContainer(UnitType.ArcherMan);
	// 	MakeUnitContainer(UnitType.Assassin);
	// }

	// void MakeUnitContainer(UnitType unitType_)
	// {
	// 	UIUnitContainer unitConta = ManagerRoot.UI.MakeSubItem<UIUnitContainer>(_unitGridPanel.transform);

	// 	int level  = UnitManager.UnitExpDic[unitType_].level;
	// 	int curExp = UnitManager.UnitExpDic[unitType_].curExp;
	// 	int maxExp = UnitManager.UnitExpDic[unitType_].maxExp;

	// 	unitConta.SetLvText($"Lv {level}");
	// 	//unitConta.SetExpText($"{curExp}/{maxExp}");
	// 	unitConta.SetIcon(_unitSpriteDic[unitType_]);
		
	// 	_unitContainers.Add(unitConta);
	// }

	public void SetPosition(Vector3 position_)
	{
		transform.position = position_;
	}
	void LoadSprite()
	{
		Texture2D spearTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Humans/Spear");
		Sprite spearSprite = Sprite.Create(spearTex, new Rect(6,0,22,16), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.SpearMan, spearSprite);

		Texture2D skeletonTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/Skeleton");
		Sprite skeletonSprite = Sprite.Create(skeletonTex, new Rect(6,0,22,16), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.SwordMan, skeletonSprite);

		Texture2D archerTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/SkeletonArcher");
		Sprite skeArcherSprite = Sprite.Create(archerTex, new Rect(6,0,22,16), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.ArcherMan, skeArcherSprite);

		Texture2D ghostTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/Ghost");
		Sprite ghostSprite = Sprite.Create(ghostTex, new Rect(5,0,21,24), new Vector2(0.5f,0.5f));
		_unitSpriteDic.Add(UnitType.Assassin, ghostSprite);
	}
}
