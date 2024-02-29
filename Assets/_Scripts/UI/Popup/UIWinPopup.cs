using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOONACIA.Unity.Managers;
using System;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using Honeti;


public class UIWinPopup : UIPopup
{

	enum Texts
	{
		TitleText,
		UnitInfoNameText,
	}

	public enum Images
	{
		FlightKnightImage,
		LichImage,
		SuccubusImage,
		GolemImage,
		HorseManImage,
		TwoSwordImage,
		SkeletonImage,
		GhostImage,
		ArcherImage,

		Blocker,
		PlayerImage,
		PlayerImageCute,

	}

	static public bool[] IsFamilyImage = new bool[9]{false, false, false, false, false, false, false, false, false};
	enum GridPanels
	{
		MenuGridPanel,
		SkillGridPanel,
		UnitGridPanel,
		UnitSkillGridPanel,
		UnitInfoGridPanel,
	}

	GridLayoutGroup _menuGridPanel;
	GridLayoutGroup _skillGridPanel;
	GridLayoutGroup _unitGridPanel;

	UIPopupButton _btnToTitle;

	List<UIUnitContainerPopup>        _unitContainers = new ();
	Dictionary<UnitType, Sprite> _unitSpriteDic  = new ();

	protected override void Init()
	{
		base.Init();


		Bind<TMP_Text, Texts>();
		Bind<Image, Images>();
		Bind<GridLayoutGroup, GridPanels>();

		if (ManagerRoot.Unit.CurCharacter == UnitManager.CharacterType.Necromancer) // 할아버지
		{
			Image playerImage = Get<Image>((int)Images.PlayerImage);
			playerImage.enabled = true;
		}
		else{
			Image playerImageCute = Get<Image>((int)Images.PlayerImageCute);
			playerImageCute.enabled = true;
		}

		for(int i = 0 ; i < 9; ++i){
			if(IsFamilyImage[i]){
				Image familyImage = Get<Image>(i);
				familyImage.enabled = true;
			}
		}

		SetMenuPanel();
		LoadSprite();
		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}

	private void OnEnable() 
	{
		SetSkillPanel();

		SetUnitGridPanel();
		//SetUnitPanel();

		EventSystem eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem; // todo 고도화
		eventSystem.SetSelectedGameObject(_btnToTitle.gameObject);
	}
	private void OnDisable()
	{
		ClearGridLayoutGroup(_skillGridPanel.transform);
		ClearGridLayoutGroup(_unitGridPanel.transform);
	}

	void SetMenuPanel() // todo; Grid로 변경하고 Destroy 해주기
	{
		_menuGridPanel = Get<GridLayoutGroup>((int)GridPanels.MenuGridPanel);

		// _btnRestart = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		// _btnRestart.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		// _btnRestart.SetButtonText("재시작");
		// _btnRestart.SetClickedAction(OnRestartClick);

		_btnToTitle = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_btnToTitle.SetCanvasSortOrder( ManagerRoot.UI.GetOrder() );
		_btnToTitle.SetButtonText("^text_panel_win_btn_Totitle");
		_btnToTitle.SetLocalizeText();
		_btnToTitle.SetClickedAction(OnToTitleClick);

		SetMenuGrid();
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

	void SetUnitGrid( GridLayoutGroup grid_ )
	{
		Rect parentRect = grid_.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width, parentRect.height * 1/5);
		Vector4 padding  = new( 0, 0, 0, 0 );
		Vector2 spacing  = new( parentRect.width * 1/5, parentRect.width * 1/5 );

		SetGridSize( grid_, cellSize, padding, spacing);

		for (int i = 0; i < grid_.transform.childCount; i++)
		{
			grid_.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}
	
	public void SetUnitGridPanel() // todo: 코드개선 필요, 일단 4종류 유닛고정이라서 그냥 이렇게 함
	{
		_unitGridPanel = Get<GridLayoutGroup>((int)GridPanels.UnitGridPanel);

		_unitContainers.Clear();
		
		//MakeUnitContainer(UnitType.SpearMan);
		MakeUnitContainer(UnitType.SwordMan);
		MakeUnitContainer(UnitType.ArcherMan);
		MakeUnitContainer(UnitType.Assassin);
		//MakeUnitContainer(UnitType.HorseMan);

		SetUnitGrid(_unitGridPanel);
	}

	void MakeUnitContainer(UnitType unitType_)
	{
		UIUnitContainerPopup unitConta = ManagerRoot.UI.MakeSubItem<UIUnitContainerPopup>(_unitGridPanel.transform);
		unitConta.SetUnitGrid(Get<GridLayoutGroup>((int)GridPanels.UnitSkillGridPanel), Get<GridLayoutGroup>((int)GridPanels.UnitInfoGridPanel), Get<TMP_Text>((int)Texts.UnitInfoNameText));
		int level  = UnitManager.UnitExpDic[unitType_].level;
		int curExp = UnitManager.UnitExpDic[unitType_].curExp;
		int maxExp = UnitManager.UnitExpDic[unitType_].maxExp;
		
		bool isMaxLevel = level >= UnitManager.UNIT_MAX_LEVEL;

		switch (unitType_)
		{
			case UnitType.SwordMan:
				unitConta.SetNameText(UnitType.SwordMan, "^text_panel_exp_Swordman");
				break;
			case UnitType.ArcherMan:
				unitConta.SetNameText(UnitType.ArcherMan, "^text_panel_exp_Archer");
				break;
			// case UnitType.HorseMan:
			// 	unitConta.SetNameText($"언데드 기마병");
			// 	break;
			case UnitType.Assassin:
				unitConta.SetNameText(UnitType.Assassin, "^text_panel_exp_Assassin");
				break;
		}
		
		GameUIManager.Instance.AddUnitExpPanelPosList(unitConta.GetComponent<RectTransform>().parent.position); //파티클 들어가는 위치
		var levelText = isMaxLevel ? "Lv Max" : $"Lv {level}";
		unitConta.SetLvText(levelText);
		unitConta.SetIcon(_unitSpriteDic[unitType_]);
		unitConta.SetLocalizeText();

		_unitContainers.Add(unitConta);
	}

	void SetMenuGrid()
	{
		Rect parentRect = _menuGridPanel.GetComponent<RectTransform>().rect;
		// Vector2 cellSize = new( parentRect.width * 4/5, parentRect.height * 1/3);
		// Vector4 padding  = new( parentRect.width * 1/10, parentRect.width * 1/10, parentRect.height * 1/10, 0 );
		// Vector2 spacing  = new( 0, parentRect.height * 1/10 );

		// SetGridSize( _menuGridPanel, cellSize, padding, spacing);

		for (int i = 0; i < _menuGridPanel.transform.childCount; i++)
		{
			_menuGridPanel.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
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
		_skillGridPanel = Get<GridLayoutGroup>((int)GridPanels.SkillGridPanel);

		List<SkillData> skillDataList = ManagerRoot.Skill.GetSkillListByType(UnitType.None);
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


	void OnRestartClick(string btnText)
	{
		Debug.Log("씬 리로드");
		GameManager.Instance.WinGame();

		SceneManagerEx.ReloadScene();
	}

	void OnToTitleClick(string btnText)
	{
		Debug.Log("'타이틀로' 클릭");
		GameManager.Instance.WinGame();

		SceneManagerEx.LoadScene("Title");
	}

	
	void ClearGridLayoutGroup(Transform gridTransform_)
	{
		foreach (Transform child in gridTransform_)
		{
			Destroy(child.gameObject);
		}
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

	//     int level  = UnitManager.UnitExpDic[unitType_].level;
	// 	int curExp = UnitManager.UnitExpDic[unitType_].curExp;
	// 	int maxExp = UnitManager.UnitExpDic[unitType_].maxExp;

	// 	unitConta.SetLvText($"Lv {level}");
	// 	unitConta.SetExpText($"{curExp}/{maxExp}");
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
