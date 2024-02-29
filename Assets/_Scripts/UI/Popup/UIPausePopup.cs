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

public class UIPausePopup : UIPopup
{
	enum Images
	{
		Blocker,
	}

	enum GridPanels
	{
		MenuGridPanel,
		SkillGridPanel,
		UnitGridPanel,
		UnitSkillGridPanel,
		UnitInfoGridPanel,
	}

	enum Texts
	{
		UnitInfoNameText,
	}


	GridLayoutGroup _menuGridPanel;
	GridLayoutGroup _skillGridPanel;
	GridLayoutGroup _unitGridPanel;

	UIPopupButton _btnContinue, _btnRestart, _btnKeymap, _btnSetting, _btnToTitle;
	List<UIPopupButton> _btns = new List<UIPopupButton>();

	List<UIUnitContainerPopup> _unitContainers = new();
	Dictionary<UnitType, Sprite> _unitSpriteDic = new();

	protected override void Init()
	{
		base.Init();

		Bind<Image, Images>();
		Bind<GridLayoutGroup, GridPanels>();
		Bind<TMP_Text, Texts>();

		SetMenuPanel();
		LoadSprite();
		
		ManagerRoot.I18N.setLanguage(Convert.ToString(ManagerRoot.Settings.CurrentLanguage));
	}

	private void OnEnable()
	{
		Debug.Log("UIPausePopup | OnEnable");

		SetUnitGridPanel();
		SetSkillPanel();

		EnableBtns();  // 다른 팝업에서 떠있을때 Navi select 방지

		EventSystem eventSystem = FindObjectOfType(typeof(@EventSystem)) as EventSystem; // todo 고도화
		eventSystem.SetSelectedGameObject(_btnContinue.gameObject);
	}

	private void OnDisable()
	{
		Debug.Log("UIPausePopup | OnDisable");

		ClearGridLayoutGroup(_skillGridPanel.transform);
		ClearGridLayoutGroup(_unitGridPanel.transform);

		DisableBtns();
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

	void SetMenuGrid()
	{
		Rect parentRect = _menuGridPanel.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width * 4/5, parentRect.height * 1/6);
		Vector4 padding  = new( parentRect.width * 1/10, parentRect.width * 1/10, parentRect.height * 1/40, 0 );
		Vector2 spacing  = new( 0, parentRect.height * 1/20 );

		SetGridSize( _menuGridPanel, cellSize, padding, spacing);

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

	void SetUnitGrid( GridLayoutGroup grid_ )
	{
		Rect parentRect = grid_.GetComponent<RectTransform>().rect;
		Vector2 cellSize = new( parentRect.width, parentRect.height * 1/5);
		Vector4 padding  = new( 0,0,0,0 );
		Vector2 spacing  = new( parentRect.width * 1/5, parentRect.height * 1/5 );

		SetGridSize( grid_, cellSize, padding, spacing);

		for (int i = 0; i < grid_.transform.childCount; i++)
		{
			grid_.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
		}
	}

	void SetMenuPanel()
	{
		_menuGridPanel = Get<GridLayoutGroup>((int)GridPanels.MenuGridPanel);

		_btnContinue = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_btnContinue.SetCanvasSortOrder(ManagerRoot.UI.GetOrder());
		_btnContinue.SetButtonText("^text_panel_pause_btn_Resume");
		_btnContinue.SetLocalizeText();
		_btnContinue.SetClickedAction(OnContinueClick);
		_btnContinue.SetCanceledAction(OnContinueClick);
		_btns.Add(_btnContinue);


		_btnRestart = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_btnRestart.SetCanvasSortOrder(ManagerRoot.UI.GetOrder());
		_btnRestart.SetButtonText("^text_panel_pause_btn_Restart");
		_btnRestart.SetLocalizeText();
		_btnRestart.SetClickedAction(OnRestartClick);
		_btnRestart.SetCanceledAction(OnContinueClick);
		_btns.Add(_btnRestart);


		_btnKeymap = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_btnKeymap.SetCanvasSortOrder(ManagerRoot.UI.GetOrder());
		_btnKeymap.SetButtonText("^text_panel_pause_btn_Keymap");
		_btnKeymap.SetLocalizeText();
		_btnKeymap.SetClickedAction(OnKeyMapClick);
		_btnKeymap.SetCanceledAction(OnContinueClick);
		_btns.Add(_btnKeymap);

		_btnSetting = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_btnSetting.SetCanvasSortOrder(ManagerRoot.UI.GetOrder());
		_btnSetting.SetButtonText("^text_panel_pause_btn_Settings");
		_btnSetting.SetLocalizeText();
		_btnSetting.SetClickedAction(OnSettingClick);
		_btnSetting.SetCanceledAction(OnContinueClick);
		_btns.Add(_btnSetting);

		_btnToTitle = ManagerRoot.UI.MakeSubItem<UIPopupButton>(_menuGridPanel.transform);
		_btnToTitle.SetCanvasSortOrder(ManagerRoot.UI.GetOrder());
		_btnToTitle.SetButtonText("^text_panel_pause_btn_Totitle");
		_btnToTitle.SetLocalizeText();
		_btnToTitle.SetClickedAction(OnToTitleClick);
		_btnToTitle.SetCanceledAction(OnContinueClick);
		_btns.Add(_btnToTitle);

		SetMenuGrid();
	}

	void SetSkillPanel()
	{
		_skillGridPanel = Get<GridLayoutGroup>((int)GridPanels.SkillGridPanel);

		List<SkillData> skillDataList = ManagerRoot.Skill.GetSkillListByType(UnitType.None);
		int i = 0;
		foreach(SkillData skillData in skillDataList)
		{
			UIIconSkill uIIconSkill = ManagerRoot.UI.MakeSubItem<UIIconSkill>(_skillGridPanel.transform);
			uIIconSkill.SetFivotX(200);
			if (skillData != null)
			{
				SkillDataAttribute _skillDataAttribute = skillData.GetCurSkillData(0);
				uIIconSkill.SetIconInfo( _skillDataAttribute.IconSprite);
				uIIconSkill.SetSkillId(_skillDataAttribute);
				uIIconSkill.SetIndex((uint)i++);
			}
			uIIconSkill.SetCanceledAction(OnContinueClick);
		}

		SetSkillGrid();
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
		Debug.Log($"UIPausePopup | MakeUnitContainer | {unitType_}");
		UIUnitContainerPopup unitConta = ManagerRoot.UI.MakeSubItem<UIUnitContainerPopup>(_unitGridPanel.transform);
		unitConta.SetUnitGrid(Get<GridLayoutGroup>((int)GridPanels.UnitSkillGridPanel), Get<GridLayoutGroup>((int)GridPanels.UnitInfoGridPanel), Get<TMP_Text>((int)Texts.UnitInfoNameText));
		unitConta.SetCanceledAction(OnContinueClick);

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


	void OnContinueClick(string btnText)
	{
		Debug.Log("UIPausePopup | 게임 계속");
		GameManager.Instance.PauseGame();
	}

	void OnRestartClick(string btnText)
	{
		Debug.Log("UIPausePopup | 씬 리로드");
		GameManager.Instance.PauseGame();

		SceneManagerEx.ReloadScene();
	}

	void OnKeyMapClick(string btnText)
	{
		Debug.Log("UIPausePopup | 키맵 버튼");

		GameUIManager.Instance.ShowKeymapPopup(this);
		this.enabled = false; // 이게 왜 안먹지?
	}

	void OnSettingClick(string btnText)
	{
		Debug.Log("UIPausePopup | 설정 버튼");

		GameUIManager.Instance.ShowSettingsPopup(this);
		this.enabled = false;
	}

	void OnToTitleClick(string btnText)
	{
		Debug.Log("UIPausePopup | 타이틀로");
		GameManager.Instance.PauseGame();

		SceneManagerEx.LoadScene("Title");
	}


	void ClearGridLayoutGroup(Transform gridTransform_)
	{
		if (gridTransform_ == null) { return; }
		foreach (Transform child in gridTransform_)
		{
			Destroy(child.gameObject);
		}
	}

	public void SetPosition(Vector3 position_)
	{
		transform.position = position_;
	}
	void LoadSprite()
	{
		Texture2D spearTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Humans/Spear");
		Sprite spearSprite = Sprite.Create(spearTex, new Rect(6, 0, 22, 16), new Vector2(0.5f, 0.5f));
		_unitSpriteDic.Add(UnitType.SpearMan, spearSprite);

		Texture2D skeletonTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/Skeleton");
		Sprite skeletonSprite = Sprite.Create(skeletonTex, new Rect(6, 0, 22, 16), new Vector2(0.5f, 0.5f));
		_unitSpriteDic.Add(UnitType.SwordMan, skeletonSprite);

		Texture2D archerTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/SkeletonArcher");
		Sprite skeArcherSprite = Sprite.Create(archerTex, new Rect(6, 0, 22, 16), new Vector2(0.5f, 0.5f));
		_unitSpriteDic.Add(UnitType.ArcherMan, skeArcherSprite);

		Texture2D ghostTex = ManagerRoot.Resource.Load<Texture2D>("Sprites/Units/Undeads/Ghost");
		Sprite ghostSprite = Sprite.Create(ghostTex, new Rect(5, 0, 21, 24), new Vector2(0.5f, 0.5f));
		_unitSpriteDic.Add(UnitType.Assassin, ghostSprite);
	}


	void EnableBtns()
	{
		foreach(var btn in _btns)
		{
			btn.gameObject.SetActive(true);
		}
	}

	void DisableBtns()
	{
		foreach(var btn in _btns)
		{
			btn.gameObject.SetActive(false);
		}
	}
}
